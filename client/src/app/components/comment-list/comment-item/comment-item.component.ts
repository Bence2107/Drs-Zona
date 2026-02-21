import {Component, Input} from '@angular/core';
import {UIComment} from '../../../models/ui-comment';
import {CommentService} from '../../../services/comment.service';
import { DatePipe } from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {FormsModule} from '@angular/forms';
import {AuthService} from '../../../services/auth.service';
import {CommentCreateDto} from '../../../api/models/comment-create-dto';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {CommentContentUpdateDto} from '../../../api/models/comment-content-update-dto';


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
    MatMenuTrigger
  ],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input() comment!: UIComment;
  @Input() articleId!: string;

  @Input() isReply: boolean = false;
  @Input() isOnProfileSite: boolean = false;

  @Input() depth: number = 0;

  repliesVisible = false;

  isEditing = false;
  editText = '';

  constructor(private commentService: CommentService, private authService: AuthService) {}

  isMyComment(commentUsername: string | null | undefined): boolean {
    return this.authService.currentProfile()?.username === commentUsername;
  }

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
  }

  saveEdit() {
    if (!this.editText.trim() || this.editText === this.comment.content) {
      this.cancelEdit();
      return;
    }

    const dto: CommentContentUpdateDto = {
      articleId: this.articleId,
      content: this.editText.trim(),
      id: this.comment.id
    };

    this.commentService.updateComment(dto).subscribe({
      next: () => {
        this.comment.content = this.editText.trim();

        this.isEditing = false;
        this.editText = '';


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
      error: (err) => {
        console.error("Hiba a mentés során:", err);
      }
    });
  }
  loadReplies() {
    if (this.comment.loaded || !this.comment.id) return;

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

  replyFormVisible = false;
  replyText = '';

  toggleReplyForm() {
    this.replyFormVisible = !this.replyFormVisible;
    if(this.isEditing) {
      this.isEditing = false;
    }
    if (!this.replyFormVisible) this.replyText = '';
  }

  submitReply() {
    const userId = this.authService.currentProfile()?.userId;
    if (!this.replyText.trim() || !userId) return;

    const dto: CommentCreateDto = {
      articleId: this.articleId,
      content: this.replyText.trim(),
      replyToCommentId: this.comment.id
    };

    this.commentService.createComment(dto, userId).subscribe({
      next: () => {
        this.replyText = '';
        this.replyFormVisible = false;

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
      }
    });
  }

  autoResize(event: Event) {
    const el = event.target as HTMLTextAreaElement;
    el.style.height = 'auto';
    el.style.height = el.scrollHeight + 'px';
  }

  deleteComment(id: string | undefined) {

  }
}
