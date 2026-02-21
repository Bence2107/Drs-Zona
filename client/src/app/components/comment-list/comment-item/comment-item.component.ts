import {Component, Input} from '@angular/core';
import {UIComment} from '../../../models/ui-comment';
import {CommentService} from '../../../services/comment.service';
import { DatePipe } from '@angular/common';
import {MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatExpansionPanel, MatExpansionPanelHeader, MatExpansionPanelTitle} from '@angular/material/expansion';
import {FormsModule} from '@angular/forms';
import {AuthService} from '../../../services/auth.service';

@Component({
  selector: 'app-comment-item',
  imports: [
    DatePipe,
    MatIconButton,
    MatIcon,
    MatExpansionPanel,
    MatExpansionPanelHeader,
    MatExpansionPanelTitle,
    FormsModule
  ],
  templateUrl: './comment-item.component.html',
  styleUrl: './comment-item.component.scss'
})
export class CommentItemComponent {
  @Input() comment!: UIComment;
  @Input() isReply: boolean = false;

  @Input() depth: number = 0;
  repliesVisible = false;

  constructor(private commentService: CommentService, private authService: AuthService) {}

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
    if (!this.replyFormVisible) this.replyText = '';
  }

  submitReply() {
    if (!this.replyText.trim()) return;
    console.log('Reply:', this.replyText);
    this.replyText = '';
    this.replyFormVisible = false;
  }

  autoResize(event: Event) {
    const el = event.target as HTMLTextAreaElement;
    el.style.height = 'auto';
    el.style.height = el.scrollHeight + 'px';
  }
}
