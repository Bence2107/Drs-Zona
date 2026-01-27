import {Component, OnInit} from '@angular/core';
import {MatProgressBar} from '@angular/material/progress-bar';
import {NgForOf, NgIf} from '@angular/common';
import {RouterLink} from '@angular/router';
import {ArticleService} from '../../../../services/article.service';
import {MatCard, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';
import {MatButton} from '@angular/material/button';
import {MatList, MatListItem, MatListItemMeta, MatListItemTitle} from '@angular/material/list';
import {ArticleListDto} from '../../../../api/models/article-list-dto';
import {ErrorDisplayComponent} from '../../../../components/error-display/error-display.component';

@Component({
  selector: 'app-recent-news',
  imports: [
    MatProgressBar,
    NgIf,
    NgForOf,
    RouterLink,
    MatCard,
    MatCardContent,
    MatList,
    MatListItem,
    MatListItemTitle,
    MatListItemMeta,
    MatCardHeader,
    MatCardTitle,
    ErrorDisplayComponent
  ],
  templateUrl: './recent-news.component.html',
  styleUrl: './recent-news.component.scss'
})
export class RecentNewsComponent implements OnInit {
  articles$: ArticleListDto[] = [];
  isLoading = false;
  errorOccurred = false;

  constructor(private articleService: ArticleService) {}

  ngOnInit() {
    this.fetchArticles();
  }

  fetchArticles() {
    this.isLoading = true;
    this.errorOccurred = false;

    this.articleService.getRecent(3).subscribe({
      next: (data) => {
        this.articles$ = data;
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
