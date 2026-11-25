import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

import { StandingsStateService } from './standings-state.service';

describe('StandingsStateService', () => {
  let service: StandingsStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [StandingsStateService, provideHttpClient(), provideHttpClientTesting()],
    });
    service = TestBed.inject(StandingsStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
