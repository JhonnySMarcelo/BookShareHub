import { Routes } from '@angular/router';
import { Register } from './auth/register/register';
import { Login } from './auth/login/login';
import { BookList } from './books/book-list/book-list';
import { authGuard } from './auth/auth.guard';

export const routes: Routes = [
  { path: 'register', component: Register },
  { path: 'login', component: Login },
  { path: 'books', component: BookList, canActivate: [authGuard] },
  { path: '', redirectTo: 'books', pathMatch: 'full' },
];
