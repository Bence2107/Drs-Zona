import {Component, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardHeader, MatCardImage, MatCardTitle} from '@angular/material/card';
import {RecentNewsComponent} from './components/recent-news/recent-news.component';
import {PollListComponent} from '../../components/lists/polls-list/polls-list.component';
import {ArticleService} from '../../services/article.service';
import {PollService} from '../../services/poll.service';
import {forkJoin} from 'rxjs';
import {ArticleListDto} from '../../api/models/article-list-dto';
import {PollListDto} from '../../api/models/poll-list-dto';
import {ErrorDisplayComponent} from '../../components/error-display/error-display.component';
import {MatProgressBar} from '@angular/material/progress-bar';

@Component({
  selector: 'app-home',
  imports: [
    MatCard,
    MatCardHeader,
    MatCardImage,
    MatCardTitle,
    MatCardContent,
    RecentNewsComponent,
    PollListComponent,
    ErrorDisplayComponent,
    MatProgressBar,
  ],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  articles: ArticleListDto[] = [];
  polls: PollListDto[] = [];
  isLoading = true;
  errorOccurred = false;

  constructor(
    private articleService: ArticleService,
    private pollService: PollService
  ) {}

  ngOnInit() {
    this.fetchAllData();
  }

  fetchAllData() {
    this.isLoading = true;
    this.errorOccurred = false;

    forkJoin({
      news: this.articleService.getRecent(3),
      activePolls: this.pollService.getAllActive()
    }).subscribe({
      next: (result) => {
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
