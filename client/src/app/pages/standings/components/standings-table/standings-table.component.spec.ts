import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { StandingsStateService } from '../standings-state.service';

import { StandingsTableComponent } from './standings-table.component';

describe('ResultsTableComponent', () => {
  let component: StandingsTableComponent;
  let fixture: ComponentFixture<StandingsTableComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StandingsTableComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        StandingsStateService,
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StandingsTableComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
