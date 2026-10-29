import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable } from 'rxjs';

import { UserResponse } from './models/UserResponse';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = '/api/users';
  constructor(private http: HttpClient) {}

  register(data: { username: string; email: string; password: string }): Observable<UserResponse> {
    return this.http.post<UserResponse>(`${this.apiUrl}/register`, data);
  }

  me(): Observable<UserResponse> {
    return this.http.get<UserResponse>(`${this.apiUrl}/me`);
  }
}
