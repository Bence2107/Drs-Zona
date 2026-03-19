import {Component, inject, OnInit, signal} from '@angular/core';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from "@angular/forms";
import {
  MAT_DIALOG_DATA,
  MatDialogActions,
  MatDialogContent,
  MatDialogRef,
  MatDialogTitle
} from '@angular/material/dialog';
import {ConstructorDetailDto} from '../../../../api/models/constructor-detail-dto';
import {ConstructorsService} from '../../../../services/constructors.service';
import {BrandListDto} from '../../../../api/models/brand-list-dto';
import {ConstructorUpdateDto} from '../../../../api/models/constructor-update-dto';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatButton} from '@angular/material/button';
import {MatIcon} from '@angular/material/icon';
import {FormErrorService} from '../../../../services/form-error.service';
import {HttpValidationError} from '../../../../services/error-interceptor.service';
import {BrandsService} from '../../../../services/brands.service';

@Component({
  selector: 'app-constructor-edit-dialog',
  imports: [
    MatDialogTitle,
    MatDialogContent,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatSelect,
    MatOption,
    MatInput,
    MatError,
    MatDialogActions,
    MatButton,
    MatIcon
  ],
  templateUrl: './constructor-edit-dialog.component.html',
  styleUrl: './constructor-edit-dialog.component.scss',
})
class ConstructorEditDialogComponent implements OnInit{
  data = inject(MAT_DIALOG_DATA) as { constructor: ConstructorDetailDto };
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
    private dialogRef: MatDialogRef<ConstructorEditDialogComponent>,
    private fb: FormBuilder,
    private brandsService: BrandsService,
    private constructorService: ConstructorsService,
    private formErrorService: FormErrorService,
  ) {}

  ngOnInit() {
    const c = this.data.constructor;
    this.form = this.fb.group({
      brandId:        [c.brandId, Validators.required],
      name:           [c.name, [Validators.required, Validators.maxLength(100)]],
      nickname:       [c.nickname, [Validators.required, Validators.maxLength(100)]],
      foundedYear:    [c.foundedYear, [Validators.required, Validators.min(1900), Validators.max(2100)]],
      headQuarters:   [c.headQuarters, Validators.required],
      teamChief:      [c.teamChief, Validators.required],
      technicalChief: [c.technicalChief, Validators.required],
      championships:  [c.championships, [Validators.required, Validators.min(0)]],
      wins:           [c.totalWins, [Validators.required, Validators.min(0)]],
      podiums:        [c.totalPodiums, [Validators.required, Validators.min(0)]],
      seasons:        [c.seasons, [Validators.required, Validators.min(1), Validators.max(99)]],
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
    const dto: ConstructorUpdateDto = {
      id:             this.data.constructor.id,
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

    this.constructorService.updateConstructor(dto).subscribe({
      next: () => { this.isSubmitting = false; this.dialogRef.close(true); },
      error: (err: HttpValidationError) => {
        this.isSubmitting = false;
        this.formErrorService.applyServerErrors(this.form, err, this.fieldMap);
      }
    });
  }

  cancel() { this.dialogRef.close(false); }
}

export default ConstructorEditDialogComponent
