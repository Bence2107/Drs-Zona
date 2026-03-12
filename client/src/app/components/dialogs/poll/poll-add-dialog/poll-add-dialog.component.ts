import {Component, OnInit} from '@angular/core';
import {FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {PollService} from '../../../../services/poll.service';
import {
  MatDialogActions,
  MatDialogClose,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from "@angular/material/dialog";
import {MatError, MatFormField, MatHint, MatInput, MatLabel} from '@angular/material/input';
import {MatDatepicker, MatDatepickerInput, MatDatepickerToggle} from '@angular/material/datepicker';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {PollCreateDto} from '../../../../api/models/poll-create-dto';
import {AuthService} from '../../../../services/auth.service';
import {SeriesListDto} from '../../../../api/models/series-list-dto';
import {SeriesService} from '../../../../services/series.service';
import {MatOption, MatSelect} from '@angular/material/select';

@Component({
  selector: 'app-poll-add-dialog',
  imports: [
    MatDialogTitle,
    MatDialogContent,
    MatFormField,
    MatLabel,
    MatError,
    MatInput,
    FormsModule,
    MatDatepickerInput,
    MatDatepickerToggle,
    MatDatepicker,
    MatIconButton,
    MatIcon,
    MatDialogActions,
    MatButton,
    MatDialogClose,
    ReactiveFormsModule,
    MatHint,
    MatSelect,
    MatOption
  ],
  templateUrl: './poll-add-dialog.component.html',
  styleUrl: './poll-add-dialog.component.scss',
})
export class PollAddDialogComponent implements OnInit {
  pollForm: FormGroup;
  minDate = new Date();
  series: SeriesListDto[] = [];

  constructor(
    private fb: FormBuilder,
    private pollService: PollService,
    private authService: AuthService,
    private seriesService: SeriesService,
    private dialogRef: MatDialogRef<PollAddDialogComponent>
  ) {
    this.pollForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(100)]],
      tag: ['', [Validators.required]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      expiresAt: [null, [Validators.required]],
      options: this.fb.array([
        this.fb.control('', [Validators.required, Validators.minLength(2)]),
        this.fb.control('', [Validators.required, Validators.minLength(2)])
      ])
    });
  }

  ngOnInit(): void {
    this.seriesService.getSeriesList().subscribe(data => this.series = data);
  }

  get options() {
    return this.pollForm.get('options') as FormArray;
  }

  addOption() {
    if (this.options.length < 10) {
      this.options.push(this.fb.control('', [Validators.required, Validators.minLength(2)]));
    }
  }

  removeOption(index: number) {
    if (this.options.length > 2) {
      this.options.removeAt(index);
    }
  }

  submit() {
    const userId = this.authService.currentProfile()?.userId;

    if (this.pollForm.valid && userId) {
      const pollDto: PollCreateDto = {
        title: this.pollForm.value.title,
        tag: this.pollForm.value.tag,
        description: this.pollForm.value.description,
        expiresAt: new Date(this.pollForm.value.expiresAt).toISOString(),
        options: this.pollForm.value.options
      };

      this.pollService.createPoll(pollDto, userId).subscribe({
        next: () => {
          this.dialogRef.close(true);
        }
      });
    } else {
      this.pollForm.markAllAsTouched();
    }
  }
}
