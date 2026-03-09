import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import endpoints from './endpoints';
import {Observable, of, Subject} from 'rxjs';

@Injectable()
export class AuthService {
  _userActionOccured: Subject<void> = new Subject<void>();

  constructor(private http: HttpClient) {}

  login(data) {
    return this.http.post<any>(
      `${endpoints.AUTH.LOGIN}`, data
    );
  }

  getSession() {
    return of(JSON.parse(sessionStorage.getItem('userData')));
  }

  notifyUserAction() {
    this._userActionOccured.next();
  }

  get userActionOccured(): Observable<void> {
    return this._userActionOccured.asObservable();
  }
}
