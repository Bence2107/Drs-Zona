import {Component, Input} from '@angular/core';
import { MatCard } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { AuthService } from '../../../../services/auth.service';
import { UserProfileResponse } from '../../../../api/models/user-profile-response';
import {MatTooltip} from '@angular/material/tooltip';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {ConfirmDialogComponent} from '../../../../components/confirmdialog/confirmdialog.component';
import {MatDialog} from '@angular/material/dialog';
import {CustomSnackbarComponent} from '../../../../components/custom-snackbar/custom-snackbar.component';
import {MatSnackBar} from '@angular/material/snack-bar';

@Component({
  selector: 'app-profile-header',
  standalone: true,
  imports: [MatCard, MatIcon, MatTooltip, MatMenuTrigger, MatMenu, MatMenuItem],
  templateUrl: './profile-header.component.html',
  styleUrl: './profile-header.component.scss',
})
export class ProfileHeaderComponent {
  @Input() userData: UserProfileResponse | null = null;
  @Input() avatarUrl: string | null = null;

  constructor(private authService: AuthService, private dialog: MatDialog, private snackBar: MatSnackBar) {}


  get hasAvatar(): boolean {
    return this.avatarUrl !== "img/user/avatars/avatar.jpg";
  }

  updateAvatar(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      this.authService.updateProfilePicture(file).subscribe({
        next: () => {
          this.snackBar.openFromComponent(CustomSnackbarComponent, {
            data: { message: 'Profilkép sikeresen feltöltve', actionLabel: 'Rendben' },
            duration: 3000,
            horizontalPosition: 'center',
          });
        },
        error: (err) =>  {
          this.snackBar.openFromComponent(CustomSnackbarComponent, {
            data: { message: 'Hiba a feltöltésnél.' + err, actionLabel: 'Rendben' },
            duration: 3000,
            horizontalPosition: 'center',
          });
        }
      });
    }
  }

  deleteAvatar(): void {
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Profilkép törlése',
        message: 'Biztosan törölni szeretnéd a jelenlegi profilképedet?',
        confirmText: 'Igen',
        cancelText: 'Nem'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.authService.deleteProfilePicture().subscribe({
          next: () => {
            this.snackBar.openFromComponent(CustomSnackbarComponent, {
              data: { message: 'Profilkép sikeresen törölve', actionLabel: 'Rendben' },
              duration: 3000,
              horizontalPosition: 'center',
            });
          },
          error: (err) => {
            this.snackBar.openFromComponent(CustomSnackbarComponent, {
              data: { message: 'Profilkép törlése sikertelen:' + err, actionLabel: 'Rendben' },
              duration: 3000,
              horizontalPosition: 'center',
            });
          }
        });

      }
    });
  }
}
