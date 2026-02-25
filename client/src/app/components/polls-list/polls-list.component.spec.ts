import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PollsListComponent } from './polls-list.component';

describe('PollsListComponent', () => {
  let component: PollsListComponent;
  let fixture: ComponentFixture<PollsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PollsListComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PollsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
