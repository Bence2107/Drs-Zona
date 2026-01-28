import {Component, Input, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardFooter} from '@angular/material/card';
import {MatIcon} from '@angular/material/icon';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';
import {NgForOf, NgIf} from '@angular/common';
import {MatButton} from '@angular/material/button';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ErrorDisplayComponent} from '../../components/error-display/error-display.component';
import {ArticleDetailDto} from '../../api/models/article-detail-dto';
import {ArticleService} from '../../services/article.service';
import {MatDivider} from '@angular/material/list';
import {CommentListComponent} from './components/comment-list/comment-list.component';

@Component({
  selector: 'app-article',
  imports: [
    MatCard,
    RouterLink,
    NgIf,
    MatButton,
    MatCardContent,
    MatCardFooter,
    ErrorDisplayComponent,
    NgForOf,
    MatDivider,
    CommentListComponent
  ],
  templateUrl: './article.component.html',
  styleUrl: './article.component.scss'
})
export class ArticleComponent implements OnInit {
  @Input() article: ArticleDetailDto | any;
  isLoading = false;
  errorOccurred = false;

  constructor(private articleService: ArticleService, private route: ActivatedRoute, private router: Router) {}

  ngOnInit() {
    this.fetchArticle();
  }

  fetchArticle() {
    const slug = this.route.snapshot.paramMap.get('slug');

    if(slug === null) {
       this.router.navigate(['/news']);
    }

    this.isLoading = true;
    this.errorOccurred = false;

    if (slug != null) {
      this.articleService.getBySlug(slug).subscribe({
        next: (data) => {
          this.article = data;
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
}
