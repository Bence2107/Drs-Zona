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
import {MatDialog} from '@angular/material/dialog';
import {
  WecCarEntryDialogComponent, WecCarEntryDialogData,
  WecCarEntryDialogResult
} from '../../../../../components/dialogs/entry/wec-entry-dialog/wec-entry-dialog.component';
import {WecBatchResultCreateDto} from '../../../../../api/models/wec-batch-result-create-dto';

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
  private dialog = inject(MatDialog);

  gpId = signal<string>('');
  context = signal<GrandPrixChampionshipContextDto | null>(null);
  selectedSession = signal<string>('');
  isLoadingContext = signal(true);
  isSaving = signal(false);

  drivers = signal<DriverLookUpDto[]>([]);
  constructors = signal<ConstructorLookUpDto[]>([]);

  wecCarDrivers = signal<Map<number, WecCarEntryDialogResult>>(new Map());

  dataSource = signal<AbstractControl[]>([]);
  form!: FormGroup;

  statusOptions = ['Finished', 'DNF', 'DNS', 'DSQ', 'DNQ'];

  displayedColumns = ['rowNum', 'driverId', 'constructorId', 'finishPosition', 'raceTime', 'laps', 'status', 'pole', 'remove'];
  wecColumns = ['rowNum', 'carNumber', 'carLabel', 'constructorId', 'finishPosition', 'raceTime', 'laps', 'status', 'pole', 'drivers', 'remove'];

  get rows(): FormArray {
    return this.form.get('rows') as FormArray;
  }

  get isWec(): boolean {
    return this.context()?.pointSystem === 'WEC';
  }

  get activeColumns(): string[] {
    return this.isWec ? this.wecColumns : this.displayedColumns;
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
        const sessions = ctx.availableSessions?.map(s => s.name) ?? [];
        if (sessions.length > 0) {
          this.selectedSession.set(sessions[0]!);
        }
        this.loadLookups(ctx.driversChampId ?? null, ctx.consChampId!);
        this.isLoadingContext.set(false);
        this.addRow();
      },
      error: () => {
        this.isLoadingContext.set(false);
        this.snackBar.open('Nem sikerült betölteni a nagydíj adatait.', '', { duration: 3000 });
      }
    });
  }

  loadLookups(driverChampId: string | null, consChampId: string) {
    if (driverChampId) {
      this.resultsService.getDriversByDriversChampionship(driverChampId).subscribe(list => this.drivers.set(list));
    }
    this.resultsService.getConstructorsByConstChampionship(consChampId).subscribe(list => this.constructors.set(list));

    if (this.rows.length === 0) this.addRow();
  }

  addRow() {
    const group = this.isWec
      ? this.fb.group({
        carNumber:     [this.rows.length + 1, [Validators.required, Validators.min(1)]],
        carLabel:      ['', Validators.required],
        constructorId: ['', Validators.required],
        finishPosition:[this.rows.length + 1, [Validators.required, Validators.min(1), Validators.max(99)]],
        raceTime:      ['', Validators.required],
        lapsCompleted: [0, [Validators.required, Validators.min(0)]],
        status:        ['Finished', Validators.required],
        pole:          [false],
      })
      : this.fb.group({
        driverId:      ['', Validators.required],
        constructorId: ['', Validators.required],
        finishPosition:[this.rows.length + 1, [Validators.required, Validators.min(1), Validators.max(99)]],
        raceTime:      ['', Validators.required],
        lapsCompleted: [0, [Validators.required, Validators.min(0)]],
        status:        ['Finished', Validators.required],
        pole:          [false],
      });

    (group as FormGroup).get('status')!.valueChanges.subscribe((status: string) => {
      this.onStatusChange(group as FormGroup, status);
    });

    if (!this.isWec) {
      (group as FormGroup).get('driverId')!.valueChanges.subscribe((selectedDriverId: string) => {
        if (selectedDriverId) {
          const driverData = this.drivers().find(d => d.id === selectedDriverId);
          if (driverData?.constructorId) {
            (group as FormGroup).get('constructorId')!.setValue(driverData.constructorId);
          }
        }
      });
    }

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

  openWecDriverDialog(index: number) {
    const row = this.getRowGroup(index).getRawValue();
    const existing = this.wecCarDrivers().get(index);

    const dialogData: WecCarEntryDialogData = {
      carNumber: row.carNumber,
      carLabel: row.carLabel || `#${row.carNumber}`,
      availableDrivers: this.drivers(),
      existingDrivers: existing?.drivers ?? []
    };

    const ref = this.dialog.open(WecCarEntryDialogComponent, {
      width: '480px',
      data: dialogData
    });

    ref.afterClosed().subscribe((result: WecCarEntryDialogResult | null) => {
      if (!result) return;
      const map = new Map(this.wecCarDrivers());
      map.set(index, result);
      this.wecCarDrivers.set(map);
    });
  }

  getWecDriverCount(index: number): number {
    return this.wecCarDrivers().get(index)?.drivers.length ?? 0;
  }

  submit() {
    if (this.form.invalid || !this.context() || !this.selectedSession()) return;
    const ctx = this.context()!;
    this.isSaving.set(true);

    if (this.isWec) {
      this.submitWec(ctx);
    } else {
      this.submitStandard(ctx);
    }
  }

  private submitStandard(ctx: GrandPrixChampionshipContextDto) {
    const dto: BatchResultCreateDto = {
      grandPrixId:    this.gpId(),
      driversChampId: ctx.driversChampId!,
      consChampId:    ctx.consChampId!,
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
      next: () => this.onSaveSuccess(),
      error: () => this.onSaveError()
    });
  }

  private submitWec(ctx: GrandPrixChampionshipContextDto) {
    const dto: WecBatchResultCreateDto = {
      grandPrixId: this.gpId(),
      consChampId: ctx.consChampId!,
      session:     this.selectedSession(),
      results: this.rows.controls.map((c, i) => {
        const raw = (c as FormGroup).getRawValue();
        const carDrivers = this.wecCarDrivers().get(i);
        return {
          constructorId: raw.constructorId,
          carNumber:     raw.carNumber,
          carLabel:      raw.carLabel,
          finishPosition:raw.finishPosition,
          raceTime:      raw.raceTime,
          lapsCompleted: raw.lapsCompleted,
          status:        raw.status,
          pole:          raw.pole,
          startPosition: 0,
          drivers:       carDrivers?.drivers ?? []
        };
      })
    };

    this.resultsService.insertWecResults(dto).subscribe({
      next: () => this.onSaveSuccess(),
      error: () => this.onSaveError()
    });
  }

  private onSaveSuccess() {
    this.isSaving.set(false);
    this.snackBar.openFromComponent(CustomSnackbarComponent, {
      data: { message: 'Eredmények mentve!', actionLabel: 'Rendben' },
      duration: 3000,
      horizontalPosition: 'center',
    });
    this.router.navigate(['/admin/results/entry', this.gpId()]);
  }

  private onSaveError() {
    this.isSaving.set(false);
    this.snackBar.openFromComponent(CustomSnackbarComponent, {
      data: { message: 'Hiba mentés során', actionLabel: 'Rendben' },
      duration: 3000,
      horizontalPosition: 'center',
    });
  }

  removeRow(index: number) {
    this.rows.removeAt(index);
    // WEC map újraindexelése
    if (this.isWec) {
      const oldMap = this.wecCarDrivers();
      const newMap = new Map<number, WecCarEntryDialogResult>();
      oldMap.forEach((v, k) => {
        if (k < index) newMap.set(k, v);
        else if (k > index) newMap.set(k - 1, v);
      });
      this.wecCarDrivers.set(newMap);
    }
    this.dataSource.set([...this.rows.controls]);
  }

  getRowGroup(index: number): FormGroup {
    return this.rows.at(index) as FormGroup;
  }

  goBack() {
    this.router.navigate(['/admin/results/entry', this.gpId()]);
  }

  isDriverSelected(driverId: string, currentIndex: number): boolean {
    return this.rows.controls.some((control, index) =>
      index !== currentIndex && control.get('driverId')?.value === driverId
    );
  }
}
