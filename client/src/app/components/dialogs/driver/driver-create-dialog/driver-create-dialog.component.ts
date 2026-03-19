import {Component, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {MatDialogActions, MatDialogContent, MatDialogRef, MatDialogTitle} from '@angular/material/dialog';
import {DriverService} from '../../../../services/api/driver.service';
import {DriverCreateDto} from '../../../../api/models/driver-create-dto';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {FormErrorService} from '../../../../services/form-error.service';
import {HttpValidationError} from '../../../../services/error-interceptor.service';

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
  isSubmitting = false;
  form!: FormGroup;

  private readonly fieldMap: { [key: string]: string } = {
    'name': 'name',
    'nationality': 'nationality',
    'birthdate': 'birthDate',
    'totalraces': 'totalRaces',
    'totalwins': 'totalWins',
    'totalpodiums': 'totalPodiums',
    'championships': 'championships',
    'polepositions': 'polePositions',
    'seasons': 'seasons',
  };

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<DriverCreateDialogComponent>,
    private driverService: DriverService,
    private formErrorService: FormErrorService
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      name:          ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
      nationality:   ['', [Validators.required, Validators.maxLength(50)]],
      birthDate:     [null, Validators.required],
      totalRaces:    [0, [Validators.required, Validators.min(0), Validators.max(500)]],
      totalWins:     [0, [Validators.required, Validators.min(0), Validators.max(500)]],
      totalPodiums:  [0, [Validators.required, Validators.min(0), Validators.max(1000)]],
      championships: [0, [Validators.required, Validators.min(0), Validators.max(99)]],
      polePositions: [0, [Validators.required, Validators.min(0), Validators.max(200)]],
      seasons:       [1, [Validators.required, Validators.min(1), Validators.max(30)]],
    });

    this.formErrorService.clearServerErrorOnChange([
      this.form.get('name') as FormControl,
      this.form.get('nationality') as FormControl,
      this.form.get('birthDate') as FormControl,
      this.form.get('totalRaces') as FormControl,
      this.form.get('totalWins') as FormControl,
      this.form.get('totalPodiums') as FormControl,
      this.form.get('championships') as FormControl,
      this.form.get('polePositions') as FormControl,
      this.form.get('seasons') as FormControl,
    ]);
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
      error: (err: HttpValidationError) => {
        this.isSubmitting = false;
        this.formErrorService.applyServerErrors(this.form, err, this.fieldMap);
      }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
