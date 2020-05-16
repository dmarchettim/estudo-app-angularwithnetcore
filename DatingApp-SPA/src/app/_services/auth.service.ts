import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take, tap, map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { User } from '../_models/User';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  baseURL: string = 'http://localhost:5000/api/auth/';
  helper = new JwtHelperService();
  decodedToken: any;
  currentUser: User;
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhotoUrl = this.photoUrl.asObservable();

  constructor(private http: HttpClient) { }

  changeMemberPhoto(photoUrl: string){
    this.photoUrl.next(photoUrl);
  }

  login(model: any){
    return this.http.post(this.baseURL + 'login', model)
    .pipe(
      map( (response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user));
          this.decodedToken = this.helper.decodeToken(user.token);
          this.currentUser = user.user;
          this.changeMemberPhoto(this.currentUser.photoURL);
          console.log(this.decodedToken);
        }
      })
      );
  }

  register(model: any)
  {
    return this.http.post(this.baseURL + 'register', model).pipe(take(1));    
  }

  loggedIn()
  {
    const token = localStorage.getItem('token');

    //se o token estiver expirado, entao o usuario nao está logado
    return !this.helper.isTokenExpired(token);
  }

}
