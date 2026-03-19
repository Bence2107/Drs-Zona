import {Component, EventEmitter, Input, Output} from '@angular/core';
import {UIComment} from '../../../../models/ui-comment';
import {CommentService} from '../../../../services/api/comment.service';
import { DatePipe } from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {FormsModule} from '@angular/forms';
import {AuthService} from '../../../../services/api/auth.service';
import {CommentCreateDto} from '../../../../api/models/comment-create-dto';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {CommentContentUpdateDto} from '../../../../api/models/comment-content-update-dto';
import {CommentUpdateVoteDto} from '../../../../api/models/comment-update-vote-dto';
import {RouterLink} from '@angular/router';
import {HttpValidationError} from '../../../../services/error-interceptor.service';

@Component({
  selector: 'app-comment-item',
  imports: [
    DatePipe,
    MatIconButton,
    MatIcon,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
    FormsModule,
    MatMenu,
    MatMenuItem,
    MatMenuTrigger,
    RouterLink
  ],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input() comment!: UIComment;
  @Input() articleId: string | null = null;

  @Input() isReply: boolean = false;
  @Input() isOnProfileSite: boolean = false;

  @Input() depth: number = 0;

  @Output() commentDeleted = new EventEmitter<void>();

  repliesVisible = false;

  isEditing = false;

  editText = '';
  replyFormVisible = false;
  replyText = '';
  editError = '';
  replyError = '';

  constructor(private commentService: CommentService, private authService: AuthService) {}


  //Getters:

  isMyComment(commentUsername: string | null | undefined): boolean {
    return this.authService.currentProfile()?.username === commentUsername;
  }

  loadReplies() {
    if (this.comment.loaded || !this.comment.id) return;
    this.fetchReplies();

    this.comment.loadingReplies = true;
    this.commentService.getCommentsReplies(this.comment.id).subscribe({
      next: (replies) => {
        this.comment.replies = replies as UIComment[];
        this.comment.loaded = true;
        this.comment.loadingReplies = false;
      },
      error: () => this.comment.loadingReplies = false
    });
  }

  get avatarUrl(): string | null {
    const url = this.comment.userAvatarUrl;
    if (url == null) return "img/user/avatars/avatar.jpg";
    return `${url}`;
  }

  get avatarReplierUrl(): string {
    const profile = this.authService.currentProfile();
    if (profile?.hasAvatar && profile?.avatarUrl) {
      return profile.avatarUrl;
    }
    return "img/user/avatars/avatar.jpg";
  }

  //Helpers:
  reloadReplies() {
    if (!this.comment.id) return;
    this.comment.loaded = false;
    this.fetchReplies();
  }

  private fetchReplies() {
    this.comment.loadingReplies = true;
    this.commentService.getCommentsReplies(this.comment.id!).subscribe({
      next: (replies) => {
        this.comment.replies = replies as UIComment[];
        this.comment.replyCount = replies.length;
        this.comment.loaded = true;
        this.comment.loadingReplies = false;
      },
      error: () => this.comment.loadingReplies = false
    });
  }

  autoResize(event: Event) {
    const el = event.target as HTMLTextAreaElement;
    el.style.height = 'auto';
    el.style.height = el.scrollHeight + 'px';
  }

  //Edit Comment:

  editComment() {
    if(this.comment.content) {
      this.editText = this.comment.content;
      this.isEditing = true;
      this.replyFormVisible = false;

      if(this.isReply) {
        this.isReply = false;
      }
    }

  }

  cancelEdit() {
    this.isEditing = false;
    this.editText = '';
    this.editError = '';
  }

  saveEdit() {
    if (!this.editText.trim() || this.editText === this.comment.content) {
      this.cancelEdit();
      return;
    }

    const dto: CommentContentUpdateDto = {
      articleId: this.articleId!,
      content: this.editText.trim(),
      id: this.comment.id
    };

    this.commentService.updateComment(dto).subscribe({
      next: () => {
        this.comment.content = this.editText.trim();
        this.isEditing = false;
        this.editText = '';
        this.editError = '';

        if (this.comment.id) {
          this.comment.loaded = false;
          this.commentService.getCommentsReplies(this.comment.id).subscribe({
            next: (replies) => {
              this.comment.replies = replies as UIComment[];
              this.comment.replyCount = replies.length;
              this.comment.loaded = true;
            }
          });
        }
      },
      error: (err: HttpValidationError) => {
        this.editError = err?.fieldErrors?.['content']?.[0] ?? err?.title ?? 'Hiba történt a mentés során';
      }
    });
  }

  // comment-item.component.ts

  vote(isUpvote: boolean) {
    const profile = this.authService.currentProfile();
    if (!profile || !this.comment.id) return;

    const oldVote = this.comment.currentUserVote || 0;
    const oldUp = this.comment.upVotes || 0;
    const oldDown = this.comment.downVotes || 0;

    const targetVote = isUpvote ? 1 : -1;

    if (this.comment.currentUserVote === targetVote) {
      if (isUpvote) this.comment.upVotes!--; else this.comment.downVotes!--;
      this.comment.currentUserVote = 0;
    } else {
      if (this.comment.currentUserVote === 1) this.comment.upVotes!--;
      if (this.comment.currentUserVote === -1) this.comment.downVotes!--;

      if (isUpvote) this.comment.upVotes!++; else this.comment.downVotes!++;
      this.comment.currentUserVote = targetVote;
    }

    const dto: CommentUpdateVoteDto = {
      commentId: this.comment.id,
      userId: profile.userId,
      isUpvote: isUpvote
    };

    this.commentService.updateVote(dto).subscribe({
      error: (err) => {
        this.comment.currentUserVote = oldVote;
        this.comment.upVotes = oldUp;
        this.comment.downVotes = oldDown;
        console.error(err);
      }
    });
  }

  //Reply Comment:

  toggleReplyForm() {
    this.replyFormVisible = !this.replyFormVisible;
    if (this.isEditing) this.isEditing = false;
    if (!this.replyFormVisible) {
      this.replyText = '';
      this.replyError = '';
    }
  }

  submitReply() {
    const userId = this.authService.currentProfile()?.userId;
    if (!this.replyText.trim() || !userId) return;

    const dto: CommentCreateDto = {
      articleId: this.articleId!,
      content: this.replyText.trim(),
      replyToCommentId: this.comment.id
    };

    this.commentService.createComment(dto, userId).subscribe({
      next: () => {
        this.replyText = '';
        this.replyFormVisible = false;
        this.replyError = '';

        if (this.comment.id) {
          this.comment.loaded = false;
          this.commentService.getCommentsReplies(this.comment.id).subscribe({
            next: (replies) => {
              this.comment.replies = replies as UIComment[];
              this.comment.replyCount = replies.length;
              this.comment.loaded = true;
              this.repliesVisible = true;
            }
          });
        }
      },
      error: (err: HttpValidationError) => {
        this.replyError = err?.fieldErrors?.['content']?.[0] ?? err?.title ?? 'Hiba történt a válasz küldése során';
      }
    });
  }

  //Delete Comment:

  deleteComment(id: string) {
    if(id.length > 0){
      this.commentService.deleteComment(id).subscribe({
        next: () => {
          this.commentDeleted.emit();
        },
        error: (err) => console.error(err)
      });
    }

    return;
  }
}
