import {Component, computed, OnInit, signal} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import {FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon } from '@angular/material/icon';
import {
  MatCell,
  MatCellDef,
  MatColumnDef,
  MatHeaderCell,
  MatHeaderCellDef, MatHeaderRow, MatHeaderRowDef, MatRow, MatRowDef,
  MatTable,

} from '@angular/material/table';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatTooltip} from '@angular/material/tooltip';
import {MatSnackBar} from '@angular/material/snack-bar';
import {StandingsService} from '../../../../../services/api/standings.service';
import {SessionEditDto} from '../../../../../api/models/session-edit-dto';
import {ResultEditDto} from '../../../../../api/models/result-edit-dto';
import {SingleResultUpdateDto} from '../../../../../api/models/single-result-update-dto';
import {CustomSnackbarComponent} from '../../../../../components/custom-snackbar/custom-snackbar.component';
import {BatchResultCreateDto} from '../../../../../api/models/batch-result-create-dto';
import {GrandPrixChampionshipContextDto} from '../../../../../api/models/grand-prix-championship-context-dto';
import {MatCheckbox} from '@angular/material/checkbox';
import {GrandPrixDetailDto} from '../../../../../api/models/grand-prix-detail-dto';
import {SeriesLookupDto} from '../../../../../api/models/series-lookup-dto';
import {
  GrandPrixManageDialogComponent
} from '../../../../../components/dialogs/grand-prix/grand-prix-creation-dialog/grand-prix-creation-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {GrandsPrixService} from '../../../../../services/api/grands-prix.service';

@Component({
  selector: 'app-entry-detail',
  imports: [
    MatIconButton,
    MatIcon,
    MatProgressSpinner,
    MatButton,
    MatTable,
    MatHeaderCell,
    MatCell,
    MatColumnDef,
    MatCellDef,
    MatHeaderCellDef,
    MatFormField,
    MatInput,
    ReactiveFormsModule,
    MatSelect,
    MatOption,
    MatHeaderRow,
    MatRow,
    MatTooltip,
    MatHeaderRowDef,
    MatRowDef,
    MatLabel,
    FormsModule,
    MatCheckbox
  ],
  templateUrl: './entry-detail.component.html',
  styleUrl: './entry-detail.component.scss',
})
export class EntryDetailComponent implements OnInit {

  constructor(
    private dialog: MatDialog,
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private snackBar: MatSnackBar,
    private grandPrixService: GrandsPrixService,
    private resultsService: StandingsService,
  ) {}

  gpId = signal<string>('');
  grandPrix = signal<GrandPrixDetailDto | null>(null);
  seriesList = signal<SeriesLookupDto[]>([]);

