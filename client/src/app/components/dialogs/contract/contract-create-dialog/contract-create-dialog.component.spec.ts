import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractCreateDialogComponent } from './contract-create-dialog.component';

describe('ContractCreateDialogComponent', () => {
  let component: ContractCreateDialogComponent;
  let fixture: ComponentFixture<ContractCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ContractCreateDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ContractCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
