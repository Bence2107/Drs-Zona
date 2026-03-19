import {Component, Input, OnInit} from '@angular/core';
import {MatCard, MatCardContent, MatCardFooter} from '@angular/material/card';
import {ActivatedRoute, Router, RouterLink} from '@angular/router';

import {MatButton, MatFabButton} from '@angular/material/button';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ErrorDisplayComponent} from '../../components/error-display/error-display.component';
import {ArticleDetailDto} from '../../api/models/article-detail-dto';
import {ArticleService} from '../../services/article.service';
import {MatDivider} from '@angular/material/list';
import {CommentListComponent} from '../../components/lists/comment-list/comment-list.component';
import {AuthService} from '../../services/auth.service';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatDialog} from '@angular/material/dialog';
import {ConfirmDialogComponent} from '../../components/dialogs/confirmdialog/confirm-dialog.component';

@Component({
  selector: 'app-article',
  imports: [
    MatCard,
    RouterLink,
    MatButton,
    MatCardContent,
    MatCardFooter,
    ErrorDisplayComponent,
    MatDivider,
    CommentListComponent,
    MatProgressBar,
    MatFabButton,
    MatIcon,
    MatTooltip,
    MatMenuTrigger,
    MatMenu,
    MatMenuItem
  ],
  templateUrl: './article.component.html',
  styleUrl: './article.component.scss'
})
export class ArticleComponent implements OnInit {
  @Input() article: ArticleDetailDto | any;
  isLoading = false;
  errorOccurred = false;

  constructor(
    private articleService: ArticleService,
    private route: ActivatedRoute,
    private router: Router,
    private authService: AuthService,
    private dialog: MatDialog
  )
  {}

  ngOnInit() {
    this.fetchArticle();
  }


  get userId(): string | null {
    return this.authService.currentProfile()?.userId ?? null;
  }

  get avatarUrl(): string | null {
    const url = this.article.authorImageUrl;
    if (url == null) return "img/user/avatars/avatar.jpg";
    return `${url}`;
  }

  private addCacheBuster(url: string | null | undefined): string | null {
    if (!url) return null;
    return `${url}?cache=${new Date().getTime()}`;
  }

  protected isTheAuthorOrAdmin(): boolean {
    const role = this.authService.currentProfile()?.role;
    if (role === "Admin") {
      return true;
    }

    return this.article?.authorId == this.userId;
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
          this.article = {
            ...data,
            primaryImageUrl: this.addCacheBuster(data.primaryImageUrl),
            secondaryImageUrl: this.addCacheBuster(data.secondaryImageUrl),
            thirdImageUrl: this.addCacheBuster(data.thirdImageUrl),
            lastImageUrl: this.addCacheBuster(data.lastImageUrl)
          };
          this.isLoading = false;
        },
        error: () => {
          console.error();
          this.isLoading = false;
          this.errorOccurred = true;
        }
      });
    }
  }

  deleteArticle() {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      data: {
        title: 'Cikk törlése',
        message: 'Biztosan törölni szeretnéd ezt a cikket? A művelet nem vonható vissza.'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.articleService.delete(this.article.id!).subscribe(() => {
          this.router.navigate(['/news']);
        });
      }
    });
  }
}
