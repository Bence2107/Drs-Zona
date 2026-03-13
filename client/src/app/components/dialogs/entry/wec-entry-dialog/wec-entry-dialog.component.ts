import {Component, inject, OnInit } from '@angular/core';
import {FormArray, FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from '@angular/material/dialog';
import {MatFormField, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatButton, MatIconButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {MatTooltip} from '@angular/material/tooltip';
import {DriverLookUpDto} from '../../../../api/models/driver-look-up-dto';

export interface WecCarEntryDialogData {
  carNumber: number;
  carLabel: string;
  availableDrivers: DriverLookUpDto[];
  existingDrivers?: { driverId: string; isQualifier: boolean }[];
}

export interface WecCarEntryDialogResult {
  drivers: { driverId: string; isQualifier: boolean }[];
}

@Component({
  selector: 'app-wec-car-entry-dialog',
  imports: [
    ReactiveFormsModule,
    MatDialogTitle,
    MatDialogContent,
    MatDialogActions,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    MatButton,
    MatIconButton,
    MatIcon,
    MatTooltip,
  ],
  template: `
    <h2 mat-dialog-title>
      <mat-icon>directions_car</mat-icon>
      #{{ data.carNumber }} — Pilóták
    </h2>

    <mat-dialog-content>
      <p class="dialog-hint">Add meg az autóban lévő pilótákat. Jelöld meg, hogy ki vezette a kvalifikációs kört.</p>

      <form [formGroup]="form">
        <div formArrayName="drivers" class="driver-rows">
          @for (_ of driverRows.controls; track $index; let i = $index) {
            <div [formGroupName]="i" class="driver-row">
              <mat-form-field appearance="outline" class="driver-select">
                <mat-label>Pilóta {{ i + 1 }}</mat-label>
                <mat-select formControlName="driverId">
                  @for (d of data.availableDrivers; track d.id) {
                    <mat-option [value]="d.id" [disabled]="isSelected(d.id!, i)">
                      {{ d.name }}
                    </mat-option>
                  }
                </mat-select>
              </mat-form-field>

              <button mat-icon-button type="button"
                      [class.qualifier-active]="driverRows.at(i).get('isQualifier')!.value"
                      (click)="toggleQualifier(i)"
                      matTooltip="Ő vezette a kvalifikációs kört">
                <mat-icon>timer</mat-icon>
              </button>

              <button mat-icon-button type="button"
                      (click)="removeDriver(i)"
                      [disabled]="driverRows.length <= 1"
                      matTooltip="Eltávolítás"
                      class="remove-btn">
                <mat-icon>delete_outline</mat-icon>
              </button>
            </div>
          }
        </div>

        @if (driverRows.length < 3) {
          <button mat-button type="button" (click)="addDriver()">
            <mat-icon>add</mat-icon> Pilóta hozzáadása
          </button>
        }
      </form>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button (click)="cancel()">Mégse</button>
      <button mat-flat-button (click)="confirm()" [disabled]="form.invalid">
        <mat-icon>check</mat-icon> Mentés
      </button>
    </mat-dialog-actions>
  `,
  styles: [`
    h2 mat-icon { vertical-align: middle; margin-right: 8px; }
    .dialog-hint {
      font-size: 0.85rem;
      color: var(--text-secondary);
      margin-bottom: 1.5rem;
    }
    .driver-rows { display: flex; flex-direction: column; gap: 8px; }
    .driver-row {
      display: flex;
      align-items: center;
      gap: 8px;
    }
    .driver-select { flex: 1; }
    .qualifier-active mat-icon { color: #f59e0b !important; }
    .remove-btn { color: var(--text-secondary); }
    .remove-btn:not([disabled]):hover mat-icon { color: #ef4444; }
    ::ng-deep .mat-mdc-form-field-subscript-wrapper { display: none; }
  `]
})
export class WecCarEntryDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<WecCarEntryDialogComponent>);
  data = inject(MAT_DIALOG_DATA) as WecCarEntryDialogData;

  form!: FormGroup;

  get driverRows(): FormArray {
    return this.form.get('drivers') as FormArray;
  }

  ngOnInit() {
    this.form = this.fb.group({ drivers: this.fb.array([]) });

    const existing = this.data.existingDrivers;
    if (existing && existing.length > 0) {
      existing.forEach(e => this.addDriver(e.driverId, e.isQualifier));
    } else {
      this.addDriver();
    }
  }

  addDriver(driverId: string = '', isQualifier: boolean = false) {
    this.driverRows.push(this.fb.group({
      driverId:    [driverId, Validators.required],
      isQualifier: [isQualifier],
    }));
  }

  removeDriver(index: number) {
    this.driverRows.removeAt(index);
  }

  toggleQualifier(index: number) {
    this.driverRows.controls.forEach((c, i) => {
      c.get('isQualifier')!.setValue(i === index
        ? !c.get('isQualifier')!.value
        : false
      );
    });
  }

  isSelected(driverId: string, currentIndex: number): boolean {
    return this.driverRows.controls.some((c, i) =>
      i !== currentIndex && c.get('driverId')?.value === driverId
    );
  }

  confirm() {
    if (this.form.invalid) return;
    const result: WecCarEntryDialogResult = {
      drivers: this.driverRows.controls.map(c => c.getRawValue())
    };
    this.dialogRef.close(result);
  }

  cancel() {
    this.dialogRef.close(null);
  }
}
