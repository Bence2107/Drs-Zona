import { TestBed } from '@angular/core/testing';

import { StandingsManageService } from './standings-manage.service';

describe('StandingsManageService', () => {
  let service: StandingsManageService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(StandingsManageService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
