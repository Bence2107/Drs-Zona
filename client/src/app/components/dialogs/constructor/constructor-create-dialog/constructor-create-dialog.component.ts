import {Component, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatDialogModule, MatDialogRef} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatSelectModule} from '@angular/material/select';
import {MatButtonModule} from '@angular/material/button';
import {MatIconModule} from '@angular/material/icon';
import {ConstructorsService} from '../../../../services/api/constructors.service';
import {BrandListDto} from '../../../../api/models/brand-list-dto';
import {ConstructorCreateDto} from '../../../../api/models/constructor-create-dto';
import {FormErrorService} from '../../../../services/form-error.service';
import {HttpValidationError} from '../../../../services/error-interceptor.service';
import {BrandsService} from '../../../../services/api/brands.service';

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
  brands = signal<BrandListDto[]>([]);
  isSubmitting = false;
  form!: FormGroup;

  private readonly fieldMap: { [key: string]: string } = {
    'brandid': 'brandId',
    'name': 'name',
    'nickname': 'nickname',
    'foundedyear': 'foundedYear',
    'headquarters': 'headQuarters',
    'teamchief': 'teamChief',
    'technicalchief': 'technicalChief',
    'championships': 'championships',
    'wins': 'wins',
    'podiums': 'podiums',
    'seasons': 'seasons',
  };

  constructor(
    private fb: FormBuilder,
    private dialogRef: MatDialogRef<ConstructorCreateDialogComponent>,
    private constructorService: ConstructorsService,
    private brandsService: BrandsService,
    private formErrorService: FormErrorService,
  ) {}

  ngOnInit() {
    this.form = this.fb.group({
      brandId:        ['', Validators.required],
      name:           ['', [Validators.required, Validators.maxLength(100)]],
      nickname:       ['', [Validators.required, Validators.maxLength(100)]],
      foundedYear:    [null, [Validators.required, Validators.min(1900), Validators.max(2100)]],
      headQuarters:   ['', [Validators.required, Validators.maxLength(100)]],
      teamChief:      ['', [Validators.required, Validators.maxLength(100)]],
      technicalChief: ['', [Validators.required, Validators.maxLength(100)]],
      championships:  [0, [Validators.required, Validators.min(0), Validators.max(300)]],
      wins:           [0, [Validators.required, Validators.min(0), Validators.max(500)]],
      podiums:        [0, [Validators.required, Validators.min(0), Validators.max(1000)]],
      seasons:        [1, [Validators.required, Validators.min(1), Validators.max(99)]],
    });

    this.brandsService.getAllBrands().subscribe(res => this.brands.set(res));

    this.formErrorService.clearServerErrorOnChange([
      this.form.get('brandId') as FormControl,
      this.form.get('name') as FormControl,
      this.form.get('nickname') as FormControl,
      this.form.get('foundedYear') as FormControl,
      this.form.get('headQuarters') as FormControl,
      this.form.get('teamChief') as FormControl,
      this.form.get('technicalChief') as FormControl,
      this.form.get('championships') as FormControl,
      this.form.get('wins') as FormControl,
      this.form.get('podiums') as FormControl,
      this.form.get('seasons') as FormControl,
    ]);
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
      error: (err: HttpValidationError) => {
        this.isSubmitting = false;
        this.formErrorService.applyServerErrors(this.form, err, this.fieldMap);
      }
    });
  }

  cancel() { this.dialogRef.close(false); }
}
