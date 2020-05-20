import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { FormGroup, FormControl, Validators, FormBuilder } from '@angular/forms';
import { debounce, debounceTime } from 'rxjs/operators';
//import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker/public_api';
import { User } from '../_models/User';
import { Route } from '@angular/compiler/src/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {

  //input property - passa componentes do pai para o filho. no caso, do home para o register
  @Input() valuesFromHome;

  //output property - passa eventis do filho para o pai. Nesse caso, está passando true ou false para o home.component
  @Output() cancelMode = new EventEmitter();

  registerForm: FormGroup;
  model: any = {};
  user: User;
  //bsConfig: Partial<BsDatepickerConfig>;

  constructor(
    private authService: AuthService, 
    private alertify: AlertifyService, 
    private fb: FormBuilder,
    private router: Router) { }

  ngOnInit() {
    // this.registerForm = new FormGroup({
    //   username: new FormControl('Hello', Validators.required),
    //   password: new FormControl('', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]),
    //   confirmPassword: new FormControl('', Validators.required),
    // }, this.passwordMatchValidator);

    // this.bsConfig = {
    //   containerClass: 'theme-red'
    // }

    this.registerForm = this.fb.group({
      gender: ['male'],
      username: ['Hello', Validators.required],
      knownAs: ['', Validators.required],
      dateOfBirth: [null, Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(4), Validators.maxLength(8)]],
      confirmPassword: ['', Validators.required]
    }, {validator: this.passwordMatchValidator});

    //só relembrando o valueChanges e o debounceTime mesmo... nada demais!!
    // this.registerForm.get('username').valueChanges
    // .pipe( 
    //   debounceTime(300))
    // .subscribe(
    //   (mudou) => {alert('valor mudou!')};
    // )
  }

  passwordMatchValidator(g: FormGroup){
    return g.get('password').value === g.get('confirmPassword').value ? null : {'mismatch': true};
  }

  register(){
    // 
    // this.authService.register(this.model)
    // .subscribe(
    //   sucess => this.alertify.success("registered sucessfully!"),
    //   error => this.alertify.error(error)
    // );

    if(this.registerForm.valid){
      this.user = Object.assign({}, this.registerForm.value); //javascript, irá colocar o registerForm.value no objeto vazio e entao atribuir para this.user
      this.authService.register(this.user)
       .subscribe(
         sucess => this.alertify.success("registered sucessfully!"),
         error => this.alertify.error(error),
         () => {
           //o complete será redirecionar para a pagina de login
           this.authService.login(this.user)
           .subscribe( () => {
             this.router.navigate(['/members']);
           });
         }
       );
    }
    console.log(this.registerForm.value);
  }

  cancel(){
    this.cancelMode.next(false); //enviando um evento falado que foi cancelado o metodo
    console.log(this.model);
  }

}
