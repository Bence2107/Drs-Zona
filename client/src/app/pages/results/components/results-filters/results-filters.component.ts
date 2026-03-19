import {Component, inject} from '@angular/core';
import {ResultsStateService} from '../results-state.service';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {FormsModule} from '@angular/forms';
import {CountryFlagPipe} from '../../../../pipes/country-flag.pipe';

@Component({
  selector: 'app-results-filters',
  imports: [
    MatButtonToggleGroup,
    MatFormField,
    MatLabel,
    MatSelect,
    FormsModule,
    MatButtonToggle,
    MatOption,
    CountryFlagPipe
  ],
  templateUrl: './results-filters.component.html',
  styleUrl: './results-filters.component.scss',
})
export class ResultsFiltersComponent {
  state = inject(ResultsStateService);
}
