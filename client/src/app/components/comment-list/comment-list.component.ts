import {Component, Input, OnInit} from '@angular/core';
import { MatCard } from '@angular/material/card';
import {CommentService} from '../../services/comment.service';
import {UIComment} from '../../models/ui-comment';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {CommentItemComponent} from './comment-item/comment-item.component';
import {MatIcon} from '@angular/material/icon';
import {AuthService} from '../../services/auth.service';

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

  submitComment() {
    if (!this.newCommentText.trim()) return;
    console.log('Comment:', this.newCommentText);
    this.newCommentText = '';
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
