import {Component, ViewChild, ElementRef, Input} from '@angular/core';
import { CommonModule } from '@angular/common';
import { PollService } from '../../../services/poll.service';
import { PollListDto } from '../../../api/models/poll-list-dto';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { PollItemComponent } from './poll-item/poll-item.component';
import {PollVoteDialogComponent} from '../../dialogs/poll/poll-vote-dialog/poll-vote-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {AuthService} from '../../../services/auth.service';
import {UserProfileResponse} from '../../../api/models/user-profile-response';
import {PollAddDialogComponent} from '../../dialogs/poll/poll-add-dialog/poll-add-dialog.component';

@Component({
  selector: 'app-poll-list',
  standalone: true,
  imports: [CommonModule, PollItemComponent, MatButtonModule, MatIconModule],
  templateUrl: './polls-list.component.html',
  styleUrl: './polls-list.component.scss'
})
export class PollListComponent {
  @ViewChild('carousel', { static: false }) carousel!: ElementRef;
  @Input() userData: UserProfileResponse | null = null;
  @Input() polls: PollListDto[] = [];
  constructor(private pollService: PollService, private dialog: MatDialog, private authService: AuthService) {}

  get userId(): string | null {
    return this.authService.currentProfile()?.userId ?? null;
  }

  scroll(direction: number) {
    const scrollAmount = 374 * direction;
    this.carousel.nativeElement.scrollBy({ left: scrollAmount, behavior: 'smooth' });
  }

  openPollDetails(pollId: string) {
    this.pollService.getPoll(pollId).subscribe({
      next: (pollData) => {
        const dialogRef = this.dialog.open(PollVoteDialogComponent, {
          width: '700px',
          data: { poll: pollData, userId: this.userId }
        });

        dialogRef.afterClosed().subscribe((updatedPoll) => {
          if (updatedPoll) {
            const index = this.polls.findIndex(p => p.id === updatedPoll.id);
            if (index !== -1) {
              this.polls[index] = updatedPoll;
            }
          }
        });
      }
    });
  }

  protected openCreatePollDialog() {
      const dialogRef = this.dialog.open(PollAddDialogComponent, {
        width: '1000px',
        disableClose: true
      });

      dialogRef.afterClosed().subscribe(result => {
        if (result) {
          this.pollService.getAllActive().subscribe(data => this.polls = data);
        }
      });
  }
}
