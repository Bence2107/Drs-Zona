import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

import { ParticipationsComponent } from './participations.component';

describe('ParticipationsComponent', () => {
  let component: ParticipationsComponent;
  let fixture: ComponentFixture<ParticipationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParticipationsComponent],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([]),
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ParticipationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
