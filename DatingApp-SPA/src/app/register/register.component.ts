import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  //input property - passa componentes do pai para o filho. no caso, do home para o register
  @Input() valuesFromHome;

  //output property - passa eventis do filho para o pai
  @Output() cancelMode = new EventEmitter();

  model: any = {};

  constructor(private authService: AuthService, private alertify: AlertifyService) { }

  ngOnInit() {
  }

  register()
  {
    this.authService.register(this.model)
    .subscribe(
      sucess => this.alertify.success("registered sucessfully!"),
      error => this.alertify.error(error)
    )
  }

  cancel()
  {
    this.cancelMode.next(false); //enviando um evento falado que foi cancelado o metodo
    console.log(this.model);
  }

}
