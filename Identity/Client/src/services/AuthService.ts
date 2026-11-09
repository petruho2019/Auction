import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { User } from '../models/user/User';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  private readonly authUrl = 'http://localhost:5000/api/v1/identity';

  public getCurrentUser(): Observable<User> {
    const user$ = this.http
      .get<User>(`${this.authUrl}/auth/current`, { withCredentials: true })
      .pipe(
        map((user) => {
          return {
            ...user,
            isAuthenticated: !!user.id,
          };
        }),
      );
    return user$;
  }

  public login(email: string, password: string): Observable<void> {
    return this.http.post<void>(
      `${this.authUrl}/auth/login`,
      { email, password },
      { withCredentials: true },
    );
  }

  public register(email: string, username: string, password: string): Observable<void> {
    return this.http.post<void>(
      `${this.authUrl}/auth/register`,
      { email, username, password },
      { withCredentials: true },
    );
  }

  public logout(): Observable<void> {
    return this.http.post<void>(`${this.authUrl}/auth/logout`, {}, { withCredentials: true });
  }
}
