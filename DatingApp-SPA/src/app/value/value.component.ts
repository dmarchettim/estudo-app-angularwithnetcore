import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { take} from 'rxjs/operators';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit {

  value: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  getValues()
  {
    this.http.get('http://localhost:5000/api/values')
    .pipe(
      take(1)
      ).
      subscribe(response => this.value = response);
  }
}
