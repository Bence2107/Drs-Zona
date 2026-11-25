import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NewsListComponent } from './news-list.component';
import { ArticleService } from '../../../../../services/api/article.service';
import { of, throwError } from 'rxjs';
import { By } from '@angular/platform-browser';
import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { provideRouter } from '@angular/router';

describe('NewsListComponent', () => {
    let component: NewsListComponent;
    let fixture: ComponentFixture<NewsListComponent>;
    let mockArticleService: any;

    beforeEach(async () => {
        mockArticleService = {
            getAllArticles: vi.fn().mockReturnValue(of({ items: [], totalCount: 0 }))
        };

        await TestBed.configureTestingModule({
            imports: [NewsListComponent],
            providers: [
                { provide: ArticleService, useValue: mockArticleService },
                provideHttpClient(),
                provideHttpClientTesting(),
                provideRouter([])
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(NewsListComponent);
        component = fixture.componentInstance;

        fixture.detectChanges();
    });

    it('US-01 AC1: Should show progress bar when loading', () => {
        component.hasLoaded = false;
        component.isLoading = true;

        fixture.detectChanges();

        const progressBar = fixture.debugElement.query(By.css('mat-progress-bar'));
        expect(progressBar, 'Progress bar should be visible during loading').toBeTruthy();
    });

    it('US-01 AC2: Should show error component when server fails', () => {
        mockArticleService.getAllArticles.mockReturnValue(throwError(() => new Error('Server error')));

        component.fetchArticles();
        fixture.detectChanges();

        const errorDisplay = fixture.debugElement.query(By.css('app-error-display'));
        expect(errorDisplay, 'Error display component should appear').toBeTruthy();
        expect(component.errorOccurred).toBe(true);
    });

    it('US-02 AC1: Should display cards if articles are loaded', async () => {
        const mockData = [{ id: '1', title: 'News 1', lead: 'Lead', datePublished: new Date() }];
        mockArticleService.getAllArticles.mockReturnValue(of({ items: mockData, totalCount: 1 }));

        component.fetchArticles();
        // Articles render only after their images are preloaded
        await fixture.whenStable();
        fixture.detectChanges();

        const cards = fixture.debugElement.queryAll(By.css('.new-item'));
        expect(cards.length).toBe(1);
        expect(cards[0].nativeElement.textContent).toContain('News 1');
    });
});
