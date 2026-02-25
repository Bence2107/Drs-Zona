import {Component, Input, OnInit} from '@angular/core';
import {PollListComponent} from '../../../../../../components/polls-list/polls-list.component';
import {PollListDto} from '../../../../../../api/models/poll-list-dto';
import {PollService} from '../../../../../../services/poll.service';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';

@Component({
  selector: 'app-profile-polls',
  imports: [
    PollListComponent
  ],
  templateUrl: './profile-polls.component.html',
  styleUrl: './profile-polls.component.scss',
})
export class ProfilePollsComponent implements OnInit {
    polls: PollListDto[] = [];
    @Input() userData: UserProfileResponse | null = null;

    constructor(private pollService: PollService) {}

    ngOnInit(): void {
      if (this.userData?.userId) {
        this.pollService.getPollsByUser(this.userData.userId).subscribe(
          (polls: PollListDto[]) => {
            this.polls = polls;
            console.log(polls);
          }
        )
      }
    }
}
