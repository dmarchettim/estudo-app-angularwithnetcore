import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode = false;
  value: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  registerToggle()
  {
    this.registerMode = !this.registerMode;
  }

  getValues()
  {
    this.http.get('http://localhost:5000/api/values')
    .pipe(
      take(1)
      ).
      subscribe(response => this.value = response);
  }

  onCancelMode(evento: any)
  {
    console.log(evento);
    this.registerMode = evento;
  }

}
