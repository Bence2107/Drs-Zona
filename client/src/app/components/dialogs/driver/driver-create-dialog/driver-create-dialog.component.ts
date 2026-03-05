import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {DriverService} from '../../../../services/driver.service';
import {DriverCreateDto} from '../../../../api/models/driver-create-dto';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';

@Component({
  selector: 'app-driver-create-dialog',
  imports: [
    MatDialogTitle,
    MatDialogContent,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatInput,
    MatError,
    MatDatepickerToggle,
    MatDatepickerInput,
    MatDatepicker,
    MatDialogActions,
    MatButton,
    MatIcon
  ],
  templateUrl: './driver-create-dialog.component.html',
  styleUrl: './driver-create-dialog.component.scss',
})
export class DriverCreateDialogComponent implements OnInit{
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<DriverCreateDialogComponent>);
  private driverService = inject(DriverService);

  isSubmitting = false;
  form!: FormGroup;

  ngOnInit() {
    this.form = this.fb.group({
      name:          ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      nationality:   ['', [Validators.required, Validators.maxLength(50)]],
      birthDate:     [null, Validators.required],
      totalRaces:    [0, [Validators.required, Validators.min(0)]],
      totalWins:     [0, [Validators.required, Validators.min(0)]],
      totalPodiums:  [0, [Validators.required, Validators.min(0)]],
      championships: [0, [Validators.required, Validators.min(0)]],
      polePositions: [0, [Validators.required, Validators.min(0)]],
      seasons: [0, [Validators.required, Validators.min(0)]],
    });
  }

  submit() {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const v = this.form.value;
    const dto: DriverCreateDto = {
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

    this.driverService.createDriver(dto).subscribe({
      next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
      error: () => { this.isSubmitting = false; }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
