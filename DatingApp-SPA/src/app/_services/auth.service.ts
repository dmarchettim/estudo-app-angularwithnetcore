import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take, tap, map } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  baseURL: string = 'http://localhost:5000/api/auth/';
  helper = new JwtHelperService();
  decodedToken: any;

  constructor(private http: HttpClient) { }

  login(model: any){
    return this.http.post(this.baseURL + 'login', model)
    .pipe(
      map( (response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          this.decodedToken = this.helper.decodeToken(user.token);
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
