import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PollDto } from '../../api/models/poll-dto';
import { PollService } from '../../services/poll.service';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {AuthService} from '../../services/auth.service';

export interface PollDialogData {
  poll: PollDto;
  userId: string | null;
}

@Component({
  selector: 'app-poll-vote-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule, MatMenuTrigger, MatMenu, MatMenuItem],
  templateUrl: './poll-vote-dialog.component.html',
  styleUrl: './poll-vote-dialog.component.scss'
})

export class PollVoteDialogComponent {
  poll: PollDto;
  userId: string | null;

  constructor(
    public dialogRef: MatDialogRef<PollVoteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PollDialogData,
    private pollService: PollService,
    private authService: AuthService
  ) {
    this.poll = data.poll; //
    this.userId = data.userId; //
  }

  get voted(): boolean {
    return !!this.poll.pollOptions?.some(o => o.isUserChoice);
  }

  private refreshData() {
    this.pollService.getPoll(this.poll.id!).subscribe({
      next: (updatedPoll) => {
        this.poll = updatedPoll;
      },
      error: (err) => console.error('Hiba a frissítéskor', err)
    });
  }

  onVote(optionId: string): void {
    const selectedOption = this.poll.pollOptions?.find(o => o.id === optionId);
    if (!this.userId || !selectedOption || selectedOption.isUserChoice) return;

    this.pollService.vote(this.poll.id!, optionId, this.userId).subscribe({
      next: () => this.refreshData(),
      error: (err) => console.error('Hiba a szavazásnál:', err)
    });
  }

  isMyPoll(authorId: string | null | undefined) {
    return this.authService.currentProfile()?.userId === authorId;
  }

  removePoll() {
     this.pollService.removePoll(this.poll.id!).subscribe({
       next: () => this.dialogRef.close()
     });
  }
}
