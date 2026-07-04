import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { AuthService } from '../core/services/auth.service';
import { Router } from '@angular/router';
import { RouterLink } from '@angular/router';
import { NavigationItem } from './navigation-item';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './header.html',
  styleUrl: './header.scss',
})
export class Header {
  constructor(
    public authService: AuthService,
    private router: Router,
  ) {}

  logout() {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  get navItems(): NavigationItem[] {
    const currentRoute = this.router.url;

    if (this.authService.isLoggedIn()) {
      return [
        {
          label: 'Logout',
          action: () => this.logout(),
        },
      ];
    }

    switch (currentRoute) {
      case '/login':
        return [
          {
            label: 'Books',
            route: '/books',
          },
        ];

      case '/register':
        return [
          {
            label: 'Books',
            route: '/books',
          },
        ];

      case '/forgot-password':
        return [
          {
            label: 'Books',
            route: '/books',
          },
          {
            label: 'Login',
            route: '/login',
          },
          {
            label: 'Register',
            route: '/register',
          },
        ];

      default:
        return [
          {
            label: 'Books',
            route: '/books',
          },
          {
            label: 'Login',
            route: '/login',
          },
          {
            label: 'Register',
            route: '/register',
          },
        ];
    }
  }
}
