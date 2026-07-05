import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';

import { UserService } from '../../users/user.service';
import { NotificationService } from '../../core/services/notification.service';
import { getErrorMessage } from '../../core/erros/error-utils';

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
