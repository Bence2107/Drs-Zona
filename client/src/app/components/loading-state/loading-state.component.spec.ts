import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { LoadingStateComponent } from './loading-state.component';

describe('LoadingStateComponent', () => {
  let component: LoadingStateComponent;
  let fixture: ComponentFixture<LoadingStateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoadingStateComponent]
    }).compileComponents();

    fixture = TestBed.createComponent(LoadingStateComponent);
    component = fixture.componentInstance;
  });

  it('should show a spinner by default', () => {
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('mat-progress-spinner'))).toBeTruthy();
    expect(fixture.debugElement.query(By.css('mat-progress-bar'))).toBeNull();
  });

  it('should show a progress bar for the bar variant', () => {
    fixture.componentRef.setInput('variant', 'bar');
    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('mat-progress-bar'))).toBeTruthy();
    expect(fixture.debugElement.query(By.css('mat-progress-spinner'))).toBeNull();
  });

  it('should render the label only when provided', () => {
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('h3'))).toBeNull();

    fixture.componentRef.setInput('label', 'Cikkek betöltése...');
    fixture.detectChanges();
    expect(fixture.debugElement.query(By.css('h3')).nativeElement.textContent).toContain('Cikkek betöltése...');
  });
});
