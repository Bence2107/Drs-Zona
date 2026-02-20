import {Component, inject, Input} from '@angular/core';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';
import {MatCard} from '@angular/material/card';
import {MatList, MatListItem} from '@angular/material/list';
import {MatIcon} from '@angular/material/icon';
import {DatePipe} from '@angular/common';

@Component({
  selector: 'app-profile-general',
  imports: [
    MatCard,
    MatListItem,
    MatList,
    MatIcon
  ],
  providers: [DatePipe],
  templateUrl: './profile-general.component.html',
  styleUrl: './profile-general.component.scss',
})
export class ProfileGeneralComponent {
  @Input() userData: UserProfileResponse | null = null;

  private datePipe = inject(DatePipe);

  get profileFields() { return [
    { label: 'Teljes név:', value: this.userData?.fullName, icon: 'person' },
    { label: 'Felhasználónév:', value: this.userData?.username, icon: 'person' },
    { label: 'Email Cím:', value: this.userData?.email, icon: 'email' },
    { label: "Létrehozva:", value: this.datePipe.transform(this.userData?.createdAt, 'yyyy. MM. dd. HH:mm') , icon: 'calendar_today' },
  ];
    }
}
