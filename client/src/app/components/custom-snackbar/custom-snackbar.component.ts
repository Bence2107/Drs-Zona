import { Component, Inject } from '@angular/core';
import { MatSnackBarRef, MAT_SNACK_BAR_DATA } from '@angular/material/snack-bar';
import { MatButton } from '@angular/material/button';

@Component({
  selector: 'app-custom-snackbar',
  imports: [
    MatButton
  ],
  template: `
    <div class="custom-snackbar">
      <span>{{ data.message }}</span>
      <button mat-flat-button (click)="dismiss()">{{ data.actionLabel }}</button>
    </div>
  `,
  styles: [`
    .custom-snackbar {
      display: flex;
      justify-content: space-between;
      align-items: center;
      color: var(--primary-text);
      background-color: var(--primary-bg);
      padding: 10px;
      border-radius: 5px;
      font-size: 14px;
    }
  `]
})
export class CustomSnackbarComponent {
  constructor(
    public snackBarRef: MatSnackBarRef<CustomSnackbarComponent>,
    @Inject(MAT_SNACK_BAR_DATA) public data: { message: string, actionLabel: string }
  ) {}

  dismiss() {
    this.snackBarRef.dismiss();
  }
}
