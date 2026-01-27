import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class ConnectionService {
  private apiUrl = 'https://localhost:7221/api/healthcheck/ping';

  constructor(private http: HttpClient) {}

  checkConnection(): Observable<any> {
    return this.http.get(this.apiUrl).pipe(
      catchError(err => throwError(() => new Error('Server unreachable')))
    );
  }
}
