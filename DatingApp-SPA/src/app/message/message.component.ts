import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

import { UserService } from '../_services/user.service';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Message } from '../_models/Message';
import { Pagination, PaginatedResult } from '../_models/Pagination';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  messageContainer = 'Unread';

  constructor(private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService,
    private route: ActivatedRoute) { }

  ngOnInit() {
    this.messages = this.route.snapshot.data['messageResolver'].result;
    this.pagination = this.route.snapshot.data['messageResolver'].pagination;
    console.log(this.messages);
  }

  loadMessages(){
    this.userService.getMessages(this.authService.decodedToken.nameid,
      this.pagination.currentPage, this.pagination.itemsPerPage, this.messageContainer)
      .subscribe((res: PaginatedResult<Message[]>) => {
        this.messages = res.result;
        this.pagination = res.pagination;
      }, error => {
        this.alertify.error(error);
      });
  }

  deleteMessage(messageId: number){
    this.alertify.confirm("Tem certeza que deseja deletar a mensagem?", () => {
      this.userService.deleteMessage(messageId, this.authService.decodedToken.nameid)
        .subscribe(() => {
          this.messages.splice(this.messages.findIndex(m => m.id === messageId), 1);
          this.alertify.success("Mensagem deletada com sucesso!");
        }, error => {
          this.alertify.error(error);
        });
    });
  }

  onPageChanged(event: any){
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }

}
