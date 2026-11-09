import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpErrorResponse } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { NgForm } from '@angular/forms';
import { throwError } from 'rxjs';
import { vi } from 'vitest';

import { Register } from './register';
import { UserService } from '../../users/user.service';
import { NotificationService } from '../../core/services/notification.service';

describe('Register', () => {
  let component: Register;
  let fixture: ComponentFixture<Register>;
  const userService = { register: vi.fn() };

  beforeEach(async () => {
    userService.register.mockReset();

    await TestBed.configureTestingModule({
      imports: [Register],
      providers: [
        provideRouter([]),
        { provide: UserService, useValue: userService },
        { provide: NotificationService, useValue: { success: vi.fn(), error: vi.fn() } },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(Register);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should show a registration error and reset submitting state after a fast failure', () => {
    userService.register.mockReturnValue(
      throwError(
        () =>
          new HttpErrorResponse({
            status: 409,
            error: { detail: 'Username or email is already in use.' },
          }),
      ),
    );

    component.register({ invalid: false } as NgForm);
    fixture.detectChanges();

    expect(component.isSubmitting()).toBe(false);
    expect(fixture.nativeElement.textContent).toContain('Username or email is already in use.');
  });
});
