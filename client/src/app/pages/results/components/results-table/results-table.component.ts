import {Component, inject} from '@angular/core';
import {AGGREGATED_ID, ALL_GP_ID, ResultsStateService} from '../results-state.service';
import {
  MatCell, MatCellDef,
  MatColumnDef,
  MatHeaderCell, MatHeaderCellDef,
  MatHeaderRow,
  MatHeaderRowDef,
  MatRow,
  MatRowDef,
  MatTable
} from '@angular/material/table';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {DatePipe} from '@angular/common';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';
import {CircuitInfoComponent} from '../circuit-info/circuit-info.component';

@Component({
  selector: 'app-results-table',
  imports: [
    MatTable,
    MatProgressSpinner,
    MatColumnDef,
    MatHeaderRow,
    MatRow,
    MatHeaderCell,
    MatCell,
    MatRowDef,
    MatHeaderRowDef,
    DatePipe,
    MatCellDef,
    MatHeaderCellDef,
    CountryFlagPipe,
    CircuitInfoComponent
  ],
  templateUrl: './results-table.component.html',
  styleUrl: './results-table.component.scss',
})
export class ResultsTableComponent {
  state = inject(ResultsStateService);
  protected readonly ALL_GP_ID = ALL_GP_ID;
  protected readonly AGGREGATED_ID = AGGREGATED_ID;
}
