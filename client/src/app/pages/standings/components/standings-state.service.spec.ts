import { TestBed } from '@angular/core/testing';

import { StandingsStateService } from './standings-state.service';

describe('StandingsStateService', () => {
  let service: StandingsStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StandingsStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
