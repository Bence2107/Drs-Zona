import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { StandingsStateService } from '../standings-state.service';

import { StandingsFiltersComponent } from './standings-filters.component';

describe('ResultsFiltersComponent', () => {
  let component: StandingsFiltersComponent;
  let fixture: ComponentFixture<StandingsFiltersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StandingsFiltersComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        StandingsStateService,
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StandingsFiltersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
