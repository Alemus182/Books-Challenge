import {
  HttpErrorResponse,
  HttpEvent,
  HttpHandler,
  HttpInterceptor,
  HttpRequest,
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {tap} from "rxjs/operators";
import {Router} from "@angular/router";
import {ClearAuth} from "../store/actions";
import {Store} from "@ngrx/store";
import {IAppState} from "../store/app.reducers";

@Injectable({
  providedIn: 'root',
})
export class JwtInterceptor implements HttpInterceptor {
  constructor(
    private router: Router,
    private store: Store<IAppState>,
  ) {}

  intercept(
    request: HttpRequest<any>,
    next: HttpHandler,
  ): Observable<HttpEvent<any>> {
    // add authorization header with jwt token if available
    if (
      !request.url.includes('Login') 
    ) {
      const currentUser = JSON.parse(sessionStorage.getItem('userData'));
      if (currentUser && currentUser.token) {
        request = request.clone({
          setHeaders: {
            Authorization: `Bearer ${currentUser.token}`,
          },
        });
      }
    }

    return next.handle(request).pipe(tap(() => {},
      (err: any) => {
        if(err instanceof HttpErrorResponse) {
          if(err.status === 401) {
            sessionStorage.setItem('userData', null);
            this.store.dispatch(new ClearAuth());
            this.router.navigate(['/login']);
          }
        }
      },
    ));
  }
}
