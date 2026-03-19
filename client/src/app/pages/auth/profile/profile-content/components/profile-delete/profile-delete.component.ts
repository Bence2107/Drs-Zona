import {Component, Input} from '@angular/core';
import { UserProfileResponse } from "../../../../../../api/models";
import {MatCard} from '@angular/material/card';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {
  ConfirmDialogComponent,
  ConfirmDialogData
} from '../../../../../../components/dialogs/confirmdialog/confirm-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {MatCheckbox} from '@angular/material/checkbox';
import {FormsModule} from '@angular/forms';
import {AuthService} from '../../../../../../services/auth.service';
import {CustomSnackbarComponent} from '../../../../../../components/custom-snackbar/custom-snackbar.component';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-profile-delete',
  imports: [
    MatCard,
    MatButton,
    MatIcon,
    MatCheckbox,
    FormsModule
  ],
  templateUrl: './profile-delete.component.html',
  styleUrl: './profile-delete.component.scss',
})
export class ProfileDeleteComponent {
  @Input() userData: UserProfileResponse | null = null;
  isConfirmed: boolean = false;

  constructor(private dialog: MatDialog, private authService: AuthService, private snackBar: MatSnackBar) {}

  onDeleteAccount(): void {
    const dialogData: ConfirmDialogData = {
      title: 'Profil törlése',
      message: 'Biztosan törölni szeretnéd a profilodat? Ez a folyamat végleges és nem vonható vissza.',
      confirmText: 'Profil végleges törlése',
      cancelText: 'Mégse'
    };

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: dialogData
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.authService.deleteProfile(this.userData?.userId!).subscribe({
            next:() => {
              this.snackBar.openFromComponent(CustomSnackbarComponent, {
                data: { message: 'Profil sikeresen törölve', actionLabel: 'Rendben' },
                duration: 3000,
                horizontalPosition: 'center',
              });
            },
            error: err => {
              console.error(err);
            }
          }
        )
      }
    });
  }
}
