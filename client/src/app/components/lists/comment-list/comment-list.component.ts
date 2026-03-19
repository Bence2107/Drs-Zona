import {Component, Input, OnInit} from '@angular/core';
import { MatCard } from '@angular/material/card';
import {CommentService} from '../../../services/api/comment.service';
import {UIComment} from '../../../models/ui-comment';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CommentItemComponent} from './comment-item/comment-item.component';
import {MatIcon} from '@angular/material/icon';
import {AuthService} from '../../../services/api/auth.service';
import {CommentCreateDto} from '../../../api/models/comment-create-dto';
import {HttpValidationError} from '../../../services/error-interceptor.service';

@Component({
  selector: 'app-comment-list',
  imports: [
    MatCard,
    ReactiveFormsModule,
    CommentItemComponent,
    MatIcon,
    FormsModule
  ],
  templateUrl: './comment-list.component.html',
  styleUrl: './comment-list.component.scss'
})
export class CommentListComponent implements OnInit {
  @Input() articleId!: string;
  @Input() articleUrl!: string;
  newCommentText = '';
  commentError = '';
  isSubmitting = false;

  submitComment() {
    const userId = this.authService.currentProfile()?.userId;
    if (!this.newCommentText.trim() || !userId) return;

    const dto: CommentCreateDto = {
      articleId: this.articleId,
      content: this.newCommentText.trim(),
      replyToCommentId: undefined
    };

    this.isSubmitting = true;
    this.commentService.createComment(dto, userId).subscribe({
      next: () => {
        this.newCommentText = '';
        this.commentError = '';
        this.isSubmitting = false;
        this.loadMainComments();
      },
      error: (err: HttpValidationError) => {
        this.commentError = err?.fieldErrors?.['content']?.[0] ?? err?.title ?? 'Hiba történt a küldés során';
        this.isSubmitting = false;
      }
    });
  }

  autoResize(event: Event) {
    const el = event.target as HTMLTextAreaElement;
    el.style.height = 'auto';
    el.style.height = el.scrollHeight + 'px';
  }

  comments: UIComment[] = [];

  get avatarUrl(): string {
    const profile = this.authService.currentProfile();
    if (profile?.hasAvatar && profile?.avatarUrl) {
      return profile.avatarUrl;
    }
    return "img/user/avatars/avatar.jpg";
  }

  constructor(
    private commentService: CommentService,
    private authService: AuthService,
  ) {}

  ngOnInit() {
    this.loadMainComments();
  }

  loadMainComments() {
    this.commentService.getCommentsWithoutReplies(this.articleId).subscribe(comments => {
      this.comments = comments as UIComment[];
    });
  }
}
