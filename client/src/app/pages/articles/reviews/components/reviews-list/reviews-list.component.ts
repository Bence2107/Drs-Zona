import {Component, Input, OnInit} from '@angular/core';
import { DatePipe } from "@angular/common";
import {ErrorDisplayComponent} from "../../../../../components/error-display/error-display.component";
import {MatCard, MatCardContent, MatCardImage} from "@angular/material/card";
import {MatProgressBar} from "@angular/material/progress-bar";
import {RouterLink} from '@angular/router';
import {ArticleListDto} from '../../../../../api/models/article-list-dto';
import {ArticleService} from '../../../../../services/article.service';

@Component({
  selector: 'app-reviews-list',
  imports: [
    DatePipe,
    ErrorDisplayComponent,
    MatCard,
    MatCardContent,
    MatCardImage,
    MatProgressBar,
    RouterLink
],
  templateUrl: './reviews-list.component.html',
  styleUrl: './reviews-list.component.scss'
})
export class ReviewsListComponent implements OnInit {
  @Input() reviews: ArticleListDto[] = []
  isLoading = false;
  errorOccurred = false;

  constructor(private articleService: ArticleService) {}

  ngOnInit() {
    this.fetchArticles();
  }

  fetchArticles() {
    this.isLoading = true;
    this.errorOccurred = false;

    this.articleService.getAllSummary().subscribe({
      next: (data) => {
        this.reviews = data;
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
