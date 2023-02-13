import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'Dating App';
  users: any;
  constructor(private http: HttpClient){}
  ngOnInit(): void {
    this.http.get(`https://localhost:3001/api/users`).subscribe({
      next: (resp) => this.users = resp,
      error: (err) => console.log({getUserErr: err}),
      complete: () => console.log('Get users request completed.')
    })
  }
  
}
