import {Routes} from "@angular/router";
import {StarterViewComponent} from "./components/starterview/starterview.component";
import {HomeComponent} from "./components/home/home.component";
import {LoginComponent} from "./components/login/login.component";
import {BlankLayoutComponent} from "./common/layouts/blankLayout.component";
import {BasicLayoutComponent} from "./common/layouts/basicLayout.component";
import {AppGuardService} from "./guards/app-guard.service";
import {NewestComponent } from "./components/stories/newest/newest.component";
import {FindByFilterComponent } from "./components/stories/find-by-filter/find-by-filter.component";
import {BookSearchComponent} from "./components/books/book-search/book-search.component";

export const ROUTES:Routes = [

  {path: '', redirectTo: 'stories', pathMatch: 'full', canActivate: [AppGuardService]},
  {
    path: '', component: BasicLayoutComponent,
    children: [
      {
        path: 'stories',
        component: HomeComponent,
        canActivate: [AppGuardService],
        children: [
          {path: '', component: StarterViewComponent, canActivate: [AppGuardService]},
          {path: 'new-stories', component: NewestComponent, canActivate: [AppGuardService]},
          {path: 'find-stories', component: FindByFilterComponent, canActivate: [AppGuardService]}                                                                         
        ]
      },
      {
        path: 'books',
        canActivate: [AppGuardService],
        children: [
          {path: 'search', component: BookSearchComponent, canActivate: [AppGuardService]}
        ]
      }
    ]
  },
  {
    path: '', component: BlankLayoutComponent,
    children: [
      { path: 'login', component: LoginComponent },
    ]
  }
  ,
  {path: '**',  redirectTo: 'stories'}
];
