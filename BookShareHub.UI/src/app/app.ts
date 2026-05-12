import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <nav>
      <a routerLink="/login">Login</a> | <a routerLink="/register">Register</a> |
      <a routerLink="/books">Books</a>
    </nav>
    <router-outlet></router-outlet>
  `,
})
export class App {}
