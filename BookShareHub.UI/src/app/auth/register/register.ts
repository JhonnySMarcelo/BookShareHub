import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { UserService } from '../../users/user.service';
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
  ) {}

  register() {
    this.userService
      .register({ username: this.username, email: this.email, password: this.password })
      .subscribe({
        next: () => {
          alert('User registered successfully!');
          this.router.navigate(['/login']);
        },
        error: (err) => {
          alert(getErrorMessage(err));
        },
      });
  }
}
