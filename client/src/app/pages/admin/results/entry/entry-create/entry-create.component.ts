import {Component, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {
  AbstractControl,
  FormArray,
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import {ResultsService} from '../../../../../services/results.service';
import {MatSnackBar} from '@angular/material/snack-bar';
import {GrandPrixChampionshipContextDto} from '../../../../../api/models/grand-prix-championship-context-dto';
import {BatchResultCreateDto} from '../../../../../api/models/batch-result-create-dto';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatTooltip} from '@angular/material/tooltip';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef,
  MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable
} from '@angular/material/table';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatOption} from '@angular/material/core';
import {MatSelect} from '@angular/material/select';
import {DriverLookUpDto} from '../../../../../api/models/driver-look-up-dto';
import {ConstructorLookUpDto} from '../../../../../api/models/constructor-look-up-dto';
import {CustomSnackbarComponent} from '../../../../../components/custom-snackbar/custom-snackbar.component';

@Component({
  selector: 'app-entry-create',
  imports: [
    MatIconButton,
    MatIcon,
    MatProgressSpinner,
    MatTooltip,
    MatButton,
    ReactiveFormsModule,
    MatTable,
    MatColumnDef,
    MatHeaderCell,
    MatCell,
    MatFormField,
    MatInput,
    MatCellDef,
    MatHeaderCellDef,
    MatOption,
    MatSelect,
    MatHeaderRow,
    MatRow,
    MatRowDef,
    MatHeaderRowDef,
    MatLabel,
    FormsModule
  ],
  templateUrl: './entry-create.component.html',
  styleUrl: './entry-create.component.scss',
})
export class EntryCreateComponent implements OnInit{
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private resultsService = inject(ResultsService);
  private snackBar = inject(MatSnackBar);

  gpId = signal<string>('');
  context = signal<GrandPrixChampionshipContextDto | null>(null);
  selectedSession = signal<string>('');
  isLoadingContext = signal(true);
  isSaving = signal(false);

  drivers = signal<DriverLookUpDto[]>([]);
  constructors = signal<ConstructorLookUpDto[]>([]);

  dataSource = signal<AbstractControl[]>([]);

  form!: FormGroup;

  statusOptions = ['Finished', 'DNF', 'DNS', 'DSQ', 'DNQ'];
  displayedColumns = ['rowNum', 'driverId', 'constructorId', 'finishPosition', 'raceTime', 'laps', 'status', 'pole', 'remove'];

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  ngOnInit() {
    this.gpId.set(this.route.snapshot.paramMap.get('gpId') ?? '');
    this.form = this.fb.group({ rows: this.fb.array([]) });
    this.loadContext();
  }

  loadContext() {
    this.isLoadingContext.set(true);
    this.resultsService.getGrandPrixContext(this.gpId()).subscribe({
      next: ctx => {
        this.context.set(ctx);
        if (ctx.availableSessions) {
          this.selectedSession.set(ctx.availableSessions[0] ?? '');
        }
        if (ctx.driversChampId != null && ctx.consChampId != null) {
            this.loadLookups(ctx.driversChampId, ctx.consChampId);
        }
        this.isLoadingContext.set(false);
        this.addRow();
      },
      error: () => {
        this.isLoadingContext.set(false);
        this.snackBar.open('Nem sikerült betölteni a nagydíj adatait.', '', { duration: 3000 });
      }
    });
  }

  addRow() {
    const group = this.fb.group({
      driverId:      ['', Validators.required],
      constructorId: ['', Validators.required],
      finishPosition:[this.rows.length + 1, [Validators.required, Validators.min(1), Validators.max(99)]],
      raceTime:      ['', Validators.required],
      lapsCompleted: [0, [Validators.required, Validators.min(0)]],
      status:        ['Finished', Validators.required],
      pole:          [false],
    });

    group.get('status')!.valueChanges.subscribe(status => {
      if (typeof status === "string") {
        this.onStatusChange(group, status);
      }
    });

    group.get('driverId')!.valueChanges.subscribe(selectedDriverId => {
      if (selectedDriverId) {
        const driverData = this.drivers().find(d => d.id === selectedDriverId);

        if (driverData && driverData.constructorId) {
          group.get('constructorId')!.setValue(driverData.constructorId);
        }
      }
    });

    this.rows.push(group);
    this.dataSource.set([...this.rows.controls]);
  }

  onStatusChange(group: FormGroup, status: string) {
    const raceTimeCtrl = group.get('raceTime')!;
    if (status !== 'Finished') {
      raceTimeCtrl.setValue('-');
      raceTimeCtrl.disable();
    } else {
      raceTimeCtrl.setValue('');
      raceTimeCtrl.enable();
    }
  }

  submit() {
    if (this.form.invalid || !this.context() || !this.selectedSession()) return;
    const ctx = this.context()!;

    this.isSaving.set(true);
    const dto: BatchResultCreateDto = {
      grandPrixId:    this.gpId(),
      driversChampId: ctx.driversChampId,
      consChampId:    ctx.consChampId,
      session:        this.selectedSession(),
      results: this.rows.controls.map(c => {
        const raw = (c as FormGroup).getRawValue();
        return {
          driverId:      raw.driverId,
          constructorId: raw.constructorId,
          finishPosition:raw.finishPosition,
          raceTime:      raw.raceTime,
          lapsCompleted: raw.lapsCompleted,
          status:        raw.status,
          pole:          raw.pole,
          startPosition: 0
        };
      })
    };

    this.resultsService.insertResults(dto).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Eredmények mentve!', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.router.navigate(['/admin/results/entry', this.gpId()]);
      },
      error: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Hiba mentés során', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      }
    });
  }

  removeRow(index: number) {
    this.rows.removeAt(index);
    this.dataSource.set([...this.rows.controls]);
  }

  getRowGroup(index: number): FormGroup {
    return this.rows.at(index) as FormGroup;
  }

  goBack() {
    this.router.navigate(['/admin/results/entry', this.gpId()]);
  }

  loadLookups(driverChampId: string, consChampId: string) {
    this.resultsService.getDriversByDriversChampionship(driverChampId).subscribe(list => this.drivers.set(list));
    this.resultsService.getConstructorsByConstChampionship(consChampId).subscribe(list => this.constructors.set(list));

    if (this.rows.length === 0) this.addRow();
  }

  isDriverSelected(driverId: string, currentIndex: number): boolean {
    return this.rows.controls.some((control, index) => {
      return index !== currentIndex && control.get('driverId')?.value === driverId;
    });
  }
}
