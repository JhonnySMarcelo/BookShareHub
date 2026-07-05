import { ErrorHandler, Injectable } from '@angular/core';

import { NotificationService } from '../services/notification.service';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  constructor(private notification: NotificationService) {}

  handleError(error: unknown): void {
    console.error('[GLOBAL ERROR]', error);
    this.notification.error('An unexpected application error occurred. Please refresh the page.');
  }
}
