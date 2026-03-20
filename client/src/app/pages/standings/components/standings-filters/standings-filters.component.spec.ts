import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StandingsFiltersComponent } from './standings-filters.component';

describe('ResultsFiltersComponent', () => {
  let component: StandingsFiltersComponent;
  let fixture: ComponentFixture<StandingsFiltersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StandingsFiltersComponent]
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
