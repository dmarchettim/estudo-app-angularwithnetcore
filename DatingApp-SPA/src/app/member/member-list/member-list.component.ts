import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { AlertifyService } from '../../_services/alertify.service';
import { User } from '../../_models/User';
import { PaginatedResult, Pagination } from 'src/app/_models/Pagination';
import { Router, Route, ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination;

  constructor(
    private userService: UserService, 
    private alertifyService: AlertifyService,
    private activatedRoute: ActivatedRoute
    ) { }

  ngOnInit() {
    this.users = this.activatedRoute.snapshot.data['usersResolver'].result;
    this.pagination = this.activatedRoute.snapshot.data['usersResolver'].pagination;
    //this.loadUsers();
  }

  loadUsers()
  {
    // this.userService.getUsers().subscribe((users: User[]) => {
    //   this.users = users
    // }, error => {
    //   this.alertifyService.error("Falha ao carregar os usuários")
    // })

      this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage)
      .subscribe((paginatedResult: PaginatedResult<User[]>) => {
        this.users = paginatedResult.result;
        this.pagination = paginatedResult.pagination;
      }, error => {
        this.alertifyService.error("Falha ao carregar os usuários")
      })

     
    
  }

  onPageChanged(event: any){
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }


}
