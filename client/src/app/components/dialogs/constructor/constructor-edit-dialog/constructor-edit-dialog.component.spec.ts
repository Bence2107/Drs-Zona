import { ComponentFixture, TestBed } from '@angular/core/testing';

import ConstructorEditDialogComponent from './constructor-edit-dialog.component';

describe('ConstructorEditDialogComponent', () => {
  let component: ConstructorEditDialogComponent;
  let fixture: ComponentFixture<ConstructorEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConstructorEditDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConstructorEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
