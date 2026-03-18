import {HttpErrorResponse, HttpInterceptorFn} from '@angular/common/http';
import {catchError, throwError} from 'rxjs';

export interface HttpValidationError {
  title: string;
  fieldErrors: { [key: string]: string[] };
}

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 400 || error.status === 401) {
        return throwError(() => parseValidationErrors(error.error));
      } else if (error.status === 404) {
        return throwError(() => ({ title: 'Resource not found', fieldErrors: {} }));
      } else if (error.status === 500) {
        return throwError(() => ({ title: 'Internal server error', fieldErrors: {} }));
      } else {
        return throwError(() => ({ title: `Server returned code ${error.status}`, fieldErrors: {} }));
      }
    })
  );
};

function parseValidationErrors(errorResponse: any): HttpValidationError {
  try {
    let errorObj = errorResponse;
    if (typeof errorResponse === 'string') {
      errorObj = JSON.parse(errorResponse);
    }

    const fieldErrors: { [key: string]: string[] } = {};

    if (errorObj?.field && errorObj?.message) {
      fieldErrors[errorObj.field.toLowerCase()] = [errorObj.message];
    }

    if (errorObj?.errors) {
      for (const field in errorObj.errors) {
        if (Object.prototype.hasOwnProperty.call(errorObj.errors, field)) {
          fieldErrors[field.toLowerCase()] = errorObj.errors[field];
        }
      }
    }

    return {
      title: errorObj?.title || errorObj?.message || 'Validation error occurred',
      fieldErrors
    };
  } catch (e) {
    return { title: 'An error occurred', fieldErrors: {} };
  }
}
