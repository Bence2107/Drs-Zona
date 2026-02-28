import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PollAddDialogComponent } from './poll-add-dialog.component';

describe('PollAddDialogComponent', () => {
  let component: PollAddDialogComponent;
  let fixture: ComponentFixture<PollAddDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PollAddDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PollAddDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
