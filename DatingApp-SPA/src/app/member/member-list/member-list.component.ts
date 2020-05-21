import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/User';
import { PaginatedResult } from 'src/app/_models/Pagination';
import { Router, Route, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];

  constructor(
    private userService: UserService, 
    private alertifyService: AlertifyService,
    private activatedRoute: ActivatedRoute
    ) { }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers()
  {
    // this.userService.getUsers().subscribe((users: User[]) => {
    //   this.users = users
    // }, error => {
    //   this.alertifyService.error("Falha ao carregar os usuários")
    // })

     this.userService.getUsers(1, 5).subscribe((paginatedResult: PaginatedResult<User[]>) => {
       this.users = paginatedResult.result
     }, error => {
       this.alertifyService.error("Falha ao carregar os usuários")
     })
    
  }


}
