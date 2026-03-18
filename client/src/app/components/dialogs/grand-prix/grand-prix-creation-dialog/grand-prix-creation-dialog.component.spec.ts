import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GrandPrixManageDialogComponent } from './grand-prix-creation-dialog.component';

describe('GrandPrixCreationDialogComponent', () => {
  let component: GrandPrixManageDialogComponent;
  let fixture: ComponentFixture<GrandPrixManageDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GrandPrixManageDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GrandPrixManageDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
