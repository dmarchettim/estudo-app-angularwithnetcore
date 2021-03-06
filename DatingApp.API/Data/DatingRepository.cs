using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {
        public DataContext _context { get; set; }
        public DatingRepository(DataContext context)
        {
            _context = context;

        }
        public void Add<T>(T entity) where T : class
        {
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            var users =  _context.Users.Include(p => p.Photos)
                            .OrderByDescending(u => u.LastActive)
                            .AsQueryable();//.ToListAsync();

            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);

            if(userParams.Likers)
            {
                var userLiker = await GetUserLikes(userParams.UserId, userParams.Likers);
                users = users.Where(u => userLiker.Contains(u.Id));
            }

            if(userParams.Likees)
            {
                Console.WriteLine("entrou");
                var userLikee = await GetUserLikes(userParams.UserId, userParams.Likers);
                Console.WriteLine(userLikee.FirstOrDefault());
                users = users.Where(u => userLikee.Contains(u.Id));
            }

            //se o usuario ta filtrando por idade tbm
            if(userParams.MinAge != 18 || userParams.MaxAge != 99)
            {
                var minDateOfBirthday = DateTime.Today.AddYears(- userParams.MaxAge -1);
                var maxDateOfBirthday = DateTime.Today.AddYears(- userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDateOfBirthday && u.DateOfBirth <= maxDateOfBirthday);
            }

            if(!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            //utilizar o metodo static que criamos em PagedList para criar um objeto do tipo PagedList<T>..
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);            
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool liker)
        {
            var user = await _context.Users
                            .Include(x => x.Likers)
                            .Include(x => x.Likees)
                            .FirstOrDefaultAsync(u => u.Id == id);
            
            if(liker)
            {
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId); //isso faz com q retorne una lista só do campo selecionado no Select                
            }
            else
            {
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId); 
            }
        }

        public async Task<bool> SaveAll()
        {
           return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Like> GetLike(int id, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == id && u.LikeeId == recipientId);
        }

        public async Task<Message> GetMessage(int id)
        {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams)
        {
            var messages = _context.Messages
                            .Include(u => u.Sender).ThenInclude(p => p.Photos)
                            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                            .AsQueryable();

            switch(messageParams.MessageContainer)
            {
                case "Inbox": //ver todas as mensagens que enviaram para mim
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeteled == false);
                    break;
                case "Outbox": //er todas as mensagens que eu enviei
                    messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                default: //unread
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeteled == false && u.IsRead == false);
                    break;
            }

            //apos buscar, ordena as mensagens
            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId)
        {
            var messages = await _context.Messages
                            .Include(u => u.Sender).ThenInclude(p => p.Photos)
                            .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                            .Where(m => m.RecipientId == userId && m.RecipientDeteled == false
                             && m.SenderId == recipientId ||
                                    m.RecipientId == recipientId && m.SenderId == userId && 
                                    m.SenderDeleted == false)
                            .OrderByDescending(m => m.MessageSent)
                            .ToListAsync();
            
            return messages;
        }
    }
}