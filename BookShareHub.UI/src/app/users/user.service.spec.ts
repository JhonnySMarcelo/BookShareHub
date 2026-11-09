import { provideHttpClient } from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { vi } from 'vitest';

import { SUPPRESS_ERROR_NOTIFICATION } from '../core/interceptors/error-notification.context';
import { UserService } from './user.service';

describe('UserService', () => {
  let service: UserService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [provideHttpClient(), provideHttpClientTesting()],
    });

    service = TestBed.inject(UserService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => httpTestingController.verify());

  it('should suppress global notifications for registration errors', () => {
    const errorHandler = vi.fn();
    service.register({ username: 'taken', email: 'user@example.com', password: 'password123' })
      .subscribe({ error: errorHandler });

    const request = httpTestingController.expectOne('/api/users/register');
    expect(request.request.context.get(SUPPRESS_ERROR_NOTIFICATION)).toBe(true);

    request.flush(
      { detail: 'Username or email is already in use.' },
      { status: 409, statusText: 'Conflict' },
    );

    expect(errorHandler).toHaveBeenCalled();
  });
});
