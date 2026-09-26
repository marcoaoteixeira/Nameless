import { inject, Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, of, tap } from 'rxjs';

interface OAuthToken {
  access_token: string;
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  readonly token = signal<string | null>(null);

  fetchToken() {
    return this.http.get<OAuthToken>('/connect/token').pipe(
      tap(res => this.token.set(res.access_token)),
      catchError(() => of(null))
    );
  }
}
