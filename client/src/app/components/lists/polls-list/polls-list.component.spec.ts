import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PollListComponent } from './polls-list.component';

describe('PollsListComponent', () => {
  let component: PollListComponent;
  let fixture: ComponentFixture<PollListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PollListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PollListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
