import { BrowserModule } from '@angular/platform-browser';
import {InjectionToken, NgModule} from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import {RouterModule} from "@angular/router";
import {LocationStrategy, HashLocationStrategy} from '@angular/common';
import {ROUTES} from "./app.routes";
import { AppComponent } from './app.component';

import { MatTableModule } from '@angular/material/table';
import { MatCommonModule } from '@angular/material/core';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatSortModule } from '@angular/material/sort';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';

// App components
import {LoginComponent} from "./components/login/login.component";
import {StarterViewComponent} from "./components/starterview/starterview.component";
import { NewestComponent } from './components/stories/newest/newest.component';
import { FindByFilterComponent } from './components/stories/find-by-filter/find-by-filter.component';
import { BookSearchComponent } from './components/books/book-search/book-search.component';


// App commons/
import {LayoutsModule} from "./common/layouts/layouts.module";
import {IboxtoolsModule} from "./common/iboxtools/iboxtools.module";
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { HomeComponent } from './components/home/home.component';

// App services
import {AuthService} from './services/auth.service';
import {AppGuardService} from './guards/app-guard.service';
import {JwtInterceptor} from './services/jwt.interceptor';
import {StoriesService} from './services/stories.service';
import {ValidationService } from './services/validation.service';
import {BooksService} from './services/books.service';

import { StoreModule} from "@ngrx/store";
import { appReducers} from "./store/app.reducers";
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

export const REDUCER_TOKEN = new InjectionToken('Registered Reducers');

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    StarterViewComponent,
    HomeComponent,
    NewestComponent,
    FindByFilterComponent,
    BookSearchComponent,
  ],
  imports: [
    BrowserModule,
    FormsModule,
    HttpClientModule,
    LayoutsModule,
    RouterModule.forRoot(ROUTES),
    IboxtoolsModule,
    BrowserAnimationsModule,
    ReactiveFormsModule,
    StoreModule.forRoot(REDUCER_TOKEN),
    NgbModule,
    MatTableModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatSortModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatRadioModule,
    MatCommonModule
  ],
  providers: [
    {provide: LocationStrategy, useClass: HashLocationStrategy},
    AuthService,
    StoriesService,
    BooksService,
    ValidationService,
    AppGuardService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JwtInterceptor,
      multi: true,
    },
    {
      provide: REDUCER_TOKEN,
      useValue: appReducers,
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
