import {Component, inject, OnInit, signal} from '@angular/core';
import {ActivatedRoute, Router} from '@angular/router';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
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
import {MatFormField} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {MatTooltip} from '@angular/material/tooltip';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ResultsService} from '../../../../../services/results.service';
import {SessionEditDto} from '../../../../../api/models/session-edit-dto';
import {ResultEditDto} from '../../../../../api/models/result-edit-dto';
import {SingleResultUpdateDto} from '../../../../../api/models/single-result-update-dto';
import {CustomSnackbarComponent} from '../../../../../components/custom-snackbar/custom-snackbar.component';
import {BatchResultCreateDto} from '../../../../../api/models/batch-result-create-dto';
import {GrandPrixChampionshipContextDto} from '../../../../../api/models/grand-prix-championship-context-dto';

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
    MatRowDef
  ],
  templateUrl: './entry-detail.component.html',
  styleUrl: './entry-detail.component.scss',
})
export class EntryDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private fb = inject(FormBuilder);
  private resultsService = inject(ResultsService);
  private snackBar = inject(MatSnackBar);

  gpId = signal<string>('');
  sessions = signal<string[]>([]);
  selectedSession = signal<string>('');
  sessionData = signal<SessionEditDto | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);
  context = signal<GrandPrixChampionshipContextDto | null>(null);


  editForms = signal<FormGroup[]>([]);

  statusOptions = ['Finished', 'DNF', 'DNS', 'DSQ', 'DNQ'];
  editColumns = ['position', 'driver', 'constructor', 'finishPos', 'raceTime', 'laps', 'status', 'actions'];

  ngOnInit() {
    this.gpId.set(this.route.snapshot.paramMap.get('gpId') ?? '');
    this.loadSessions();
    this.loadContext();
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
      this.snackBar.open('Kérlek javítsd a hibákat a mentés előtt!', 'OK', { duration: 3000 });
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
          startPosition: originalResult.startPosition
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
        this.snackBar.open('Hiba történt a tömeges mentés során!', 'OK', { duration: 3000 });
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
          this.snackBar.open('Hiba a kontextus betöltésekor', '', { duration: 3000 });
        }
      });
  }
}
