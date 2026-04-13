import { ComponentFixture, TestBed } from '@angular/core/testing';

import { GrandPixManageDialogComponent } from './grand-prix-manage-dialog.component';

describe('GrandPrixManageDialogComponent', () => {
  let component: GrandPixManageDialogComponent;
  let fixture: ComponentFixture<GrandPixManageDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [GrandPixManageDialogComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(GrandPixManageDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
