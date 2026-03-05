import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PollVoteDialogComponent } from './poll-vote-dialog.component';

describe('PollVoteDialogComponent', () => {
  let component: PollVoteDialogComponent;
  let fixture: ComponentFixture<PollVoteDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PollVoteDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PollVoteDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
