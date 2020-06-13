import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { Injectable } from "@angular/core";

import { User } from "../_models/User";
import { UserService } from "../_services/user.service";
import { AlertifyService } from "../_services/alertify.service";
import { catchError } from "rxjs/operators";
import { Observable, of } from "rxjs";
import { PaginatedResult } from "../_models/Pagination";
import { Message } from "../_models/Message";
import { AuthService } from "../_services/auth.service";

@Injectable({
    providedIn: 'root'
  })
export class MessagesResolver implements Resolve<Message[]> {
    pageNumber = 1;
    pageSize = 5;
    messageContainer = 'Unread';

    constructor(private userService: UserService,
        private authService: AuthService,
        private alertify: AlertifyService,
        private router: Router){ }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        console.log(route);
        return this.userService.getMessages(this.authService.decodedToken.nameid,
            this.pageNumber, this.pageSize, this.messageContainer)
        .pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/home']);
                return of(null);
             })
        );
    }
}