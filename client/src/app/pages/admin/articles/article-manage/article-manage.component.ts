import {Component, OnDestroy, OnInit} from '@angular/core';
import {MatCard, MatCardContent} from '@angular/material/card';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatError, MatFormField, MatHint, MatInput, MatLabel} from '@angular/material/input';
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatProgressSpinner} from '@angular/material/progress-spinner';
import {ActivatedRoute, Router,} from '@angular/router';
import {AngularEditorConfig, AngularEditorModule} from '@kolkov/angular-editor';
import {v4 as uuidv4} from 'uuid';
import {ArticleImageService} from '../../../../services/api/article-image.service';
import {ConfirmDialogComponent} from '../../../../components/dialogs/confirmdialog/confirm-dialog.component';
import {MatDialog} from '@angular/material/dialog';
import {ArticleService} from '../../../../services/api/article.service';
import {ArticleCreateDto} from '../../../../api/models/article-create-dto';
import {AuthService} from '../../../../services/api/auth.service';
import {switchMap} from 'rxjs';
import {ArticleUpdateDto} from '../../../../api/models/article-update-dto';
import {ArticleDetailDto} from '../../../../api/models/article-detail-dto';
import {SeriesListDto} from '../../../../api/models/series-list-dto';
import {SeriesService} from '../../../../services/api/series.service';
import {MatOption, MatSelect} from '@angular/material/select';
import {MatSnackBar} from '@angular/material/snack-bar';
import {CustomSnackbarComponent} from '../../../../components/custom-snackbar/custom-snackbar.component';
import {FormErrorService} from '../../../../services/form-error.service';
import {HttpValidationError} from '../../../../services/error-interceptor.service';

interface ArticleForm {
  title: FormControl<string>;
  lead: FormControl<string>;
  slug: FormControl<string>;
  tag: FormControl<string>;
  authorId: FormControl<string>;
  grandPrixId: FormControl<string | null>;
  isReview: FormControl<boolean>;
  firstSection: FormControl<string>;
  summary: FormGroup<{
    secondSection: FormControl<string>;
    thirdSection: FormControl<string>;
    fourthSection: FormControl<string>;
  }>;
  lastSection: FormControl<string>;
}

@Component({
  selector: 'app-article-manage',
  imports: [
    MatCard,
    MatCardContent,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatInput,
    MatSlideToggle,
    MatIcon,
    MatButton,
    MatProgressSpinner,
    AngularEditorModule,
    MatHint,
    MatSelect,
    MatOption,
    MatError
  ],
  templateUrl: './article-manage.component.html',
  styleUrl: './article-manage.component.scss',
})
export class ArticleManageComponent implements OnInit, OnDestroy {
  articleToEdit?: ArticleDetailDto | null = null;
  articleForm!: FormGroup<ArticleForm>;
  draftId = uuidv4()
  editorConfig: AngularEditorConfig = {
    editable: true,
    spellcheck: true,
    height: 'auto',
    minHeight: '200px',
    maxHeight: 'auto',
    width: 'auto',
    minWidth: '0',
    translate: 'yes',
    enableToolbar: true,
    showToolbar: true,
    placeholder: 'Írd be a szöveget ide...',
    defaultParagraphSeparator: 'p',
    defaultFontName: ' Roboto, Helvetica Neue, sans-serif;',
    defaultFontSize: '',
    fonts: [
      {class: 'arial', name: 'Arial'},
      {class: 'times-new-roman', name: 'Times New Roman'},
      {class: 'calibri', name: 'Calibri'},
    ],
    toolbarHiddenButtons: [
      ['insertImage', 'insertVideo']
    ]
  };
  
