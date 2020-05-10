import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { NgForm } from '@angular/forms';

import { User } from 'src/app/_models/User';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  user: User;
  @ViewChild('editForm') editForm: NgForm;

  constructor(private activeRoute: ActivatedRoute, 
    private alertify: AlertifyService, 
    private authService: AuthService,
    private userService: UserService
    ) { }

  //o canDeactivate funciona apenas se sair da pagina, não funciona se fecharmos a janela. No caso, precisamos
  //implementar um HostListener para prevenir também de sair da página. ;)
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any){
    if(this.editForm.dirty){
      $event.returnValue = true;
    }
  }


  ngOnInit() {
    this.loadUser();
  }

  loadUser(){
    this.user = this.activeRoute.snapshot.data['currentUser'];
  }

  updateUser(){      
    console.log(this.user);
    this.userService.updateUser(this.authService.decodedToken.nameid, this.user)
    .subscribe(sucess => {
      this.alertify.success("Update sucessfully!!!!");
    }, error =>{
      this.alertify.error("Erro ao atualizar o usuário!");
      console.log(error);
    });

    //resetar o form, obtendo-o através do ViewChild, e então colocá-lo preenchido com as informações do usuário
    this.editForm.reset(this.user);
  }

}
