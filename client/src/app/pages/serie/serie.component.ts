import {Component, DestroyRef, inject, OnInit} from '@angular/core';
import {takeUntilDestroyed} from '@angular/core/rxjs-interop';
import {PollService} from '../../services/api/poll.service';
import {ArticleService} from '../../services/api/article.service';
import {SeriesService} from '../../services/api/series.service';
import {ActivatedRoute} from '@angular/router';
import {PollListDto} from '../../api/models/poll-list-dto';
import {ArticleListDto} from '../../api/models/article-list-dto';
import {SeriesDetailDto} from '../../api/models/series-detail-dto';
import {catchError, forkJoin, from, map, of, Subject, switchMap} from 'rxjs';
import {LoadingStateComponent} from '../../components/loading-state/loading-state.component';
import {ErrorDisplayComponent} from '../../components/error-display/error-display.component';
import {PollListComponent} from '../../components/lists/polls-list/polls-list.component';
import {RecentNewsComponent} from '../../components/lists/recent-news-list/recent-news.component';
import {MatCard, MatCardContent, MatCardImage} from '@angular/material/card';
import {ImagePreloadService} from '../../services/image-preload.service';

@Component({
  selector: 'app-serie',
  imports: [
    LoadingStateComponent,
    ErrorDisplayComponent,
    PollListComponent,
    RecentNewsComponent,
    MatCard,
    MatCardContent,
    MatCardImage
  ],
  templateUrl: './serie.component.html',
  styleUrl: './serie.component.scss',
})
export class SerieComponent implements OnInit{
  seriesName!: string;
  seriesDetail?: SeriesDetailDto;
  articles: ArticleListDto[] = [];
  polls: PollListDto[] = [];

  isLoading = true;
  hasLoaded = false;
  errorOccurred = false;

  private readonly destroyRef = inject(DestroyRef);
  // Every (re)load goes through this stream, switchMap drops responses of outdated requests
  private readonly load$ = new Subject<string>();

  constructor(
    private route: ActivatedRoute,
    private seriesService: SeriesService,
    private articleService: ArticleService,
    private pollService: PollService,
    private imagePreload: ImagePreloadService
  ) {}

  ngOnInit() {
    this.load$.pipe(
      switchMap(seriesName => this.loadSeriesData(seriesName)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(result => {
      this.isLoading = false;
      if (!result) {
        this.errorOccurred = true;
        return;
      }
      this.seriesDetail = result.detail;
      this.articles = result.news;
      this.polls = result.activePolls;
      this.hasLoaded = true;
    });

    this.route.params.pipe(
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(params => {
      this.seriesName = params['name'];
      this.seriesDetail = undefined;
      this.hasLoaded = false;
      this.fetchSeriesData();
    });
  }

  fetchSeriesData() {
    this.isLoading = true;
    this.errorOccurred = false;
    this.load$.next(this.seriesName);
  }

  get seriesImageUrl(): string | null {
    return this.imageUrlFor(this.seriesDetail);
  }

  private loadSeriesData(seriesName: string) {
    return forkJoin({
      detail: this.seriesService.getByName(seriesName),
      news: this.articleService.getRecent(3, seriesName),
      activePolls: this.pollService.getAllActive(seriesName)
    }).pipe(
      // Data is applied only after its images are ready, so the page renders in one step
      switchMap(result => from(this.imagePreload.preload([
        ...result.news.map(article => article.primaryImageUrl),
        this.imageUrlFor(result.detail)
      ])).pipe(map(() => result))),
      catchError(() => of(null))
    );
  }

  private imageUrlFor(detail?: SeriesDetailDto): string | null {
    const shortName = detail?.shortName;
    return shortName ? `img/series/${shortName.toLowerCase()}.jpg` : null;
  }
}
