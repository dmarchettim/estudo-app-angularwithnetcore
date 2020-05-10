import { Resolve, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from "@angular/router";
import { Injectable } from "@angular/core";

import { User } from "../_models/User";
import { UserService } from "../_services/user.service";
import { AlertifyService } from "../_services/alertify.service";
import { catchError } from "rxjs/operators";
import { Observable, of } from "rxjs";

@Injectable({
    providedIn: 'root'
  })
export class MemberDetailResolver  implements Resolve<User> {

    constructor(private userService: UserService,
        private alertify: AlertifyService,
        private router: Router){ }

    resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot) {
        console.log(route);
        return this.userService.getUser(route.params['id']).pipe(
            catchError(error => {
                this.alertify.error(error);
                this.router.navigate(['/members']);
                return of(null);
             })
        );
    }
}