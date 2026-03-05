import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverEditDialogComponent } from './driver-edit-dialog.component';

describe('DriverEditDialogComponent', () => {
  let component: DriverEditDialogComponent;
  let fixture: ComponentFixture<DriverEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DriverEditDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DriverEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
