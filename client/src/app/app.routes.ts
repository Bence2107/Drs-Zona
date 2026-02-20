import { Routes } from '@angular/router';
import {HomeComponent} from './pages/home/home.component';
import {NewsComponent} from './pages/articles/news/news.component';
import {ReviewsComponent} from './pages/articles/reviews/reviews.component';
import {ArticleComponent} from './pages/article/article.component';
import {AuthComponent} from './pages/auth/auth/auth.component';
import {ProfileComponent} from './pages/auth/profile/profile.component';
import {authGuard, guestGuard} from './guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: "home", component: HomeComponent },
  { path: "news", component: NewsComponent },
  { path: "reviews", component: ReviewsComponent },
  { path: "article/:slug", component: ArticleComponent },
  {
    path: "auth",
    component: AuthComponent,
    canActivate: [guestGuard]
  },
  {
    path: "profile/:username",
    component: ProfileComponent,
    canActivate: [authGuard]
  },
];