  sessions = signal<string[]>([]);
  selectedSession = signal<string>('');
  sessionData = signal<SessionEditDto | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);
  context = signal<GrandPrixChampionshipContextDto | null>(null);

  editForms = signal<FormGroup[]>([]);

  statusOptions = ['Finished', 'DNF', 'DNS', 'DSQ', 'DNQ'];
  editColumns = computed(() => {
    const ctx = this.context();
    const session = this.selectedSession();

    const isF1Qualy = ctx?.pointSystem === 'F1' && session?.includes('Időmérő');
    if (isF1Qualy) {
      return ['driver', 'constructor', 'position', 'q1', 'q2', 'q3', 'lapsCompleted', 'status', 'pole', 'actions'];
    }
    return ['driver', 'constructor', 'position', 'raceTime', 'lapsCompleted', 'status', 'pole', 'fastestLap', 'actions'];
  });

  ngOnInit() {
    this.gpId.set(this.route.snapshot.paramMap.get('gpId') ?? '');
    this.loadSessions();
    this.loadContext();
    this.loadSeriesList();
  }

  private loadSeriesList() {
    this.resultsService.getAllSeries().subscribe({
      next: data => {
        const filtered = data.filter(s => {
          const name = s.name?.toLowerCase() ?? '';
          return !name.includes('wec') && !name.includes('nascar');
        });
        this.seriesList.set(filtered)
      }
    });
  }

  loadSessions() {
    this.isLoading.set(true);
    this.resultsService.getSessionsByGrandPrix(this.gpId()).subscribe({
      next: sessions => {
        this.sessions.set(sessions);
        if (sessions.length > 0) {
          this.onSessionChange(sessions[0]);
        } else {
          this.isLoading.set(false);
        }
      },
      error: () => this.isLoading.set(false)
    });
  }

  openEditGrandPrixDialog() {
    this.grandPrixService.getGrandPrixById(this.gpId()).subscribe({
      next: (gp) => {
        this.grandPrix.set(gp);
        const ref = this.dialog.open(GrandPrixManageDialogComponent, {
          width: '560px',
          data: {
            seriesList: this.seriesList(),
            editData: gp
          }
        });
        ref.afterClosed().subscribe(result => {
          if (result) {
            this.loadSessions();
            this.loadContext();
          }
        });
      },
      error: () => {
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Hiba a nagydíj betöltésekor', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      }
    });
  }

  onSessionChange(session: string) {
    this.selectedSession.set(session);
    this.isLoading.set(true);
    this.resultsService.getSessionForEdit(this.gpId(), session).subscribe({
      next: data => {
        this.sessionData.set(data);
        this.buildForms(data.results ?? []);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }
  buildForms(results: ResultEditDto[]) {
    const forms = results.map(r => {
      const isFinished = r.status === 'Finished';
      const group = this.fb.group({
        resultId:      [r.resultId],
        finishPosition:[r.finishPosition, [Validators.required, Validators.min(1), Validators.max(99)]],
        raceTime:      [{ value: r.raceTime ?? '-', disabled: !isFinished }, Validators.required],
        lapsCompleted: [r.lapsCompleted, [Validators.required, Validators.min(0)]],
        status:        [r.status, Validators.required],
        isPole:        [r.isPole],
        isFastestLap:  [r.isFastestLap],
        q1: [r.q1],
        q2: [r.q2],
        q3: [r.q3]
      });

      group.get('status')!.valueChanges.subscribe(status => {
        if (typeof status === "string") {
          this.onStatusChange(group, status);
        }
      });

      return group;
    });
    this.editForms.set(forms);
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

  saveRow(index: number) {
    const form = this.editForms()[index];
    if (form.invalid) return;
    this.isSaving.set(true);

    const v = form.getRawValue();
    const dto: SingleResultUpdateDto = {
      resultId:      v.resultId,
      finishPosition:v.finishPosition,
      raceTime:      v.raceTime,
      lapsCompleted: v.lapsCompleted,
      status:        v.status,
      isPole:        v.isPole,
      isFastestLap:  v.isFastestLap,
      q1: v.q1,
      q2: v.q2,
      q3: v.q3
    };

    this.resultsService.updateSingleResult(dto).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Eredmény frissítve', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.onSessionChange(this.selectedSession());
      },
      error: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Hiba történt', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      }
    });
  }

  saveSessionUpdateResults() {
    const currentData = this.sessionData();
    const forms = this.editForms();
    const ctx = this.context();

    const isAnyInvalid = forms.some(f => f.invalid);
    if (isAnyInvalid || !currentData) {
      this.snackBar.openFromComponent(CustomSnackbarComponent, {
        data: { message: 'Kérlek javítsd a hibákat a mentés előtt!', actionLabel: 'Rendben' },
        duration: 3000,
        horizontalPosition: 'center',
      });
      return;
    }

    this.isSaving.set(true);
    const dto: BatchResultCreateDto = {
      grandPrixId: this.gpId(),
      driversChampId: ctx?.driversChampId,
      consChampId: ctx?.consChampId,
      session: this.selectedSession(),
      results: forms.map((f, index) => {
        const v = f.getRawValue();
        const originalResult = currentData.results![index];

        return {
          driverId:      originalResult.driverId,
          constructorId: originalResult.constructorId,
          finishPosition:v.finishPosition,
          raceTime:      v.raceTime,
          lapsCompleted: v.lapsCompleted,
          status:        v.status,
          pole:          v.isPole,
          isFastestLap:  v.isFastestLap,
          q1: v.q1,
          q2: v.q2,
          q3: v.q3
        };
      })
    };

    this.resultsService.saveSessionResults(dto).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Minden módosítás mentve és újraszámolva!', actionLabel: 'Rendben' },
          duration: 3000
        });
        this.onSessionChange(this.selectedSession());
      },
      error: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Hiba a mentés után!', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      }
    });
  }

  recalculate() {
    this.isSaving.set(true);
    this.resultsService.recalculateSession(this.gpId(), this.selectedSession()).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Eredmény újraszámolva', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.onSessionChange(this.selectedSession());
      },
      error: () => {
        this.isSaving.set(false);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Hiba történt!', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      }
    });
  }

  goToCreate() {
    this.router.navigate(['/admin/results/entry', this.gpId(), 'create']);
  }

  goBack() {
    this.router.navigate(['/admin/results/entry']);
  }

  loadContext() {
      this.isLoading.set(true);
      this.resultsService.getGrandPrixContext(this.gpId()).subscribe({
        next: ctx => {
          this.context.set(ctx);
          this.loadSessions();
        },
        error: () => {
          this.isLoading.set(false);
          this.snackBar.openFromComponent(CustomSnackbarComponent, {
            data: { message: 'Hiba a kontextus betöltésekor', actionLabel: 'Rendben' },
            duration: 3000,
            horizontalPosition: 'center',
          });
        }
      });
  }
}
