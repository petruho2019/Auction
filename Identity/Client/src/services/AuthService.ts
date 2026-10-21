import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private http = inject(HttpClient);

  private readonly authUrl = 'http://localhost:5058/api/v1/identity';

  public getCurrentUser(): Observable<User> {
    const user$ = this.http.get<User>(`${this.authUrl}/auth/current`).pipe(
      map((user) => ({
        ...user,
        isAuthenticated: !!user.id,
      })),
    );
    return user$;
  }
}
