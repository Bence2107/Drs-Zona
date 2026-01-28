import { Routes } from '@angular/router';
import {HomeComponent} from './pages/home/home.component';
import {NewsComponent} from './pages/news/news.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: "home", component: HomeComponent },
  { path: "news", component: NewsComponent },
];
