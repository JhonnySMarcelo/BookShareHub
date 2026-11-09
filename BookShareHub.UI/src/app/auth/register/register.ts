import { Component } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { UserService } from '../../users/user.service';
import { NotificationService } from '../../core/services/notification.service';
import { getErrorMessage } from '../../core/erros/error-utils';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrl: './register.scss',
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
})
export class Register {
  username = '';
  email = '';
  password = '';

  constructor(
    private userService: UserService,
    private router: Router,
    private notification: NotificationService,
  ) {}

  register() {
    this.userService
      .register({ username: this.username, email: this.email, password: this.password })
      .subscribe({
        next: () => {
          this.notification.success('User registered successfully!');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          this.notification.error(getErrorMessage(err));
        },
      });
  }
}
