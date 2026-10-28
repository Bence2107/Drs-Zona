import {ApplicationConfig, ErrorHandler, LOCALE_ID,} from '@angular/core';
import {provideRouter, withInMemoryScrolling} from '@angular/router';

import { routes } from './app.routes';
import {provideApiConfiguration} from './api/api-configuration';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import localeHu from '@angular/common/locales/hu';
import {registerLocaleData} from '@angular/common';
import {authInterceptor} from './interceptors/auth.interceptor';
import {provideNativeDateAdapter} from '@angular/material/core';
import {MatPaginatorIntl} from '@angular/material/paginator';
import {getHungarianPaginatorIntl} from './shared/getHungarianPaginator';
import {errorInterceptor} from './services/error-interceptor.service';

registerLocaleData(localeHu)

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),
    provideHttpClient(withInterceptors([errorInterceptor])),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideNativeDateAdapter(),
    provideRouter(routes),
    {
      provide: ErrorHandler,
    },
    provideRouter(routes, withInMemoryScrolling({
      scrollPositionRestoration: 'top',
      anchorScrolling: 'enabled'
    })),
    // Relative URLs: the dev proxy forwards them to the API on whatever port it runs, prod serves both from one origin
    provideApiConfiguration(''),
    { provide: LOCALE_ID, useValue: 'hu-HU' },
    { provide: MatPaginatorIntl, useValue: getHungarianPaginatorIntl() }
  ]
};
