import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParticipationAddDialogComponent } from './participation-add-dialog.component';

describe('ParticipationAddDialogComponent', () => {
  let component: ParticipationAddDialogComponent;
  let fixture: ComponentFixture<ParticipationAddDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ParticipationAddDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ParticipationAddDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
