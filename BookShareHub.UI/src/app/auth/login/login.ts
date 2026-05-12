import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  imports: [[FormsModule]],
})
export class Login {
  email = '';
  password = '';

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  login() {
    this.authService.login({ email: this.email, password: this.password }).subscribe({
      next: (res: any) => {
        this.authService.saveToken(res.token);
        this.router.navigate(['/books']);
      },
      error: () => alert('Invalid credentials'),
    });
  }
}
