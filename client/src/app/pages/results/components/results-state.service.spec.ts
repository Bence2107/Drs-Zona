import { TestBed } from '@angular/core/testing';

import { ResultsStateService } from './results-state.service';

describe('ResultsStateService', () => {
  let service: ResultsStateService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ResultsStateService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
