import { Component, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { FormsModule, NgForm } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { finalize } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';

import { UserService } from '../../users/user.service';
import { NotificationService } from '../../core/services/notification.service';
import { PasswordMatchValidatorDirective } from './password-match-validator.directive';
import { getErrorMessage } from '../../core/erros/error-utils';

@Component({
  selector: 'app-register',
  templateUrl: './register.html',
  styleUrl: './register.scss',
  imports: [
    CommonModule,
    FormsModule,
    RouterLink,
    PasswordMatchValidatorDirective,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
  ],
})
export class Register {
  username = '';
  email = '';
  password = '';
  confirmPassword = '';
  isSubmitting = signal(false);
  registrationError = signal('');

  constructor(
    private userService: UserService,
    private router: Router,
    private notification: NotificationService,
  ) {}

  register(form: NgForm) {
    if (form.invalid || this.isSubmitting()) {
      return;
    }

    this.registrationError.set('');
    this.isSubmitting.set(true);

    this.userService
      .register({
        username: this.username.trim(),
        email: this.email.trim(),
        password: this.password,
      })
      .pipe(finalize(() => this.isSubmitting.set(false)))
      .subscribe({
        next: () => {
          this.notification.success('User registered successfully!');
          this.router.navigate(['/login']);
        },
        error: (error: HttpErrorResponse) => {
          const message = error.error?.detail ?? getErrorMessage(error);
          this.registrationError.set(message);
        },
      });
  }
}
