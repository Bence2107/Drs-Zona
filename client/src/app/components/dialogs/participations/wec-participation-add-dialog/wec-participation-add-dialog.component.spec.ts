import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WecParticipationAddDialogComponent } from './wec-participation-add-dialog.component';

describe('WecParticipationAddDialogComponent', () => {
  let component: WecParticipationAddDialogComponent;
  let fixture: ComponentFixture<WecParticipationAddDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [WecParticipationAddDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(WecParticipationAddDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
