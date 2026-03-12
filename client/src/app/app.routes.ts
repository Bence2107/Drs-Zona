import { Routes } from '@angular/router';
import {HomeComponent} from './pages/home/home.component';
import {NewsComponent} from './pages/articles/news/news.component';
import {ReviewsComponent} from './pages/articles/reviews/reviews.component';
import {ArticleComponent} from './pages/article/article.component';
import {AuthComponent} from './pages/auth/auth/auth.component';
import {ProfileComponent} from './pages/auth/profile/profile.component';
import {authGuard, guestGuard} from './guards/auth.guard';
import {ResultsComponent} from './pages/results/results.component';
import {EntryComponent} from './pages/admin/results/entry/entry.component';
import {ChampionshipsComponent} from './pages/admin/results/championships/championships.component';
import {ParticipationsComponent} from './pages/admin/results/participations/participations.component';
import {DriversComponent} from './pages/admin/results/drivers/drivers.component';
import {ConstructorsComponent} from './pages/admin/results/constructors/constructors.component';
import {ContractsComponent} from './pages/admin/results/contracts/contracts.component';
import {EntryDetailComponent} from './pages/admin/results/entry/entry-detail/entry-detail.component';
import {EntryCreateComponent} from './pages/admin/results/entry/entry-create/entry-create.component';
import {ArticleManageComponent} from './pages/admin/articles/article-manage/article-manage.component';
import {SerieComponent} from './pages/serie/serie.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: "home", component: HomeComponent },
  { path: "news", component: NewsComponent },
  { path: "reviews", component: ReviewsComponent },
  { path: "article/:slug", component: ArticleComponent },
  { path: "results", component: ResultsComponent },
  { path: "serie/:name", component: SerieComponent},
  { path: "admin/results/entry", component:  EntryComponent },
  { path: "admin/championships", component:  ChampionshipsComponent },
  { path: "admin/participations", component:  ParticipationsComponent },
  { path: "admin/drivers", component:  DriversComponent },
  { path: "admin/constructors", component:  ConstructorsComponent },
  { path: "admin/contracts", component: ContractsComponent},
  { path: 'admin/results/entry/:gpId', component: EntryDetailComponent },
  { path: 'admin/results/entry/:gpId/create', component: EntryCreateComponent },
  { path: 'admin/articles/create', component: ArticleManageComponent },
  { path: 'admin/articles/update/:slug', component: ArticleManageComponent },
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
