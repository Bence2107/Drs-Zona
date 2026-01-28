import {ApplicationConfig, ErrorHandler, LOCALE_ID,} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import {ApiConfiguration} from './api/api-configuration';
import {provideHttpClient} from '@angular/common/http';
import localeHu from '@angular/common/locales/hu';
import {registerLocaleData} from '@angular/common';

registerLocaleData(localeHu)

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(),
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
