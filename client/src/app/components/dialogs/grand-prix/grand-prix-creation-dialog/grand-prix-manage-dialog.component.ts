import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import {CircuitListDto} from '../../../../api/models/circuit-list-dto';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';
import {GrandPrixCreateDto} from '../../../../api/models/grand-prix-create-dto';
import {GrandPrixUpdateDto} from '../../../../api/models/grand-prix-update-dto';
import {GrandPrixDetailDto} from '../../../../api/models/grand-prix-detail-dto';
import {FormErrorService} from '../../../../services/form-error.service';
import {HttpValidationError} from '../../../../services/error-interceptor.service';
import {GrandsPrixService} from '../../../../services/api/grands-prix.service';

export interface GrandPrixDialogData {
  seriesList: SeriesLookupDto[];
  preselectedSeriesId?: string;
  preselectedYear?: number;
  editData?: GrandPrixDetailDto;
}

@Component({
  selector: 'app-grandprix-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule,
    MatIconModule, MatDatepickerModule, MatNativeDateModule,
    CountryFlagPipe
  ],
  templateUrl: './grand-prix-manage-dialog.component.html',
  styleUrl: './grand-prix-manage-dialog.component.scss',
})
export class GrandPixManageDialogComponent implements OnInit {
  circuits = signal<CircuitListDto[]>([]);

  form: FormGroup;
  data = inject(MAT_DIALOG_DATA) as GrandPrixDialogData;
  isSubmitting = false;
  isEditMode = false;

  private readonly fieldMap: { [key: string]: string } = {
    'seriesid': 'seriesId',
    'circuitid': 'circuitId',
    'name': 'name',
    'shortname': 'shortName',
    'roundnumber': 'roundNumber',
    'seasonyear': 'seasonYear',
    'starttime': 'startTime',
    'endtime': 'endTime',
    'racedistance': 'raceDistance',
    'lapscompleted': 'lapsCompleted',
  };


  constructor(
    private fb: FormBuilder,
    private dialogRef:MatDialogRef<GrandPixManageDialogComponent>,
    private formErrorService: FormErrorService,
    private grandPrixService: GrandsPrixService,
) {

    this.form = this.fb.group({
      seriesId:      ['', Validators.required],
      circuitId:     ['', Validators.required],
      name:          ['', [Validators.required, Validators.maxLength(100)]],
      shortName:     ['', [Validators.required, Validators.maxLength(100)]],
      roundNumber:   [null, [Validators.required, Validators.min(1), Validators.max(30)]],
      seasonYear:    [null, [Validators.required, Validators.min(1906), Validators.max(2100)]],
      startTime:     [null, Validators.required],
      endTime:       [null, Validators.required],
      raceDistance:  [null, [Validators.required, Validators.min(1), Validators.max(1000)]],
      lapsCompleted: [null, [Validators.min(0), Validators.max(100)]],
    });
  }

  get seriesList() { return this.data.seriesList; }
  get title() { return this.isEditMode ? 'Nagydíj szerkesztése' : 'Új nagydíj létrehozása'; }
  get submitLabel() { return this.isEditMode ? 'Mentés' : 'Létrehozás'; }

  ngOnInit() {
    this.grandPrixService.getAllCircuitsToList().subscribe(res => this.circuits.set(res));

    if (this.data.editData) {
      this.isEditMode = true;
      this.loadEditData(this.data.editData);
    } else {
      if (this.data.preselectedSeriesId) this.form.patchValue({ seriesId: this.data.preselectedSeriesId });
      if (this.data.preselectedYear) this.form.patchValue({ seasonYear: this.data.preselectedYear });
    }

    this.formErrorService.clearServerErrorOnChange([
      this.form.get('seriesId') as FormControl,
      this.form.get('circuitId') as FormControl,
      this.form.get('name') as FormControl,
      this.form.get('shortName') as FormControl,
      this.form.get('roundNumber') as FormControl,
      this.form.get('seasonYear') as FormControl,
      this.form.get('startTime') as FormControl,
      this.form.get('endTime') as FormControl,
      this.form.get('raceDistance') as FormControl,
      this.form.get('lapsCompleted') as FormControl,
    ]);
  }

  private loadEditData(data: GrandPrixDetailDto) {
    this.form.patchValue({
      seriesId:      data.seriesId,
      name:          data.name,
      shortName:     data.name,
      roundNumber:   data.roundNumber,
      seasonYear:    data.seasonYear,
      startTime:     data.startTime ? new Date(data.startTime) : null,
      endTime:       data.endTime ? new Date(data.endTime) : null,
      raceDistance:  data.raceDistance,
      lapsCompleted: data.lapsCompleted,
    });

    const readonlyFields = ['seriesId', 'circuitId', 'name', 'shortName', 'roundNumber', 'seasonYear', 'raceDistance'];
    readonlyFields.forEach(f => this.form.get(f)?.disable());
  }

  submit() {
    this.form.markAllAsTouched();
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const v = this.form.getRawValue();

    if (this.isEditMode) {
      const dto: GrandPrixUpdateDto = {
        id: this.data.editData!.id,
        startTime: v.startTime instanceof Date ? v.startTime.toISOString() : v.startTime,
        endTime:   v.endTime instanceof Date ? v.endTime.toISOString() : v.endTime,
        lapsCompleted: v.lapsCompleted,
      };

      this.grandPrixService.updateGrandPrix(dto).subscribe({
        next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
        error: (err: HttpValidationError) => {
          this.isSubmitting = false;
          this.formErrorService.applyServerErrors(this.form, err, this.fieldMap);
        }
      });
    } else {
      const dto: GrandPrixCreateDto = {
        seriesId:      v.seriesId,
        circuitId:     v.circuitId,
        name:          v.name,
        shortName:     v.shortName,
        roundNumber:   v.roundNumber,
        seasonYear:    v.seasonYear,
        startTime:     v.startTime instanceof Date ? v.startTime.toISOString() : v.startTime,
        endTime:       v.endTime instanceof Date ? v.endTime.toISOString() : v.endTime,
        raceDistance:  v.raceDistance,
        lapsCompleted: v.lapsCompleted,
      };

      this.grandPrixService.createGrandPrix(dto).subscribe({
        next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
        error: (err: HttpValidationError) => {
          this.isSubmitting = false;
          this.formErrorService.applyServerErrors(this.form, err, this.fieldMap);
        }
      });
    }
  }

  cancel() { this.dialogRef.close(false); }
}
