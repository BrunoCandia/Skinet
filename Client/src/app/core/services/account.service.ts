import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { User } from '../../shared/models/user';
import { map, tap } from 'rxjs';
import { SignalrService } from './signalr.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  baseUrl = environment.apiUrl;
  currentUser = signal<User | null>(null);

  constructor(private httpClient: HttpClient, private signalrService: SignalrService) { }

  login(values: any) {
    let params = new HttpParams();
    params = params.append('useCookies', true);

    return this.httpClient.post<User>(this.baseUrl + 'login', values, {params})
                .pipe(tap(() => this.signalrService.createHubConnection()));
  }

  logout() {
    return this.httpClient.post(this.baseUrl + 'account/logout', {})
                .pipe(tap(() => this.signalrService.stopHubConnection()));
  }

  // login(values: any) {
  //   let params = new HttpParams();
  //   params = params.append('useCookies', true);

  //   return this.httpClient.post<User>(this.baseUrl + 'login', values, {params, withCredentials: true});
  // }

  // logout() {
  //   return this.httpClient.post(this.baseUrl + 'account/logout', {}, {withCredentials: true});
  // }

  register(values: any) {
    return this.httpClient.post(this.baseUrl + 'account/register', values);
  }

  getUserInfo() {
    return this.httpClient.get<User>(this.baseUrl + 'account/user-info')
      .pipe(
          map((user) => {
            if (user) {              
              this.currentUser.set(user);
            }
            return user;
          })
        );
  }

  // getUserInfo() {
  //   return this.httpClient.get<User>(this.baseUrl + 'account/user-info', {withCredentials: true})
  //     .pipe(
  //         map((user) => {
  //           if (user) {
  //             this.currentUser.set(user);
  //           }
  //           return user;
  //         })
  //       );
  // }

  updateAddress(address: any) {
    return this.httpClient.post(this.baseUrl + 'account/address', address)
      .pipe(
        tap(() => {
          this.currentUser.update(user => {
            if (user) {
              user.address = address;              
            }

            return user;
          });
        })
      );
  }

  getAuthState() {
    return this.httpClient.get<{isAuthenticated: boolean}>(this.baseUrl + 'account/auth-status');
  }
}
