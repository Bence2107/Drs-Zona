import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import {map, of} from 'rxjs';
import {catchError} from 'rxjs/operators';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const me$ = authService.getMe();

  if (!me$) return router.createUrlTree(['/auth']);

  return me$.pipe(
    map(user => user ? true : router.createUrlTree(['/auth'])),
    catchError(() => of(router.createUrlTree(['/auth'])))
  );
};

export const guestGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const me$ = authService.getMe();

  if (!me$) return true;

  return me$.pipe(
    map(user => user ? router.createUrlTree(['/home']) : true),
    catchError(() => of(true))
  );
};
