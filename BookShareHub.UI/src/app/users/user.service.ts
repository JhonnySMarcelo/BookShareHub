import { Injectable } from '@angular/core';
import { HttpClient, HttpContext } from '@angular/common/http';

import { Observable } from 'rxjs';

import { User } from './models/User';
import { SUPPRESS_ERROR_NOTIFICATION } from '../core/interceptors/error-notification.context';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = '/api/users';
  constructor(private http: HttpClient) {}

  register(data: { username: string; email: string; password: string }): Observable<User> {
    const context = new HttpContext().set(SUPPRESS_ERROR_NOTIFICATION, true);
    return this.http.post<User>(`${this.apiUrl}/register`, data, { context });
  }

  me(): Observable<User> {
    return this.http.get<User>(`${this.apiUrl}/me`);
  }
}
