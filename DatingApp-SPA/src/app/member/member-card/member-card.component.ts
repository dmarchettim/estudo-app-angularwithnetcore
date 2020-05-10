import { Component, OnInit, Input, Inject } from '@angular/core';
import { User } from 'src/app/_models/User';
import { Router } from '@angular/router';


@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;

  constructor(private router: Router) { }

  ngOnInit() {
  }

  onClick(id: number){
   this.router.navigate(['members', id]);
   //console.log(`members/${id}`);
  }

}
