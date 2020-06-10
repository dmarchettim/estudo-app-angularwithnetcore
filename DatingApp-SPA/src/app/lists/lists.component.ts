import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { User } from '../_models/User';
import { Pagination, PaginatedResult } from '../_models/Pagination';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likeParams: string;

  constructor(private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.users = this.route.snapshot.data['usersListResolver'].result;
    this.pagination = this.route.snapshot.data['usersListResolver'].pagination;
    this.likeParams = 'Likers';
  }

  loadUsers(){
      this.userService.getUsers(this.pagination.currentPage, this.pagination.itemsPerPage, null, this.likeParams)
      .subscribe((paginatedResult: PaginatedResult<User[]>) => {
        this.users = paginatedResult.result;
        this.pagination = paginatedResult.pagination;
      }, error => {
        this.alertify.error("Falha ao carregar os usuários")
      });
  }

  onPageChanged(event: any){
    this.pagination.currentPage = event.page;
    this.loadUsers();
  }

}