  imagePreviews: { [key: string]: string } = {};
  isLoading = false;
  private isSubmitted = false;
  isUploading: { [key: string]: boolean } = {};
  series: SeriesListDto[] = [];


  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private imageService: ArticleImageService,
    private authService: AuthService,
    private articleService: ArticleService,
    private seriesService: SeriesService,
    private snackBar: MatSnackBar,
    private formErrorService: FormErrorService
  ) {
  }

  private readonly articleFieldMap: { [key: string]: string } = {
    'title': 'title',
    'lead': 'lead',
    'slug': 'slug',
    'tag': 'tag',
    'firstsection': 'firstSection',
    'lastsection': 'lastSection',
  };

  private readonly summaryFieldMap: { [key: string]: string } = {
    'secondsection': 'secondSection',
    'thirdsection': 'thirdSection',
    'fourthsection': 'fourthSection',
  };

  private buildCreatePayLoad(): ArticleCreateDto {
    const userId = this.authService.currentProfile()?.userId;
    const raw = this.articleForm.getRawValue();


    return {
      title: raw.title,
      lead: raw.lead,
      slug: raw.slug,
      tag: raw.tag,
      authorId: userId,
      grandPrixId: raw.grandPrixId || null,
      isReview: raw.isReview,
      firstSection: raw.firstSection,
      lastSection: raw.lastSection,
      ...(raw.isReview ? {summary: raw.summary} : {}),
    };
  }

  private buildUpdatePayLoad(): ArticleUpdateDto {
    const raw = this.articleForm.getRawValue();

    return {
      id: this.articleToEdit!.id,
      title: raw.title,
      lead: raw.lead,
      slug: raw.slug,
      grandPrixId: raw.grandPrixId || null,
      isReview: raw.isReview,
      firstSection: raw.firstSection,
      lastSection: raw.lastSection,
      ...(raw.isReview ? {summary: raw.summary} : {}),
    };
  }

  private initEmptyForm() {
    this.articleForm = this.fb.group<ArticleForm>({
      title: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.minLength(5),
        Validators.maxLength(200)
      ]),
      lead: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.minLength(20),
        Validators.maxLength(500)
      ]),
      slug: this.fb.nonNullable.control('', [
        Validators.minLength(5),
        Validators.maxLength(200)
      ]),
      tag: this.fb.nonNullable.control<string>('', Validators.required),
      authorId: this.fb.nonNullable.control('admin'),
      grandPrixId: this.fb.control(null),
      isReview: this.fb.nonNullable.control(false),
      firstSection: this.fb.nonNullable.control('', [
        Validators.required,
        Validators.minLength(100)
      ]),
      summary: this.fb.group({
        secondSection: this.fb.nonNullable.control(''),
        thirdSection: this.fb.nonNullable.control(''),
        fourthSection: this.fb.nonNullable.control(''),
      }),
      lastSection: this.fb.nonNullable.control('', [
        Validators.minLength(100)
      ]),
    });

    this.formErrorService.clearServerErrorOnChange([
      this.articleForm.get('title') as FormControl,
      this.articleForm.get('lead') as FormControl,
      this.articleForm.get('slug') as FormControl,
      this.articleForm.get('tag') as FormControl,
      this.articleForm.get('firstSection') as FormControl,
      this.articleForm.get('lastSection') as FormControl,
      this.articleForm.get('summary.secondSection') as FormControl,
      this.articleForm.get('summary.thirdSection') as FormControl,
      this.articleForm.get('summary.fourthSection') as FormControl,
    ]);
  }

  private loadArticleData(article: ArticleDetailDto) {
    this.articleForm.patchValue({
      title: article.title!,
      tag: article.tag!,
      lead: article.lead!,
      slug: article.slug!,
      isReview: article.isReview,
      firstSection: article.firstSection!,
      lastSection: article.lastSection!,
      summary: {
        secondSection: article.middleSections?.[0] || '',
        thirdSection: article.middleSections?.[1] || '',
        fourthSection: article.middleSections?.[2] || ''
      }
    });

    this.imagePreviews = {
      primary: article.primaryImageUrl ? `${article.primaryImageUrl}?t=${new Date().getTime()}` : '',
      secondary: article.secondaryImageUrl ? `${article.secondaryImageUrl}?t=${new Date().getTime()}` : '',
      third: article.thirdImageUrl ? `${article.thirdImageUrl}?t=${new Date().getTime()}` : '',
      last: article.lastImageUrl ? `${article.lastImageUrl}?t=${new Date().getTime()}` : ''
    };
  }

  get isReview(): boolean {
    return this.articleForm.get('isReview')?.value ?? false;
  }

  ngOnInit(): void {
    this.seriesService.getSeriesList().subscribe(data => this.series = data);

    this.initEmptyForm();

    const slug = this.route.snapshot.paramMap.get('slug');
    if (slug) {
      this.articleService.getBySlug(slug).subscribe(article => {
        this.articleToEdit = article;
        this.draftId = article.id!;
        this.loadArticleData(article);
      });
    }

    this.articleForm.get('isReview')?.valueChanges.subscribe(isReview => {
      const summary = this.articleForm.get('summary') as FormGroup;

      if (isReview) {
        summary.get('secondSection')?.setValidators(Validators.required);
        summary.get('thirdSection')?.setValidators(Validators.required);
        summary.get('fourthSection')?.setValidators(Validators.required);
      } else {
        summary.get('secondSection')?.clearValidators();
        summary.get('thirdSection')?.clearValidators();
        summary.get('fourthSection')?.clearValidators();
      }

      summary.get('secondSection')?.updateValueAndValidity();
      summary.get('thirdSection')?.updateValueAndValidity();
      summary.get('fourthSection')?.updateValueAndValidity();
    });
  }

  ngOnDestroy(): void {
    if (!this.isSubmitted) {
      this.imageService.deleteDraft(this.draftId).subscribe({
        next: () => {
          this.isLoading = false;
          this.router.navigate(['/news']);
        },
        error: (err) => {
          console.error(err);
          this.isLoading = false;
          this.router.navigate(['/news']);
        }
      });
    }
  }

  onCancel(): void {
    if (Object.keys(this.imagePreviews).length === 0 && this.articleForm.pristine) {
      this.router.navigate(['/news']);
      return;
    }

    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '400px',
      data: {
        title: 'Művelet megszakítása',
        message: 'Biztosan kilépsz? Minden feltöltött kép és beírt adat elvész.',
        confirmText: 'Igen, kilépek',
        cancelText: 'Mégse'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.isLoading = true;
        this.imageService.deleteDraft(this.draftId).subscribe({
          next: () => {
            this.isLoading = false;
            this.router.navigate(['news']);
          },
          error: (err) => {
            console.error(err);
            this.isLoading = false;
            this.router.navigate(['news']);
          }
        });
      }
    });
  }

  onEditorBlur(controlPath: string) {
    const control = this.articleForm.get(controlPath);
    if (control) {
      control.markAsTouched();
      control.updateValueAndValidity();
    }
  }

  onFileSelected(event: any, slot: string) {
    const file: File = event.target.files[0];
    if (!file) return;

    this.isUploading[slot] = true;

    this.imageService.uploadDraftImage(this.draftId, slot, file).subscribe({
      next: (url: string) => {
        this.imagePreviews[slot] = `${url}?t=${new Date().getTime()}`;
        this.isUploading[slot] = false;
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Kép sikeresen feltöltve', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      },
      error: () => {
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Hiba a feltöltés során!', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.isUploading[slot] = false;
      }
    });
  }

  onSubmit(): void {
    this.articleForm.markAllAsTouched();
    if (this.articleForm.invalid) return;
    this.isLoading = true;
    const payload = this.articleToEdit
      ? this.buildUpdatePayLoad()
      : this.buildCreatePayLoad()

    const action$ = this.articleToEdit
      ? this.articleService.update(payload)
      : this.articleService.create(payload);

    action$.pipe(
      switchMap(() => this.imageService.promoteDraftImages(this.draftId, payload.slug!))
    ).subscribe({
      next: () => {
        this.isSubmitted = true;
        const version = new Date().getTime();
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Profilkép sikeresen feltöltve', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        window.location.href = `/article/${payload.slug}?v=${version}`;
      },
      error: (err: HttpValidationError) => {
        this.isLoading = false;

        // Summary nested group külön kezelése
        const summaryGroup = this.articleForm.get('summary') as FormGroup;
        const summaryErrors: HttpValidationError = {
          title: err.title,
          fieldErrors: {}
        };
        const rootErrors: HttpValidationError = {
          title: err.title,
          fieldErrors: {}
        };

        for (const field of Object.keys(err.fieldErrors ?? {})) {
          if (field in this.summaryFieldMap) {
            summaryErrors.fieldErrors[field] = err.fieldErrors[field];
          } else {
            rootErrors.fieldErrors[field] = err.fieldErrors[field];
          }
        }

        this.formErrorService.applyServerErrors(this.articleForm, rootErrors, this.articleFieldMap);
        this.formErrorService.applyServerErrors(summaryGroup, summaryErrors, this.summaryFieldMap);
      }
    });
  }
}
