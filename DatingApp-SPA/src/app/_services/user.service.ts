import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/User';
import { take, map } from 'rxjs/operators';
import { PaginatedResult } from '../_models/Pagination';
import { Message } from '../_models/Message';

const httpOptions = {
  headers: new HttpHeaders({
    'Authorization': 'Bearer ' + localStorage.getItem('token')
  })
}

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) { }

  getUsers(page?, itemsPerPage?, userParams?, likeParams?): Observable<PaginatedResult<User[]>>{
    //return this.http.get<User[]>(this.baseUrl + 'users', httpOptions);

    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<User[]>();

    let params = new HttpParams();

    if(page != null && itemsPerPage != null){
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    if(userParams != null){
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if(likeParams === 'Likers'){
      params = params.append('likers', 'true');
    }

    if(likeParams === 'Likees'){
      params = params.append('likees', 'true');
    }


    return this.http.get<User[]>(this.baseUrl + 'users', { observe: 'response', params})
    .pipe(
      map(response => {
        paginatedResult.result = response.body;
        //console.log(response);
        if(response.headers.get('Pagination') != null){
             console.log(JSON.parse(response.headers.get('Pagination')))
          paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          console.log(paginatedResult.pagination);
        }
        return paginatedResult;
      })
    );
  }

  getUser(id): Observable<User>{
    //return this.http.get<User>(this.baseUrl + 'user/' + id, httpOptions);
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User){
    return this.http.put(this.baseUrl + 'users/' + id, user);
  }

  setMainPhoto(userId: number, photoId: number){
    return this.http.post(this.baseUrl + 'users/' + userId + '/photos/' + photoId + '/setMain', {}).pipe(take(1));
  }

  deletePhoto(userId: number, photoId: number){
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + photoId).pipe(take(1));
  }

  sendLike(id: number, recipientId: number){
    return this.http.post(this.baseUrl + 'users/' + id + '/like/' + recipientId, {}).pipe(take(1));
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?){
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<Message[]>();

    let params = new HttpParams();

    params = params.append('MessageContainer', messageContainer);

    if(page != null && itemsPerPage != null){
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    return this.http.get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {observe: 'response', params})
      .pipe(
        map(response => {
          paginatedResult.result = response.body;

          if(response.headers.get('Pagination') !== null){
            paginatedResult.pagination = JSON.parse(response.headers.get('Pagination'));
          }

          return paginatedResult;
        })
      );
  }

  getMessageThread(id: number, recipientId: number){
    return this.http.get<Message[]>(this.baseUrl +'users/' + id + '/messages/thread/' + recipientId);
  }

  sendMessage(id: number, message: string){
    return this.http.post(this.baseUrl + 'users/' + id + '/messages', message);
  }

  deleteMessage(messageId: number, userId: number){
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId, {});
  }

  markAsRead(userId: number, messageId: number){
    return this.http.post(this.baseUrl + 'users/' + userId + '/messages/' + messageId + '/read', {})
    .subscribe();
  }

}
