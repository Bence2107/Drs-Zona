import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HeaderComponent } from './header.component';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

describe('HeaderComponent - US-05: Dark/Light Mode Toggle', () => {
  let component: HeaderComponent;
  let fixture: ComponentFixture<HeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HeaderComponent, BrowserAnimationsModule],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(HeaderComponent);
    component = fixture.componentInstance;

    localStorage.clear();
    fixture.detectChanges();
  });

  it('AC1: Should immediately change CSS class on body when toggle is switched', () => {
    expect(document.body.classList.contains('dark-mode')).toBeTrue();

    component.toggleTheme();
    fixture.detectChanges();

    expect(component.isDarkMode).toBeFalse();
    expect(document.body.classList.contains('light-mode')).toBeTrue();
    expect(document.body.classList.contains('dark-mode')).toBeFalse();
  });

  it('AC1: Clicking the UI element (mat-slide-toggle) triggers theme toggle', () => {
    spyOn(component, 'toggleTheme').and.callThrough();

    const toggle = fixture.debugElement.query(By.css('mat-slide-toggle'));

    toggle.triggerEventHandler('change', null);
    fixture.detectChanges();

    expect(component.toggleTheme).toHaveBeenCalled();
  });

  it('AC2: Selected theme should be saved to localStorage', () => {
    component.toggleTheme();
    fixture.detectChanges();

    const savedTheme = localStorage.getItem('theme');
    expect(savedTheme).toBe('light');
  });

  it('AC2: Should load saved theme from localStorage on initialization (ngOnInit)', () => {
    localStorage.setItem('theme', 'light');

    component.ngOnInit();
    fixture.detectChanges();

    expect(component.isDarkMode).toBeFalse();
    expect(document.body.classList.contains('light-mode')).toBeTrue();
  });
});
