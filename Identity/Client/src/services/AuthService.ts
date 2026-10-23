import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { User } from '../models/user/User';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  private readonly authUrl = '/api/v1/identity';

  public getCurrentUser(): Observable<User> {
    const user$ = this.http
      .get<User>(`${this.authUrl}/auth/current`, { withCredentials: true })
      .pipe(
        map((user) => ({
          ...user,
          isAuthenticated: !!user.id,
        })),
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
}
