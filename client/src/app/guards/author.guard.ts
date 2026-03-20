import {CanActivateFn, Router} from '@angular/router';
import {inject} from '@angular/core';
import {AuthService} from '../services/api/auth.service';

export const authorGuard: CanActivateFn = () => {
    const authService = inject(AuthService);
    const router = inject(Router);

    if (authService.getUserRole() === 'Admin' || authService.getUserRole() === 'Author') {
      return true;
    }

    router.navigate(['/news']);
    return false;
};
