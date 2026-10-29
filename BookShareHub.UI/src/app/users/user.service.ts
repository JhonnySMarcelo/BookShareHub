import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { User } from './models/User';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = '/api/users';
  constructor(private http: HttpClient) {}

  register(data: { username: string; email: string; password: string }): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}/register`, data);
  }

  me(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/me`);
  }
}
