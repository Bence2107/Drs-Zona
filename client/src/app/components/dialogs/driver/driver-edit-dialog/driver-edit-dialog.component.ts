import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from "@angular/material/dialog";
import {DriverService} from '../../../../services/driver.service';
import {DriverDetailDto} from '../../../../api/models/driver-detail-dto';
import {DriverUpdateDto} from '../../../../api/models/driver-update-dto';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';

@Component({
  selector: 'app-driver-edit-dialog',
  imports: [
    MatDialogTitle,
    MatDialogContent,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatInput,
    MatError,
    MatDatepickerInput,
    MatDatepickerToggle,
    MatDatepicker,
    MatDialogActions,
    MatButton,
    MatIcon
  ],
  templateUrl: './driver-edit-dialog.component.html',
  styleUrl: './driver-edit-dialog.component.scss',
})
export class DriverEditDialogComponent implements OnInit{
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<DriverEditDialogComponent>);
  private driverService = inject(DriverService);
  data = inject(MAT_DIALOG_DATA) as { driver: DriverDetailDto };

  isSubmitting = false;
  form!: FormGroup;

  ngOnInit() {
    const d = this.data.driver;
    this.form = this.fb.group({
      name:          [d.name, [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      nationality:   [d.nationality, [Validators.required, Validators.maxLength(50)]],
      birthDate:     [d.birthDate ? new Date(d.birthDate) : null, Validators.required],
      totalRaces:    [d.totalRaces, [Validators.required, Validators.min(0)]],
      totalWins:     [d.totalWins, [Validators.required, Validators.min(0)]],
      totalPodiums:  [d.totalPodiums, [Validators.required, Validators.min(0)]],
      championships: [d.championships, [Validators.required, Validators.min(0)]],
      polePositions: [d.polePositions, [Validators.required, Validators.min(0)]],
      seasons:       [d.seasons, [Validators.required, Validators.min(0)]],
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const v = this.form.value;
    const dto: DriverUpdateDto = {
      id:            this.data.driver.id,
      name:          v.name,
      nationality:   v.nationality,
      birthDate:     v.birthDate instanceof Date ? v.birthDate.toISOString() : v.birthDate,
      totalRaces:    v.totalRaces,
      totalWins:     v.totalWins,
      totalPodiums:  v.totalPodiums,
      championships: v.championships,
      polePositions: v.polePositions,
      seasons:       v.seasons,
    };

    this.driverService.updateDriver(dto).subscribe({
      next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
      error: () => { this.isSubmitting = false; }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
