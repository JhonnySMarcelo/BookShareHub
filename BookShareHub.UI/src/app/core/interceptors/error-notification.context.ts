import { HttpContextToken } from '@angular/common/http';

export const SUPPRESS_ERROR_NOTIFICATION = new HttpContextToken<boolean>(() => false);
