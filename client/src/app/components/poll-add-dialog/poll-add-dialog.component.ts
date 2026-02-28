import { Component } from '@angular/core';
import {FormArray, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators} from '@angular/forms';
import {PollService} from '../../services/poll.service';
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
import {PollCreateDto} from '../../api/models/poll-create-dto';
import {AuthService} from '../../services/auth.service';

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
    MatHint
  ],
  templateUrl: './poll-add-dialog.component.html',
  styleUrl: './poll-add-dialog.component.scss',
})
export class PollAddDialogComponent {
  pollForm: FormGroup;
  minDate = new Date();

  constructor(
    private fb: FormBuilder,
    private pollService: PollService,
    private authService: AuthService,
    private dialogRef: MatDialogRef<PollAddDialogComponent>
  ) {
    this.pollForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(100)]],
      description: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(500)]],
      expiresAt: [null, [Validators.required]],
      options: this.fb.array([
        this.fb.control('', [Validators.required, Validators.minLength(2)]),
        this.fb.control('', [Validators.required, Validators.minLength(2)])
      ])
    });
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
        description: this.pollForm.value.description,
        expiresAt: new Date(this.pollForm.value.expiresAt).toISOString(),
        options: this.pollForm.value.options
      };

      this.pollService.createPoll(pollDto, userId).subscribe({
        next: (response) => {
          console.log('Sikeres mentés:', response);
          this.dialogRef.close(true);
        },
        error: (err) => {
          console.error('Szerver hiba:', err);
        }
      });
    } else {
      this.pollForm.markAllAsTouched();
    }
  }
}
