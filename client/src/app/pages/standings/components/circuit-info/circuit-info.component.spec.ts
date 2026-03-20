import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CircuitInfoComponent } from './circuit-info.component';

describe('CircuitInfoComponent', () => {
  let component: CircuitInfoComponent;
  let fixture: ComponentFixture<CircuitInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CircuitInfoComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CircuitInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
