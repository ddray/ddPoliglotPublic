import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { LayoutModule } from '@angular/cdk/layout';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ArticlesComponent } from './components/articles/articles.component';
import { ArticleComponent } from './components/article/article.component';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// tslint:disable-next-line:max-line-length
import { MatInputModule } from '@angular/material/input';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatButtonModule } from '@angular/material/button';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatTableModule } from '@angular/material/table';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSortModule } from '@angular/material/sort';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSnackBarModule } from '@angular/material/snack-bar';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatSliderModule } from '@angular/material/slider';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSlideToggleModule } from '@angular/material/slide-toggle';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatChipsModule } from '@angular/material/chips';
import { PhraseWithDetailsComponent } from './components/article/phrase-with-details/phrase-with-details.component';
import { ErrorHandlerInterceptor } from './shared/intercepters/error-handler.interceptor';
import { ToastrModule } from 'ngx-toastr';
import { CommandBarComponent } from './shared/components/command-bar/command-bar.component';
import { SplitPhraseDialogComponent } from './shared/components/split-phrase-dialog/split-phrase-dialog.component';
import { ArticleActorDialogComponent } from './shared/components/article-actor-dialog/article-actor-dialog.component';
import { SelectActorsDialogComponent } from './shared/components/select-actors-dialog/select-actors-dialog.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { VideoPlayerDialogComponent } from './shared/components/video-player-dialog/video-player-dialog.component';
import { WordsComponent } from './components/words/words.component';
import { SpecialPlayComponent } from './components/special-play/special-play.component';
import { ArticleByVocabularyComponent } from './components/article-by-vocabulary/article-by-vocabulary.component';
import { SelectWordPhraseDialogComponent } from './shared/components/select-word-phrase-dialog/select-word-phrase-dialog.component';
import { SelectRepeatWordsPhrasesDialogComponent } from './shared/components/select-repeat-words-phrases-dialog/select-repeat-words-phrases-dialog.component';

// tslint:disable-next-line:max-line-length
import { UpdateWordTranslationDialogComponent } from './shared/components/update-word-translation-dialog/update-word-translation-dialog.component';
// tslint:disable-next-line:max-line-length
import { UpdateWordphraseTranslationDialogComponent } from './shared/components/update-wordphrase-translation-dialog/update-wordphrase-translation-dialog.component';
import { MixParametersComponent } from './shared/components/mix-parameters/mix-parameters.component';
import { RatingComponent } from './shared/components/rating/rating.component';
import { LanguagesSelectDialogComponent } from './shared/components/languages-select-dialog/languages-select-dialog.component';
import { AddLngHeaderInterceptor } from './shared/intercepters/add-lng-header.interceptor';
import { WordphraseDialogComponent } from './shared/components/wordphrase-dialog/wordphrase-dialog.component';
import { ArticleByDialogComponent } from './components/article-by-dialog/article-by-dialog.component';
import { SelectTemplateTextDialogComponent } from './shared/components/select-template-text-dialog/select-template-text-dialog.component';
import { UpdateWordDialogComponent } from './shared/components/update-word-dialog/update-word-dialog.component';
import { ArticleByVocabularyListComponent } from './components/article-by-vocabulary-list/article-by-vocabulary-list.component';
import { ArticleByDialogListComponent } from './components/article-by-dialog-list/article-by-dialog-list.component';
import { UsersComponent } from './components/users/users.component';
import { UpdateUserDialogComponent } from './shared/components/update-user-dialog/update-user-dialog.component';
import { UpdateTemplateJsonDialogComponent } from './shared/components/update-template-json-dialog/update-template-json-dialog.component';
import { LessonsComponent } from './components/lessons/lessons.component';
import { LessonComponent } from './components/lesson/lesson.component';
import { UpdateLessonDialogComponent } from './shared/components/update-lesson-dialog/update-lesson-dialog.component';
import { SelectFileDialogComponent } from './shared/components/select-file-dialog/select-file-dialog.component';
import { ArticleSpecialPlayComponent } from './components/article-special-play/article-special-play.component';
import { MultyplayerComponent } from './components/special-play/multyplayer/multyplayer.component';
import { WordFilteredListComponent } from './shared/components/word-filtered-list/word-filtered-list.component';
import { SpecialLessonsComponent } from './components/special-lessons/special-lessons.component';
import { ArticleBySchemaComponent } from './components/article-by-schema/article-by-schema.component';
import { ArticleBySchemaListComponent } from './components/article-by-schema-list/article-by-schema-list.component';
import { LessonsListComponent } from './shared/components/lessons-list/lessons-list.component';
import { ArticleByParamListComponent } from './shared/components/article-by-param-list/article-by-param-list.component';
import { SelectArticleByParamDialogComponent } from './shared/components/select-article-by-param-dialog/select-article-by-param-dialog.component';
import { RunArtparamGenerationDialogComponent } from './shared/components/run-artparam-generation-dialog/run-artparam-generation-dialog.component';

