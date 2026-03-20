import {Component, OnInit} from '@angular/core';
import {PollService} from '../../services/api/poll.service';
import {ArticleService} from '../../services/api/article.service';
import {SeriesService} from '../../services/api/series.service';
import {ActivatedRoute} from '@angular/router';
import {PollListDto} from '../../api/models/poll-list-dto';
import {ArticleListDto} from '../../api/models/article-list-dto';
import {SeriesDetailDto} from '../../api/models/series-detail-dto';
import {forkJoin} from 'rxjs';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ErrorDisplayComponent} from '../../components/error-display/error-display.component';
import {PollListComponent} from '../../components/lists/polls-list/polls-list.component';
import {RecentNewsComponent} from '../../components/lists/recent-news-list/recent-news.component';
import {MatCard, MatCardContent, MatCardImage} from '@angular/material/card';

@Component({
  selector: 'app-serie',
  imports: [
    MatProgressBar,
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
  errorOccurred = false;

  constructor(
    private route: ActivatedRoute,
    private seriesService: SeriesService,
    private articleService: ArticleService,
    private pollService: PollService
  ) {}

  ngOnInit() {
    this.route.params.subscribe(params => {
      this.seriesName = params['name'];
      this.fetchSeriesData();
    });
  }

  fetchSeriesData() {
    this.isLoading = true;
    this.errorOccurred = false;

    forkJoin({
      detail: this.seriesService.getByName(this.seriesName),
      news: this.articleService.getRecent(3, this.seriesName),
      activePolls: this.pollService.getAllActive(this.seriesName)
    }).subscribe({
      next: (result) => {
        this.seriesDetail = result.detail;
        this.articles = result.news;
        this.polls = result.activePolls;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorOccurred = true;
      }
    });
  }
}
