import {Injectable, signal} from '@angular/core';
import {AuthResponse} from '../api/models/auth-response';
import {HttpClient} from '@angular/common/http';
import {Router} from '@angular/router';
import {LoginRequest} from '../api/models/login-request';
import {Observable, tap} from 'rxjs';
import {
  apiAuthChangePasswordPost,
  apiAuthLoginPost$Json,
  apiAuthLogoutPost,
  apiAuthMeGet$Json, apiAuthProfilePictureDeletePost, apiAuthProfilePictureUpdatePost,
  ApiAuthProfilePictureUpdatePost$Params, apiAuthProfileUpdatePost,
  apiAuthRegisterPost$Json
} from '../api/functions';
import {map} from 'rxjs/operators';
import {ApiConfiguration} from '../api/api-configuration';
import {RegisterRequest} from '../api/models/register-request';
import {UserProfileResponse} from '../api/models/user-profile-response';
import {ChangePasswordRequest} from '../api/models/change-password-request';
import {UpdateUserRequest} from '../api/models/update-user-request';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly TOKEN_KEY = 'auth_token';

  currentUser = signal<AuthResponse | null>(this.loadFromStorage());
  currentProfile = signal<UserProfileResponse | null>(null);

  constructor(private http: HttpClient, private router: Router, private apiConfig: ApiConfiguration) {}

  register(request: RegisterRequest): Observable<AuthResponse> {
    return apiAuthRegisterPost$Json(this.http, this.apiConfig.rootUrl, {body: request}).pipe(
      map(response => response.body),
      tap(response => {
        localStorage.setItem(this.TOKEN_KEY, JSON.stringify(response));
        this.currentUser.set(response);
      })
    )
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return apiAuthLoginPost$Json(this.http, this.apiConfig.rootUrl, { body: request }).pipe(
      map(response => response.body),
      tap(response => {
        localStorage.setItem(this.TOKEN_KEY, JSON.stringify(response));
        this.currentUser.set(response);
        this.loadProfile();
      })
    );
  }

  loadProfile(): void {
    if (!this.isLoggedIn()) return;
    apiAuthMeGet$Json(this.http, this.apiConfig.rootUrl).subscribe({
      next: (response) => this.currentProfile.set(response.body),
      error: () => this.currentProfile.set(null)
    });
  }

  getMe(): Observable<UserProfileResponse> | null {
    if(!this.isLoggedIn()) return null;
    return apiAuthMeGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => response.body as UserProfileResponse),
    )
  }

  updateProfile(request: UpdateUserRequest): Observable<void> {
    return apiAuthProfileUpdatePost(this.http, this.apiConfig.rootUrl, { body: request }).pipe(
      map(() => void 0),
      tap(() => this.loadProfile())
    );
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return apiAuthChangePasswordPost(this.http, this.apiConfig.rootUrl, { body: request }).pipe(
      map(() => void 0)
    );
  }

  updateProfilePicture(file: File): Observable<void> {
    const params: ApiAuthProfilePictureUpdatePost$Params = {
      body: {
        File: file
      }
    };

    return apiAuthProfilePictureUpdatePost(this.http, this.apiConfig.rootUrl, params).pipe(
      map(() => void 0),
      tap(() => this.loadProfile())
    );
  }

  deleteProfilePicture(): Observable<void> {
    return apiAuthProfilePictureDeletePost(this.http, this.apiConfig.rootUrl).pipe(
      map(() => void 0),
      tap(() => this.loadProfile())
    );
  }


  logout(): void {
    apiAuthLogoutPost(this.http, this.apiConfig.rootUrl).subscribe({
      complete: () => this.clearSession()
    });
  }


  getToken(): string | null {
    return this.currentUser()?.token ?? null;
  }

  isLoggedIn(): boolean {
    const user = this.currentUser();
    if (!user?.token || !user?.expiresAt) return false;
    return new Date(user.expiresAt) > new Date();
  }


  private loadFromStorage(): AuthResponse | null {
    const stored = localStorage.getItem(this.TOKEN_KEY);
    return stored ? JSON.parse(stored) : null;
  }

  private clearSession(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    this.currentUser.set(null);
    this.currentProfile.set(null);
    this.router.navigate(['/home']);
  }
}
