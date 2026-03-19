import {Component, Input, OnInit} from '@angular/core';
import {DatePipe, NgClass} from "@angular/common";
import {ErrorDisplayComponent} from "../../../../../components/error-display/error-display.component";
import {MatCard, MatCardContent, MatCardImage} from "@angular/material/card";
import {MatProgressBar} from "@angular/material/progress-bar";
import {RouterLink} from '@angular/router';
import {ArticleListDto} from '../../../../../api/models/article-list-dto';
import {ArticleService} from '../../../../../services/article.service';
import {MatFabButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {AuthService} from '../../../../../services/auth.service';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';
import {FormsModule} from '@angular/forms';
import {SeriesListDto} from '../../../../../api/models/series-list-dto';
import {SeriesService} from '../../../../../services/series.service';

@Component({
  selector: 'app-reviews-list',
  imports: [
    DatePipe,
    ErrorDisplayComponent,
    MatCard,
    MatCardContent,
    MatCardImage,
    MatProgressBar,
    RouterLink,
    MatFabButton,
    MatIcon,
    MatTooltip,
    MatPaginator,
    NgClass,
    MatFormField,
    MatLabel,
    MatOption,
    MatSelect,
    FormsModule
  ],
  templateUrl: './reviews-list.component.html',
  styleUrl: './reviews-list.component.scss'
})
export class ReviewsListComponent implements OnInit {
  @Input() reviews: ArticleListDto[] = []
  isLoading = false;
  errorOccurred = false;

  seriesList: SeriesListDto[] = []
  selectedTag: string | undefined = undefined;
  totalElements = 0;
  pageSize = 2;
  pageIndex = 0;

  constructor(
    private articleService: ArticleService,
    private authService: AuthService,
    private seriesService: SeriesService
  ) {}

  ngOnInit() {
    this.fetchArticles();

    this.seriesService.getSeriesList().subscribe(seriesList => {
      this.seriesList = seriesList;
    })
  }

  onTagChange(tag: string | undefined): void {
    this.selectedTag = tag;
    this.pageIndex = 0;
    this.fetchArticles();
  }

  onPageChange(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;
    this.fetchArticles();
  }

  fetchArticles() {
    this.isLoading = true;
    this.errorOccurred = false;

    this.articleService.getAllSummary(this.pageIndex, this.pageSize, this.selectedTag).subscribe({
      next: (data) => {
        this.reviews = data.items!;
        this.totalElements = data.totalCount!;
        this.isLoading = false;
      },
      error: () => {
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
