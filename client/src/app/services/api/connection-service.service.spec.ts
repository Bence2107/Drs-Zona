import { TestBed } from '@angular/core/testing';

import { ConnectionService } from './connection-service.service';
import {provideHttpClient} from '@angular/common/http';
import {provideHttpClientTesting} from '@angular/common/http/testing';
import {provideRouter} from '@angular/router';

describe('ConnectionServiceService', () => {
  let service: ConnectionService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([])
      ]
    });
    service = TestBed.inject(ConnectionService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});

