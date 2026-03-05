import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DriverCreateDialogComponent } from './driver-create-dialog.component';

describe('DriverCreateDialogComponent', () => {
  let component: DriverCreateDialogComponent;
  let fixture: ComponentFixture<DriverCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DriverCreateDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DriverCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
