import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ArticlesComponent } from './components/articles/articles.component';
import { AuthorizeGuard } from '../api-authorization/authorize.guard';
import { ArticleComponent } from './components/article/article.component';
import { WordsComponent } from './components/words/words.component';
import { ArticleByVocabularyComponent } from './components/article-by-vocabulary/article-by-vocabulary.component';
import { ArticleByDialogComponent } from './components/article-by-dialog/article-by-dialog.component';
import { ArticleByVocabularyListComponent } from './components/article-by-vocabulary-list/article-by-vocabulary-list.component';
import { ArticleByDialogListComponent } from './components/article-by-dialog-list/article-by-dialog-list.component';
import { UsersComponent } from './components/users/users.component';
import { LessonsComponent } from './components/lessons/lessons.component';
import { LessonComponent } from './components/lesson/lesson.component';
import { ArticleSpecialPlayComponent } from './components/article-special-play/article-special-play.component';
import { SpecialPlayComponent } from './components/special-play/special-play.component';
import { SpecialLessonsComponent } from './components/special-lessons/special-lessons.component';
import { ArticleBySchemaComponent } from './components/article-by-schema/article-by-schema.component';
import { ArticleBySchemaListComponent } from './components/article-by-schema-list/article-by-schema-list.component';

const appRoutes: Routes = [
  { path: 'article/:id', component: ArticleComponent, canActivate: [AuthorizeGuard] },
  { path: 'lesson/:id', component: LessonComponent, canActivate: [AuthorizeGuard] },
  { path: 'special-play', component: SpecialPlayComponent, canActivate: [AuthorizeGuard] },
  { path: 'special-lessons', component: SpecialLessonsComponent, data: { title: 'SLessons List' }, canActivate: [AuthorizeGuard] },
  { path: 'articleSpecialPlay/:id', component: ArticleSpecialPlayComponent, canActivate: [AuthorizeGuard] },
  { path: 'article-by-vocabulary-list', component: ArticleByVocabularyListComponent, canActivate: [AuthorizeGuard] },
  { path: 'article-by-vocabulary/:id', component: ArticleByVocabularyComponent, canActivate: [AuthorizeGuard] },
  { path: 'article-by-dialog-list', component: ArticleByDialogListComponent, canActivate: [AuthorizeGuard] },
  { path: 'article-by-dialog/:id', component: ArticleByDialogComponent, canActivate: [AuthorizeGuard] },
  { path: 'article-by-schema-list', component: ArticleBySchemaListComponent, canActivate: [AuthorizeGuard] },
  { path: 'article-by-schema/:id', component: ArticleBySchemaComponent, canActivate: [AuthorizeGuard] },
  {
    path: 'lessons',
    component: LessonsComponent,
    data: { title: 'Lessons List' },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'articles',
    component: ArticlesComponent,
    data: { title: 'Articles List' },
    canActivate: [AuthorizeGuard]
  },
  {
    path: 'words',
    component: WordsComponent,
    data: { title: 'Words List' }, canActivate: [AuthorizeGuard]
  },
  {
    path: 'users',
    component: UsersComponent,
    data: { title: 'Users List' }, canActivate: [AuthorizeGuard]
  },
  {
    path: '',
    redirectTo: '/articles',
    pathMatch: 'full'
  },
  { path: '**', component: WordsComponent, canActivate: [AuthorizeGuard] },
];

@NgModule({
  imports: [RouterModule.forRoot(appRoutes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

