import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {ResultsService} from '../../../../services/results.service';
import {ChampionshipCreateDto} from '../../../../api/models/championship-create-dto';
import {FormErrorService} from '../../../../services/form-error.service';
import {HttpValidationError} from '../../../../services/error-interceptor.service';

@Component({
  selector: 'app-championshipcreatedialog',
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule
  ],
  templateUrl: './championship-create-dialog.component.html',
  styleUrl: './championship-create-dialog.component.scss',
})
export class ChampionshipCreateDialogComponent implements OnInit {
  data = inject(MAT_DIALOG_DATA) as { seriesList: SeriesLookupDto[] };
  isSubmitting = false;
  form: FormGroup

  get seriesList() { return this.data.seriesList; }

  private readonly fieldMap: { [key: string]: string } = {
    'seriesid': 'seriesId',
    'season': 'season',
    'driversname': 'driversName',
    'constructorsname': 'constructorsName',
  };

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ChampionshipCreateDialogComponent>,
    private resultService: ResultsService,
    private formErrorService: FormErrorService,
  ) {
    this.form =  this.fb.group({
      seriesId:         ['', Validators.required],
      season:           ['', [Validators.required, Validators.pattern(/^\d{4}$/)]],
      driversName:      ['', Validators.required],
      constructorsName: ['', Validators.required],
    });
  }

  ngOnInit() {
    if (this.seriesList.length === 1) {
      this.form.patchValue({ seriesId: this.seriesList[0].id });
    }

    this.form.get('season')?.valueChanges.subscribe(year => {
      if (year?.length === 4) {
        const seriesName = this.seriesList.find(
          s => s.id === this.form.get('seriesId')?.value
        )?.name ?? '';
        this.form.patchValue({
          driversName: `${year} ${seriesName} Drivers Championship`,
          constructorsName: `${year} ${seriesName} Constructors Championship`,
        }, { emitEvent: false });
      }
    });

    this.form.get('seriesId')?.valueChanges.subscribe(() => {
      const year = this.form.get('season')?.value;
      if (year?.length === 4) {
        const seriesName = this.seriesList.find(
          s => s.id === this.form.get('seriesId')?.value
        )?.name ?? '';
        this.form.patchValue({
          driversName: `${year} ${seriesName} Drivers Championship`,
          constructorsName: `${year} ${seriesName} Constructors Championship`,
        }, { emitEvent: false });
      }
    });

    this.formErrorService.clearServerErrorOnChange([
      this.form.get('seriesId') as FormControl,
      this.form.get('season') as FormControl,
      this.form.get('driversName') as FormControl,
      this.form.get('constructorsName') as FormControl,
    ]);
  }

  submit() {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const dto: ChampionshipCreateDto = {
      seriesId: this.form.value.seriesId,
      season: this.form.value.season,
      driversName: this.form.value.driversName,
      constructorsName: this.form.value.constructorsName
    };

    this.resultService.createChampionship(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.dialogRef.close(true);
      },
      error: (err: HttpValidationError) => {
        this.isSubmitting = false;
        this.formErrorService.applyServerErrors(this.form, err, this.fieldMap);
      }
    });
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
