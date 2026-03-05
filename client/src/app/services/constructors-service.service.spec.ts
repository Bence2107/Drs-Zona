import { TestBed } from '@angular/core/testing';

import { ConstructorsServiceService } from './constructors-service.service';

describe('ConstructorsServiceService', () => {
  let service: ConstructorsServiceService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ConstructorsServiceService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
