import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  constructor(private http: HttpClient) {}

  register(data: any): Observable<any> {
    return this.http.post('/api/users/register', data);
  }

  login(data: any): Observable<any> {
    return this.http.post('/api/users/login', data);
  }

  isAuthenticated(): boolean {
    return !!localStorage.getItem('jwt');
  }

  logout(): void {
    localStorage.removeItem('jwt');
  }
}
