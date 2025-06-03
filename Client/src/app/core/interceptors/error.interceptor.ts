import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import { SnackbarService } from '../services/snackbar.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackbarService = inject(SnackbarService);
  
  // The default response type for an HTTP 400 response is ValidationProblemDetails.

  // {
  //   "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  //   "title": "One or more validation errors occurred.",
  //   "status": 400,
  //   "traceId": "|7fb5e16a-4c8f23bbfc974667.",
  //   "errors": {
  //     "": [
  //       "A non-empty request body is required."
  //     ]
  //   }
  // }

  // https://learn.microsoft.com/en-us/aspnet/core/web-api/?view=aspnetcore-9.0

  return next(req).pipe(
    catchError((err: HttpErrorResponse) => {
      if (err.status === 400) {
        //alert(err.error.title || err.error);
        if (err.error.errors) {
          const modelSateErrors = [];
          for(const key in err.error.errors) {
            if (err.error.errors[key]) {
              modelSateErrors.push(err.error.errors[key]);
            }
          }
          throw modelSateErrors.flat();
        } else {
          snackbarService.error(err.error.title || err.error);
        }
      }

      if (err.status === 401) {
        //alert(err.error.title || err.error);
        snackbarService.error(err.error.title || err.error);
      }

      if (err.status === 403) {        
        snackbarService.error('Forbidden');
      }

      if (err.status === 404) {
        router.navigateByUrl('/not-found');
      }

      if (err.status === 500) {
        const navigationExtras: NavigationExtras = {state: {error: err.error}};
        router.navigateByUrl('/server-error', navigationExtras);
      }

      return throwError(() => err);
    })
  )
};
