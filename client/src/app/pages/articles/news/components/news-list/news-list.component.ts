import {Component, Input, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardImage} from '@angular/material/card';
import { DatePipe } from '@angular/common';
import {ErrorDisplayComponent} from '../../../../../components/error-display/error-display.component';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ArticleListDto} from '../../../../../api/models/article-list-dto';
import {ArticleService} from '../../../../../services/article.service';
import {RouterLink} from '@angular/router';
import {MatFabButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {AuthService} from '../../../../../services/auth.service';

@Component({
  selector: 'app-news-list',
  imports: [
    MatCard,
    MatCardImage,
    MatCardContent,
    ErrorDisplayComponent,
    MatProgressBar,
    DatePipe,
    RouterLink,
    MatFabButton,
    MatIcon,
    MatTooltip
  ],
  templateUrl: './news-list.component.html',
  styleUrl: './news-list.component.scss'
})
export class NewsListComponent implements OnInit {
  @Input() articles: ArticleListDto[] = []
  isLoading = false;
  errorOccurred = false;

  constructor(private articleService: ArticleService, private authService: AuthService) {}

  ngOnInit() {
    this.fetchArticles();
  }

  fetchArticles() {
    this.isLoading = true;
    this.errorOccurred = false;

    this.articleService.getAllArticles().subscribe({
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

  protected isAuthorOrAdmin(): boolean {
    const role = this.authService.currentProfile()?.role;
    return role === 'Author' || role === 'Admin';
  }
}
