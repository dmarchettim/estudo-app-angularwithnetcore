import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { Injectable } from "@angular/core";

import { User } from "../_models/User";
import { UserService } from "../_services/user.service";
import { AlertifyService } from "../_services/alertify.service";
import { catchError } from "rxjs/operators";
import { Observable, of } from "rxjs";
import { PaginatedResult } from "../_models/Pagination";

@Injectable({
    providedIn: 'root'
  })
export class ListsResolver implements Resolve<User[]> {
    pageNumber = 1;
    pageSize = 5;
    likeParams = 'Likers';

    constructor(private userService: UserService,
        private alertify: AlertifyService,
        private router: Router){ }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        console.log(route);
        return this.userService.getUsers(this.pageNumber, this.pageSize, null, this.likeParams)
        .pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/home']);
                return of(null);
             })
        );
    }
}