import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {ConstructorsService} from '../../../../services/constructors.service';
import {BrandListDto} from '../../../../api/models/brand-list-dto';
import {ConstructorCreateDto} from '../../../../api/models/constructor-create-dto';

@Component({
  selector: 'app-constructor-create-dialog',
  standalone: true,
  imports: [
    ReactiveFormsModule, MatDialogModule, MatFormFieldModule,
    MatInputModule, MatSelectModule, MatButtonModule, MatIconModule
  ],
  templateUrl: './constructor-create-dialog.component.html',
  styleUrl: './constructor-create-dialog.component.scss',
})
export class ConstructorCreateDialogComponent implements OnInit {
  private fb = inject(FormBuilder);
  private dialogRef = inject(MatDialogRef<ConstructorCreateDialogComponent>);
  private constructorService = inject(ConstructorsService);

  brands = signal<BrandListDto[]>([]);
  isSubmitting = false;
  form!: FormGroup;

  ngOnInit() {
    this.form = this.fb.group({
      brandId:       ['', Validators.required],
      name:          ['', [Validators.required, Validators.maxLength(100)]],
      nickname:      ['', [Validators.required, Validators.maxLength(100)]],
      foundedYear:   [null, [Validators.required, Validators.min(1900), Validators.max(2100)]],
      headQuarters:  ['', Validators.required],
      teamChief:     ['', Validators.required],
      technicalChief:['', Validators.required],
      championships: [0, [Validators.required, Validators.min(0)]],
      wins:          [0, [Validators.required, Validators.min(0)]],
      podiums:       [0, [Validators.required, Validators.min(0)]],
      seasons:       [1, [Validators.required, Validators.min(1), Validators.max(99)]],
    });

    this.constructorService.getAllBrands().subscribe(res => this.brands.set(res));
  }

  submit() {
    if (this.form.invalid) return;
    this.isSubmitting = true;

    const v = this.form.value;
    const dto: ConstructorCreateDto = {
      brandId:        v.brandId,
      name:           v.name,
      nickname:       v.nickname,
      foundedYear:    v.foundedYear,
      headQuarters:   v.headQuarters,
      teamChief:      v.teamChief,
      technicalChief: v.technicalChief,
      championships:  v.championships,
      wins:           v.wins,
      podiums:        v.podiums,
      seasons:        v.seasons,
    };

    this.constructorService.createConstructor(dto).subscribe({
      next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
      error: () => { this.isSubmitting = false; }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
