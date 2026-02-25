import { Component, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PollDto } from '../../api/models/poll-dto';
import { PollService } from '../../services/poll.service';

export interface PollDialogData {
  poll: PollDto;
  userId: string | null;
}

@Component({
  selector: 'app-poll-vote-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatIconModule],
  templateUrl: './poll-vote-dialog.component.html',
  styleUrl: './poll-vote-dialog.component.scss'
})

export class PollVoteDialogComponent {
  poll: PollDto;
  userId: string | null;

  constructor(
    public dialogRef: MatDialogRef<PollVoteDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public data: PollDialogData,
    private pollService: PollService
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
    if (!this.userId) return;

    const selectedOption = this.poll.pollOptions?.find(o => o.id === optionId);
    if (!selectedOption) return;

    if (selectedOption.isUserChoice) {
      this.pollService.removeVote(this.poll.id!, optionId, this.userId).subscribe({
        next: () => this.refreshData()
      });
    }
    else if (this.voted) {
      const previousOption = this.poll.pollOptions?.find(o => o.isUserChoice);
      if (previousOption) {
        this.pollService.removeVote(this.poll.id!, previousOption.id!, this.userId).subscribe({
          next: () => {
            this.pollService.vote(this.poll.id!, optionId, this.userId!).subscribe({
              next: () => this.refreshData()
            });
          }
        });
      }
    }
    else {
      this.pollService.vote(this.poll.id!, optionId, this.userId).subscribe({
        next: () => this.refreshData()
      });
    }
  }

  private castVote(optionId: string) {
    this.pollService.vote(this.poll.id!, optionId, this.userId!).subscribe({
      next: () => this.updateLocalUI(optionId, true),
      error: (err) => {
        if (err.status === 400) alert("Már leadtál egy szavazatot!");
      }
    });
  }

  private handleRemoveVote(optionId: string) {
    this.pollService.removeVote(this.poll.id!, optionId, this.userId!).subscribe({
      next: () => this.updateLocalUI(optionId, false)
    });
  }

  private updateLocalUI(optionId: string, isAdd: boolean) {
    if (!this.poll.pollOptions) return;
    this.poll.pollOptions.forEach(opt => {
      if (opt.id === optionId) {
        opt.isUserChoice = isAdd;
        if (opt.votePercentage !== undefined) {
          opt.votePercentage = isAdd ? opt.votePercentage + 1 : Math.max(0, opt.votePercentage - 1);
        }
      } else if (isAdd) {
        opt.isUserChoice = false;
      }
    });
  }
}
