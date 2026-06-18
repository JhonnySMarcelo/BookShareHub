import { ErrorHandler, Injectable } from '@angular/core';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  handleError(error: unknown): void {
    console.error('[GLOBAL ERROR]', error);

    alert('An unexpected application error occurred. Please refresh the page and try again.');
  }
}
