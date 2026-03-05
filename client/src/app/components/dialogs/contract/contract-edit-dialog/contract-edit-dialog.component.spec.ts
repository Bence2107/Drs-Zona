import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractEditDialogComponent } from './contract-edit-dialog.component';

describe('ContractEditDialogComponent', () => {
  let component: ContractEditDialogComponent;
  let fixture: ComponentFixture<ContractEditDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContractEditDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContractEditDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
