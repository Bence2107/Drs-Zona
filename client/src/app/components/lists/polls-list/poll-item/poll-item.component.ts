import { Component, Input, Output, EventEmitter, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PollListDto } from '../../../../api/models/poll-list-dto';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-poll-item',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatRadioModule,
    MatProgressBarModule,
  ],
  templateUrl: './poll-item.component.html',
  styleUrl: './poll-item.component.scss',
})
export class PollItemComponent implements OnInit {
  @Input() poll: PollListDto | null = null;
  @Output() voteSubmitted = new EventEmitter<string>();
  @Output() voteRemoved = new EventEmitter<string>();

  isExpired = false;
  expiresIn = '';

  ngOnInit() {
    if (this.poll) {
      this.updateExpirationStatus();
    }
  }

  get IsExpired(): boolean {
    if (!this.poll?.expiresAt) return false;
    return new Date(this.poll.expiresAt) < new Date();
  }

  private updateExpirationStatus() {
    if (!this.poll || !this.poll.expiresAt) return;
    const now = new Date();
    const expiresAt = new Date(this.poll.expiresAt);
    this.isExpired = expiresAt <= now;
    if (!this.isExpired) {
      this.expiresIn = this.getTimeRemaining(expiresAt, now);
    }
  }

  private getTimeRemaining(expiresAt: Date, now: Date): string {
    const diffMs = expiresAt.getTime() - now.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMins / 60);
    const diffDays = Math.floor(diffHours / 24);

    if (diffDays > 0) return `${diffDays} day${diffDays > 1 ? 's' : ''} left`;
    if (diffHours > 0) return `${diffHours} hour${diffHours > 1 ? 's' : ''} left`;
    if (diffMins > 0) return `${diffMins} minute${diffMins > 1 ? 's' : ''} left`;
    return 'Expires soon';
  }


}
