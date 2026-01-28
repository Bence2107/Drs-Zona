import {Component, Input, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardImage} from '@angular/material/card';
import {DatePipe, NgForOf, NgIf} from '@angular/common';
import {ErrorDisplayComponent} from '../../../../components/error-display/error-display.component';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ArticleListDto} from '../../../../api/models/article-list-dto';
import {ArticleService} from '../../../../services/article.service';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-news-list',
  imports: [
    MatCard,
    MatCardImage,
    NgIf,
    NgForOf,
    MatCardContent,
    ErrorDisplayComponent,
    MatProgressBar,
    DatePipe,
    RouterLink
  ],
  templateUrl: './news-list.component.html',
  styleUrl: './news-list.component.scss'
})
export class NewsListComponent implements OnInit {
  @Input() articles: ArticleListDto[] = []
  isLoading = false;
  errorOccurred = false;

  constructor(private articleService: ArticleService) {}

  ngOnInit() {
    this.fetchArticles();
  }

  fetchArticles() {
    this.isLoading = true;
    this.errorOccurred = false;

    this.articleService.getAll().subscribe({
      next: (data) => {
        this.articles = data;
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
        this.errorOccurred = true;
      }
    });
  }
}