@NgModule({
  declarations: [
    AppComponent,
    ArticlesComponent,
    ArticleComponent,
    PhraseWithDetailsComponent,
    CommandBarComponent,
    SplitPhraseDialogComponent,
    ArticleActorDialogComponent,
    VideoPlayerDialogComponent,
    WordsComponent,
    ArticleByVocabularyComponent,
    SelectActorsDialogComponent,
    RunArtparamGenerationDialogComponent,
    SelectWordPhraseDialogComponent,
    SelectRepeatWordsPhrasesDialogComponent,
    UpdateWordTranslationDialogComponent,
    UpdateWordphraseTranslationDialogComponent,
    MixParametersComponent,
    RatingComponent,
    LanguagesSelectDialogComponent,
    WordphraseDialogComponent,
    ArticleByDialogComponent,
    SelectTemplateTextDialogComponent,
    UpdateWordDialogComponent,
    ArticleByVocabularyListComponent,
    ArticleByDialogListComponent,
    UsersComponent,
    UpdateUserDialogComponent,
    UpdateTemplateJsonDialogComponent,
    LessonsComponent,
    LessonComponent,
    UpdateLessonDialogComponent,
    SelectFileDialogComponent,
    ArticleSpecialPlayComponent,
    WordFilteredListComponent,
    SpecialPlayComponent,
    MultyplayerComponent,
    SpecialLessonsComponent,
    ArticleBySchemaComponent,
    ArticleBySchemaListComponent,
    LessonsListComponent,
    ArticleByParamListComponent,
    SelectArticleByParamDialogComponent,
  ],
  imports: [
    BrowserModule.withServerTransition({ appId: 'ng-cli-universal' }),
    LayoutModule,
    HttpClientModule,
    FormsModule,
    BrowserModule,
    BrowserAnimationsModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatSidenavModule,
    MatListModule,
    MatButtonModule,
    MatIconModule, MatPaginatorModule, MatTableModule, MatProgressSpinnerModule, MatSortModule,
    MatCardModule,
    MatGridListModule,
    MatSnackBarModule,
    ToastrModule.forRoot(),
    MatInputModule,
    MatSelectModule,
    MatButtonToggleModule,
    MatSliderModule,
    MatTooltipModule,
    MatCheckboxModule,
    MatSlideToggleModule,
    MatDialogModule,
    MatExpansionModule,
    DragDropModule,
    MatChipsModule,
  ],
  exports: [
    FormsModule,
    ReactiveFormsModule
  ],
  entryComponents: [
    SplitPhraseDialogComponent,
    ArticleActorDialogComponent,
    SelectActorsDialogComponent,
    RunArtparamGenerationDialogComponent,
    VideoPlayerDialogComponent,
    SelectWordPhraseDialogComponent,
    SelectRepeatWordsPhrasesDialogComponent,
    UpdateWordTranslationDialogComponent,
    UpdateWordphraseTranslationDialogComponent,
    LanguagesSelectDialogComponent,
    WordphraseDialogComponent,
    SelectTemplateTextDialogComponent,
    UpdateWordDialogComponent,
    UpdateUserDialogComponent,
    UpdateTemplateJsonDialogComponent,
    UpdateLessonDialogComponent,
  ],
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: ErrorHandlerInterceptor, multi: true },
    { provide: HTTP_INTERCEPTORS, useClass: AddLngHeaderInterceptor, multi: true }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
