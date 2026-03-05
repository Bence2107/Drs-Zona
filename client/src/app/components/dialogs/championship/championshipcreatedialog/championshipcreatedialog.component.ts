import {Component, inject, OnInit} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {SeriesLookupDto} from '../../../../api/models/series-lookup-dto';
import {ResultsService} from '../../../../services/results.service';
import {ChampionshipCreateDto} from '../../../../api/models/championship-create-dto';

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
  templateUrl: './championshipcreatedialog.component.html',
  styleUrl: './championshipcreatedialog.component.scss',
})
export class ChampionshipcreatedialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ChampionshipcreatedialogComponent>);
  private resultService = inject(ResultsService);
  data = inject(MAT_DIALOG_DATA) as { seriesList: SeriesLookupDto[] };

  isSubmitting = false;

  form: FormGroup = this.fb.group({
    seriesId:         ['', Validators.required],
    season:           ['', [Validators.required, Validators.pattern(/^\d{4}$/)]],
    driversName:      ['', Validators.required],
    constructorsName: ['', Validators.required],
  });

  get seriesList() { return this.data.seriesList; }

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
      error: () => {
        this.isSubmitting = false;
      }
    });
  }

  cancel() {
    this.dialogRef.close(false);
  }
}
