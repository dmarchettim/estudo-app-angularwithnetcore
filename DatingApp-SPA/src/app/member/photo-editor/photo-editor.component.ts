import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

import { Photo } from 'src/app/_models/Photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  @Input() photos: Photo[];
  @Output() getMemberPhotoChange = new EventEmitter<string>();
  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;
  baseUrl = environment.apiUrl;
  currentPhoto: Photo;

  constructor(private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService){}

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  fileOverAnother(e: any): void {
    this.hasAnotherDropZoneOver = e;
  }

  ngOnInit() {
    this.uploadArquivos();
  }

  uploadArquivos()
  {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false
      //disableMultipart: false, // 'DisableMultipart' must be 'true' for formatDataFunction to be called.
      // formatDataFunctionIsAsync: true,
      //  formatDataFunction: async (item) => {
      //    return new Promise( (resolve, reject) => {
      //      resolve({
      //        name: item._file.name,
      //        length: item._file.size,
      //        contentType: item._file.type,
      //        date: new Date()
      //      });
      //    });
      //  }
    });

    this.hasBaseDropZoneOver = false;
    this.hasAnotherDropZoneOver = false;

    this.response = '';

    this.uploader.response.subscribe( res => this.response = res );
    
    //resolvendo o problema do CORS - ref. https://github.com/valor-software/ng2-file-upload/issues/1018
    this.uploader.onAfterAddingFile = (file) => { file.withCredentials = false; };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
        };
        this.photos.push(photo);
        if(photo.isMain){
          this.authService.changeMemberPhoto(photo.url);

          // aqui, é preciso atualiar no usuário também, porque qdo carrega, pelo AppComponent, ele obtem as informações pelo que está
          //na section do 'user'. Embora persistido no banco de dados já, como nos vamos mandar o user que está no authuser, ele nao ira
          //pegar as infos do banco de dados pois não foi dado novo login(). Assim, a unica maneiara
          //de atualizar é setando a foto na variavel currentUser do authService
          this.authService.currentUser.photoURL = photo.url;

          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        }
      };
    }
  }

  setMainPhoto(photo: Photo)
  {
    this.userService.setMainPhoto(this.authService.decodedToken.nameid, photo.id)
    .subscribe(
      sucess => {
        //console.log(`Foto ${photo.id} alterada para main com sucesso!!`)
        this.currentPhoto = this.photos.filter(p => p.isMain === true)[0];
        this.currentPhoto.isMain = false;
        photo.isMain = true;
        //this.getMemberPhotoChange.emit(photo.url);
        this.authService.changeMemberPhoto(photo.url);

        // aqui, é preciso atualiar no usuário também, porque qdo carrega, pelo AppComponent, ele obtem as informações pelo que está
        //na section do 'user'. Embora persistido no banco de dados já, como nos vamos mandar o user que está no authuser, ele nao ira
        //pegar as infos do banco de dados pois não foi dado novo login(). Assim, a unica maneiara
        //de atualizar é setando a foto na variavel currentUser do authService
        this.authService.currentUser.photoURL = photo.url;

        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
      }, error => {
        this.alertify.error(error)
      }
    )
  }

  deletePhoto(id: number){
    this.alertify.confirm("Are you sure you want to delete the photo?", () => {
      this.userService.deletePhoto(this.authService.decodedToken.nameid, id).subscribe(() => {
        //utilizar a função splice, e nela precisamos encontrar o index e passar a qtde de fotos q queremos deletar
        this.photos.splice(this.photos.findIndex(p => p.id == id), 1);
        this.alertify.success("Foto deletada!");
      }, error => {
        this.alertify.error("Erro ao deletar a foto!");
      })
    })
  }

}
