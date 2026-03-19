import {Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import {MatFabButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {RouterLink} from '@angular/router';
import {AuthService} from '../../services/api/auth.service';
import {MatTooltip} from '@angular/material/tooltip';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatDivider} from '@angular/material/list';
import {ResultsStateService} from './components/results-state.service';
import {ResultsFiltersComponent} from './components/results-filters/results-filters.component';
import {ResultsTableComponent} from './components/results-table/results-table.component';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [
    CommonModule, FormsModule, MatButtonToggleModule,
    MatFormFieldModule, MatSelectModule, MatTableModule, MatProgressSpinnerModule, MatIcon, RouterLink, MatFabButton, MatTooltip, MatMenuTrigger, MatMenu, MatMenuItem, MatDivider, ResultsFiltersComponent, ResultsTableComponent
  ],
  templateUrl: './results.component.html',
  styleUrls: ['./results.component.scss'],
  providers: [
    ResultsStateService
  ]
})
export class ResultsComponent implements OnInit {
  state = inject(ResultsStateService);
  private authService = inject(AuthService);

  ngOnInit() {
    this.state.loadAllSeries();
  }

  isAdmin(): boolean {
    return this.authService.currentProfile()?.role === 'Admin'
      || this.authService.currentProfile()?.role === 'Manager';
  }
}
