import { ComponentFixture, fakeAsync, flushMicrotasks, TestBed } from '@angular/core/testing';
import { ActivatedRoute, Params, provideRouter } from '@angular/router';
import { BehaviorSubject, of, Subject } from 'rxjs';

import { SerieComponent } from './serie.component';
import { SeriesService } from '../../services/api/series.service';
import { ArticleService } from '../../services/api/article.service';
import { PollService } from '../../services/api/poll.service';
import { ImagePreloadService } from '../../services/image-preload.service';
import { SeriesDetailDto } from '../../api/models/series-detail-dto';

describe('CategoryComponent', () => {
  let component: SerieComponent;
  let fixture: ComponentFixture<SerieComponent>;
  let params$: BehaviorSubject<Params>;
  let detailResponses: Record<string, Subject<SeriesDetailDto>>;

  const respond = (seriesName: string, detail: SeriesDetailDto) => {
    detailResponses[seriesName].next(detail);
    detailResponses[seriesName].complete();
  };

  beforeEach(async () => {
    params$ = new BehaviorSubject<Params>({ name: 'f1' });
    detailResponses = {};

    await TestBed.configureTestingModule({
      imports: [SerieComponent],
      providers: [
        provideRouter([]),
        { provide: ActivatedRoute, useValue: { params: params$ } },
        {
          provide: SeriesService,
          useValue: {
            getByName: (name: string) => (detailResponses[name] = new Subject<SeriesDetailDto>())
          }
        },
        { provide: ArticleService, useValue: { getRecent: () => of([]) } },
        { provide: PollService, useValue: { getAllActive: () => of([]) } },
        { provide: ImagePreloadService, useValue: { preload: () => Promise.resolve() } },
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(SerieComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should ignore a late response of a previously selected series', fakeAsync(() => {
    params$.next({ name: 'f2' });

    respond('f2', { shortName: 'F2' });
    flushMicrotasks();
    respond('f1', { shortName: 'F1' });
    flushMicrotasks();

    expect(component.seriesDetail?.shortName).toBe('F2');
    expect(component.isLoading).toBeFalse();
    expect(component.hasLoaded).toBeTrue();
  }));
});
