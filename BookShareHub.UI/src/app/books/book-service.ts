import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';

import { Observable, tap, catchError, throwError } from 'rxjs';

import { NotificationService } from '../core/services/notification.service';
import { getErrorMessage } from '../core/erros/error-utils';
import { Book } from './models/Book';

@Injectable({ providedIn: 'root' })
export class BookService {
  private apiUrl = '/api/books';

  constructor(
    private http: HttpClient,
    private notification: NotificationService,
  ) {}

  getAll(): Observable<Book[]> {
    return this.http.get<Book[]>(this.apiUrl).pipe(
      catchError((err) => {
        this.notification.error(getErrorMessage(err));
        return throwError(() => err);
      }),
    );
  }

  getById(id: string): Observable<Book> {
    return this.http.get<Book>(`${this.apiUrl}/${id}`).pipe(
      catchError((err) => {
        this.notification.error(getErrorMessage(err));
        return throwError(() => err);
      }),
    );
  }

  create(book: Partial<Book>): Observable<Book> {
    return this.http.post<Book>(this.apiUrl, book).pipe(
      tap(() => this.notification.success('Book created successfully!')),
      catchError((err) => {
        this.notification.error(getErrorMessage(err));
        return throwError(() => err);
      }),
    );
  }

  update(id: string, book: Partial<Book>): Observable<Book> {
    return this.http.patch<Book>(`${this.apiUrl}/${id}`, book).pipe(
      tap(() => this.notification.success('Book updated successfully!')),
      catchError((err) => {
        this.notification.error(getErrorMessage(err));
        return throwError(() => err);
      }),
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      tap(() => this.notification.success('Book deleted successfully!')),
      catchError((err) => {
        this.notification.error(getErrorMessage(err));
        return throwError(() => err);
      }),
    );
  }
}
