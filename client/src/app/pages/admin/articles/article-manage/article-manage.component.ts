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
import {ArticleImageService} from '../../../../services/article-image.service';
import {ConfirmDialogComponent} from '../../../../components/dialogs/confirmdialog/confirmdialog.component';
import {MatDialog} from '@angular/material/dialog';
import {ArticleService} from '../../../../services/article.service';
import {ArticleCreateDto} from '../../../../api/models/article-create-dto';
import {AuthService} from '../../../../services/auth.service';
import {switchMap} from 'rxjs';
import {ArticleUpdateDto} from '../../../../api/models/article-update-dto';
import {ArticleDetailDto} from '../../../../api/models/article-detail-dto';
import {SeriesListDto} from '../../../../api/models/series-list-dto';
import {SeriesService} from '../../../../services/series.service';
import {MatOption, MatSelect} from '@angular/material/select';

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
  isLoading = false;
  errorMessage = '';

  series: SeriesListDto[] = [];
  private isSubmitted = false;

  get isReview(): boolean {
    return this.articleForm.get('isReview')?.value ?? false;
  }

  draftId = uuidv4()
  imagePreviews: { [key: string]: string } = {};
  isUploading: { [key: string]: boolean } = {};

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private dialog: MatDialog,
    private imageService: ArticleImageService,
    private authService: AuthService,
    private articleService: ArticleService,
    private seriesService: SeriesService
  ) {
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

  private initEmptyForm() {
    this.articleForm = this.fb.group<ArticleForm>({
      title: this.fb.nonNullable.control(''),
      lead: this.fb.nonNullable.control(''),
      slug: this.fb.nonNullable.control(''),
      tag: this.fb.nonNullable.control<string>(''),
      authorId: this.fb.nonNullable.control('admin'),
      grandPrixId: this.fb.control(null),
      isReview: this.fb.nonNullable.control(false),
      firstSection: this.fb.nonNullable.control(''),
      summary: this.fb.group({
        secondSection: this.fb.nonNullable.control(''),
        thirdSection: this.fb.nonNullable.control(''),
        fourthSection: this.fb.nonNullable.control(''),
      }),
      lastSection: this.fb.nonNullable.control(''),
    });
  }

  private loadArticleData(article: ArticleDetailDto) {
    this.articleForm.patchValue({
      title: article.title!,
      lead: article.lead!,
      slug: article.slug!,
      grandPrixId: article.grandPrixId,
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
    defaultFontName: 'Arial',
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

  onSubmit(): void {
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
        window.location.href = `/article/${payload.slug}?v=${version}`;
      },
      error: () => { this.errorMessage = 'Hiba történt.'; this.isLoading = false; }
    });
  }

  onFileSelected(event: any, slot: string) {
    const file: File = event.target.files[0];
    if (!file) return;

    this.isUploading[slot] = true;

    this.imageService.uploadDraftImage(this.draftId, slot, file).subscribe({
      next: (url: string) => {
        this.imagePreviews[slot] = `${url}?t=${new Date().getTime()}`;
        this.isUploading[slot] = false;
      },
      error: (err) => {
        console.error(err);
        this.isUploading[slot] = false;
      }
    });
  }

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

  onEditorBlur(controlPath: string) {
    const control = this.articleForm.get(controlPath);
    if (control) {
      control.updateValueAndValidity();
      control.markAsTouched();
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
}
