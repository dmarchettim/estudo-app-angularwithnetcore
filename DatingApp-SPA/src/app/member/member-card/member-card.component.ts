import { Component, OnInit, Input, Inject } from '@angular/core';
import { Router } from '@angular/router';

import { User } from 'src/app/_models/User';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';


@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(private router: Router,
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService) { }

  ngOnInit() {
  }

  onClick(id: number){
   this.router.navigate(['members', id]);
   //console.log(`members/${id}`);
  }

  sendLike(id: number){
    this.userService.sendLike(this.authService.decodedToken.nameid, id)
      .subscribe(data => {
        this.alertify.success("You have liked " + this.user.knownAs);
      }, error => {
        this.alertify.error(error);
      });
  }

}
