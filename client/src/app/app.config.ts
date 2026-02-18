import {ApplicationConfig, ErrorHandler, LOCALE_ID,} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {ApiConfiguration} from './api/api-configuration';
import {provideHttpClient, withInterceptors} from '@angular/common/http';
import localeHu from '@angular/common/locales/hu';
import {registerLocaleData} from '@angular/common';
import {authInterceptor} from './interceptors/auth';

registerLocaleData(localeHu)

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),
    provideHttpClient(withInterceptors([authInterceptor])),
    provideRouter(routes),
    {
      provide: ErrorHandler,
    },
    {
      provide: ApiConfiguration,
      useFactory: () => {
        const config = new ApiConfiguration();
        config.rootUrl = "https://localhost:7221";
        return config;
      }
    },
    { provide: LOCALE_ID, useValue: 'hu-HU' }
  ]
};
