import { TestBed } from '@angular/core/testing';

import { GrandsPrixService } from './grands-prix.service';

describe('GrandsPrixService', () => {
  let service: GrandsPrixService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(GrandsPrixService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
