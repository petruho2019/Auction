import { inject } from '@angular/core';
import { CanActivateFn } from '@angular/router';
import { AuthService } from '../../../services/AuthService';

export const authorizeGuard: CanActivateFn = (route, state) => {
  
  

  return true;
};
