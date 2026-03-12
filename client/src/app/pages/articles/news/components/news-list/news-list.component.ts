import {Component, Input, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardImage} from '@angular/material/card';
import {DatePipe, NgClass} from '@angular/common';
import {ErrorDisplayComponent} from '../../../../../components/error-display/error-display.component';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ArticleListDto} from '../../../../../api/models/article-list-dto';
import {ArticleService} from '../../../../../services/article.service';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {MatFabButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {AuthService} from '../../../../../services/auth.service';
import {MatPaginator, PageEvent} from '@angular/material/paginator';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {FormsModule} from '@angular/forms';
import {SeriesListDto} from '../../../../../api/models/series-list-dto';
import {SeriesService} from '../../../../../services/series.service';

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
    MatTooltip,
    MatPaginator,
    NgClass,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    FormsModule
  ],
  templateUrl: './news-list.component.html',
  styleUrl: './news-list.component.scss'
})
export class NewsListComponent implements OnInit {
  @Input() articles: ArticleListDto[] = []
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
    private seriesService: SeriesService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe(params => {
      this.pageIndex = params['page'] ? +params['page'] : 0;
      this.pageSize = params['size'] ? +params['size'] : 2;
      this.fetchArticles();
    });

    this.seriesService.getSeriesList().subscribe(seriesList => {
      this.seriesList = seriesList;
    })
  }

  onTagChange(tag: string | undefined): void {
    this.selectedTag = tag;
    this.pageIndex = 0;
    this.fetchArticles();
  }

  fetchArticles() {
    this.isLoading = true;
    this.errorOccurred = false;

    this.articleService.getAllArticles(this.pageIndex, this.pageSize, this.selectedTag).subscribe({
      next: (data) => {
        this.articles = data.items!;
        this.totalElements = data.totalCount!;
        this.isLoading = false;
      },
      error: () => {
        this.isLoading = false;
        this.errorOccurred = true;
      }
    });
  }

  onPageChange(event: PageEvent) {
    this.pageIndex = event.pageIndex;
    this.pageSize = event.pageSize;

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: { page: this.pageIndex, size: this.pageSize },
      queryParamsHandling: 'merge'
    });
  }

  protected isAuthorOrAdmin(): boolean {
    const role = this.authService.currentProfile()?.role;
    return role === 'Author' || role === 'Admin';
  }
}
