using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private Cloudinary _cloudinary;
        public IDatingRepository _repository { get; }
        public IMapper _mapper { get; }
        public IOptions<CloudinarySettings> _cloudinaryConfig { get; }

        public PhotosController(IDatingRepository repository, 
        IMapper mapper, 
        IOptions<CloudinarySettings> cloudinaryConfig) //options porque foi fornecido como serviço (services) na Startup.cs
        {
            _repository = repository;
            _mapper = mapper;
            _cloudinaryConfig = cloudinaryConfig;

            //inicializando o CloudinaryConfig
            //see more in documentation https://cloudinary.com/documentation/dotnet_integration#overview

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.APIKey,
                _cloudinaryConfig.Value.APISecret
            );

            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repository.GetPhoto(id);

            //if(photoFromRepo == null)
            //return NotFound();

            var photo = _mapper.Map<PhotoForReturnDTO>(photoFromRepo);

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm] PhotoForCreationDTO photoForCreationDTO)
        //FromForm porque estamos recebendo um form e não um json no body
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

            var userFromRepo = await _repository.GetUser(userId); 

            var file = photoForCreationDTO.File;

            var uploadResult = new ImageUploadResult();    

            if(file.Length > 0 )
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Width(500).Crop("fill").Gravity("face")
                    };

                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            } 

            photoForCreationDTO.Url = uploadResult.Uri.ToString();  
            photoForCreationDTO.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDTO);   

            if(!userFromRepo.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userFromRepo.Photos.Add(photo);

            if(await _repository.SaveAll())
            {
                //o photoToReturn está aqui apenas porque o photo.Id só é gerado após persistir no banco
                var photoToReturn = _mapper.Map<PhotoForReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new { userId, id = photo.Id}, photoToReturn);
                //return Ok(photoToReturn);
            }

            return BadRequest("Could not upload the photo");

        }

       
        [HttpPost]
        [Route("{photoId}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int photoId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

            //return Ok();
            
            var user = await _repository.GetUser(userId);

            if(user == null) return NotFound("Usuário não encontrado!");

            if(!user.Photos.Any(p => p.Id == photoId)) return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(photoId);

            if(photoFromRepo == null) return NotFound("Foto não encontrada!");

            if(photoFromRepo.IsMain) return BadRequest("Já é a principal foto!");

            var currentMainPhoto = await _repository.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if(await _repository.SaveAll())
            return NoContent();

            return BadRequest("Não foi possível setar a foto como principal!");  
        }

        [HttpDelete("{photoId}")]
        public async Task<IActionResult> DeletePhoto(int userId, int photoId)
        {
            if(userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
             return Unauthorized();

            //return Ok();
            
            var user = await _repository.GetUser(userId);

            if(user == null) return NotFound("Usuário não encontrado!");

            if(!user.Photos.Any(p => p.Id == photoId)) return Unauthorized();

            var photoFromRepo = await _repository.GetPhoto(photoId);

            if(photoFromRepo == null) return NotFound("Foto não encontrada!");

            if(photoFromRepo.IsMain) return BadRequest("Não é permitido deletar a foto principal!");

            //deletando do Cloudinary        
            //obtido da documentação do Cloudinary

            if(photoFromRepo.PublicId != null)
            {
                var deletionParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deletionParams);

                if(result.Result == "ok")
                {
                    _repository.Delete(photoFromRepo);
                }
            }

            if(photoFromRepo.PublicId == null)
            {
                _repository.Delete(photoFromRepo);
            }  
            
            if(await _repository.SaveAll())
            return Ok();

            return BadRequest("Falha ao deletar a foto");
        }

        
    }
}