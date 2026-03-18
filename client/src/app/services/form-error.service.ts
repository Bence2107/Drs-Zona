import {Injectable} from '@angular/core';
import {FormControl, FormGroup} from '@angular/forms';
import {HttpValidationError} from './error-interceptor.service';

@Injectable({
  providedIn: 'root'
})
export class FormErrorService {

  applyServerErrors(form: FormGroup, error: HttpValidationError, fieldMap: { [key: string]: string }): void {
    if (!error?.fieldErrors) {
      form.get('email')?.setErrors({ serverError: error?.title ?? 'Ismeretlen hiba' });
      return;
    }

    let hasFieldError = false;
    for (const backendField of Object.keys(error.fieldErrors)) {
      const controlName = fieldMap[backendField] ?? backendField;
      const control = form.get(controlName);
      if (control) {
        control.setErrors({ serverError: error.fieldErrors[backendField][0] });
        hasFieldError = true;
      }
    }

    if (!hasFieldError && error.title) {
      form.get('email')?.setErrors({ serverError: error.title });
    }
  }

  clearServerErrorOnChange(controls: FormControl[]): void {
    controls.forEach(control => {
      control.valueChanges.subscribe(() => {
        if (control.hasError('serverError')) {
          const errors = { ...control.errors };
          delete errors['serverError'];
          control.setErrors(Object.keys(errors).length ? errors : null);
        }
      });
    });
  }
}
