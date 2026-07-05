import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';

import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
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
    private notification: NotificationService,
  ) {}

  onSubmit() {
    this.authService.login(this.loginData).subscribe({
      next: () => {
        this.router.navigate(['/books']);
      },
      error: (err) => {
        if (err.status === 401) {
          this.notification.error('Invalid email or password. Please try again.');
        } else {
          this.notification.error(getErrorMessage(err));
        }
      },
    });
  }
}
