import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConstructorCreateDialogComponent } from './constructor-create-dialog.component';

describe('ConstructorCreateDialogComponent', () => {
  let component: ConstructorCreateDialogComponent;
  let fixture: ComponentFixture<ConstructorCreateDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConstructorCreateDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ConstructorCreateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
