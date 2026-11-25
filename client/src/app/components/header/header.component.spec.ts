import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HeaderComponent } from './header.component';
import { provideRouter } from '@angular/router';
import { By } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';

describe('HeaderComponent - US-05: Dark/Light Mode Toggle', () => {
    let component: HeaderComponent;
    let fixture: ComponentFixture<HeaderComponent>;

    const currentTheme = () => document.documentElement.getAttribute('data-theme');

    beforeEach(async () => {
        localStorage.clear();

        await TestBed.configureTestingModule({
            imports: [HeaderComponent, BrowserAnimationsModule],
            providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()]
        }).compileComponents();

        fixture = TestBed.createComponent(HeaderComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('AC1: Should immediately switch the theme attribute when toggle is switched', () => {
        expect(component.isDarkMode).toBe(false);
        expect(currentTheme()).toBe('light');

        component.toggleTheme();
        fixture.detectChanges();

        expect(component.isDarkMode).toBe(true);
        expect(currentTheme()).toBe('dark');
    });

    it('AC1: Clicking the UI element (theme toggle button) triggers theme toggle', () => {
        vi.spyOn(component, 'toggleTheme');

        const toggle = fixture.debugElement.query(By.css('.theme-toggle'));

        toggle.triggerEventHandler('click', null);
        fixture.detectChanges();

        expect(component.toggleTheme).toHaveBeenCalled();
    });

    it('AC2: Selected theme should be saved to localStorage', () => {
        component.toggleTheme();
        fixture.detectChanges();

        expect(localStorage.getItem('theme')).toBe('dark');
    });

    it('AC2: Should load saved theme from localStorage on initialization (ngOnInit)', () => {
        localStorage.setItem('theme', 'dark');

        component.ngOnInit();
        fixture.detectChanges();

        expect(component.isDarkMode).toBe(true);
        expect(currentTheme()).toBe('dark');
    });
});
