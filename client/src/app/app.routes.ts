import { Routes } from '@angular/router';
import { HomeComponent } from './pages/home/home.component';
import { NewsComponent } from './pages/articles/news/news.component';
import { ReviewsComponent } from './pages/articles/reviews/reviews.component';
import { ArticleComponent } from './pages/article/article.component';
import { AuthComponent } from './pages/auth/auth/auth.component';
import { ProfileComponent } from './pages/auth/profile/profile.component';
import { authGuard, guestGuard } from './guards/auth.guard';
import { editorGuard } from './guards/editor.guard';
import { authorGuard } from './guards/author.guard';
import { StandingsComponent } from './pages/standings/standings.component';
import { EntryComponent } from './pages/admin/standings/entry/entry.component';
import { ChampionshipsComponent } from './pages/admin/standings/championships/championships.component';
import { ParticipationsComponent } from './pages/admin/standings/participations/participations.component';
import { DriversComponent } from './pages/admin/standings/drivers/drivers.component';
import { ConstructorsComponent } from './pages/admin/standings/constructors/constructors.component';
import { ContractsComponent } from './pages/admin/standings/contracts/contracts.component';
import { EntryDetailComponent } from './pages/admin/standings/entry/entry-detail/entry-detail.component';
import { EntryCreateComponent } from './pages/admin/standings/entry/entry-create/entry-create.component';
import { ArticleManageComponent } from './pages/admin/articles/article-manage/article-manage.component';
import { SerieComponent } from './pages/serie/serie.component';

export const routes: Routes = [
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: "home", component: HomeComponent },
  { path: "news", component: NewsComponent },
  { path: "reviews", component: ReviewsComponent },
  { path: "article/:slug", component: ArticleComponent },
  { path: "results", component: StandingsComponent },
  { path: "serie/:name", component: SerieComponent },

  {
    path: 'admin',
    canActivate: [editorGuard],
    children: [
      { path: 'championships', component: ChampionshipsComponent },
      { path: 'participations', component: ParticipationsComponent },
      { path: 'participations/:champId', component: ParticipationsComponent },
      { path: 'drivers', component: DriversComponent },
      { path: 'constructors', component: ConstructorsComponent },
      { path: 'contracts', component: ContractsComponent },
      {
        path: 'results/entry',
        children: [
          { path: '', component: EntryComponent },
          { path: ':gpId', component: EntryDetailComponent },
          { path: ':champId', component: EntryDetailComponent },
          { path: ':gpId/create', component: EntryCreateComponent },
        ]
      },
    ]
  },

  {
    path: 'admin/articles',
    canActivate: [authorGuard],
    children: [
      { path: 'create', component: ArticleManageComponent },
      { path: 'update/:slug', component: ArticleManageComponent },
    ]
  },

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

  { path: '**', redirectTo: '/home' }
];
