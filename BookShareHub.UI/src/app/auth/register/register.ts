import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  imports: [[FormsModule]],
})
export class Register {
  username = '';
  email = '';
  password = '';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  register() {
    this.authService
      .register({ username: this.username, email: this.email, password: this.password })
      .subscribe({
        next: () => {
          alert('User registered successfully!');
          this.router.navigate(['/login']);
        },
        error: () => alert('Failed to register'),
      });
  }
}
