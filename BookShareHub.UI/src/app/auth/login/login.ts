import { Component } from '@angular/core';
import { AuthService } from '../auth.service';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { MatFormFieldModule } from '@angular/material/form-field';
import { CommonModule } from '@angular/common';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { getErrorMessage } from '../../core/erros/error-utils';

@Component({
  selector: 'app-login',
  templateUrl: './login.html',
  styleUrl: './login.scss',
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
  ],
})
export class Login {
  loginData = { email: '', password: '' };

  constructor(
    private authService: AuthService,
    private router: Router,
  ) {}

  onSubmit() {
    this.authService.login(this.loginData).subscribe({
      next: (res) => {
        this.router.navigate(['/books']);
      },
      error: (err) => {
        if (err.status === 401) {
          alert('Invalid email or password. Please try again.');
        } else {
          alert(getErrorMessage(err));
        }
      },
    });
  }
}
