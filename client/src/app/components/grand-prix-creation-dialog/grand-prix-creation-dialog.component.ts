import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MAT_DIALOG_DATA, MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatNativeDateModule} from '@angular/material/core';
import {CircuitListDto} from '../../api/models/circuit-list-dto';
import {SeriesLookupDto} from '../../api/models/series-lookup-dto';
import {ResultsService} from '../../services/results.service';
import {CountryFlagPipe} from '../../pipes/country-flag.pipe';
import {GrandPrixCreateDto} from '../../api/models/grand-prix-create-dto';

@Component({
  selector: 'app-grandprix-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatIconModule,
    MatDatepickerModule,
    MatNativeDateModule,
    CountryFlagPipe
  ],
  templateUrl: './grand-prix-creation-dialog.component.html',
  styleUrl: './grand-prix-creation-dialog.component.scss',
})
export class GrandPrixCreateDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<GrandPrixCreateDialogComponent>);
  private resultService = inject(ResultsService);
  data = inject(MAT_DIALOG_DATA) as { seriesList: SeriesLookupDto[], preselectedSeriesId?: string, preselectedYear?: number };

  circuits = signal<CircuitListDto[]>([]);
  isSubmitting = false;

  form: FormGroup = this.fb.group({
    seriesId:      ['', Validators.required],
    circuitId:     ['', Validators.required],
    name:          ['', Validators.required],
    shortName:     ['', Validators.required],
    roundNumber:   [null, [Validators.required, Validators.min(1)]],
    seasonYear:    [null, [Validators.required, Validators.min(1950)]],
    startTime:     [null, Validators.required],
    endTime:       [null, Validators.required],
    raceDistance:  [null, [Validators.required, Validators.min(1)]],
    lapsCompleted: [null, [Validators.required, Validators.min(1)]],
  });

  get seriesList() { return this.data.seriesList; }

  ngOnInit() {
    this.resultService.getAllCircuitsToList().subscribe(res => {
      this.circuits.set(res);
    });

    if (this.data.preselectedSeriesId) {
      this.form.patchValue({ seriesId: this.data.preselectedSeriesId });
    }
    if (this.data.preselectedYear) {
      this.form.patchValue({ seasonYear: this.data.preselectedYear });
    }
  }

  getFlag(circuit: CircuitListDto): string {
    return circuit.location ?? '';
  }

  submit() {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const v = this.form.value;
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

    this.resultService.createGrandPrix(dto).subscribe({
      next: () => {
        this.isSubmitting = false;
        this.dialogRef.close(true);
      },
      error: () => { this.isSubmitting = false; }
    });
  }
  cancel() { this.dialogRef.close(false); }
}
