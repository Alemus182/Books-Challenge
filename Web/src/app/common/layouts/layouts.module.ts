import {NgModule} from "@angular/core";
import {BrowserModule} from "@angular/platform-browser";
import {RouterModule} from "@angular/router";

import {BsDropdownModule} from 'ngx-bootstrap/dropdown';

import {BasicLayoutComponent} from "./basicLayout.component";
import {BlankLayoutComponent} from "./blankLayout.component";

import {NavigationComponent} from "../navigation/navigation.component";
import {FooterComponent} from "../footer/footer.component";
import {TopNavbarComponent} from "../topnavbar/topnavbar.component";
import {FieldErrorDisplayComponent} from "../field-error-display/field-error-display.component";
import { AlertComponent } from '../alert/alert.component';


@NgModule({
  declarations: [
    FooterComponent,
    BasicLayoutComponent,
    BlankLayoutComponent,
    NavigationComponent,
    TopNavbarComponent,
    AlertComponent,
    FieldErrorDisplayComponent,
  ],
  imports: [
    BrowserModule,
    RouterModule,
    BsDropdownModule.forRoot(),
  ],
  exports: [
    FooterComponent,
    BasicLayoutComponent,
    BlankLayoutComponent,
    NavigationComponent,
    TopNavbarComponent,
    FieldErrorDisplayComponent
  ],
})

export class LayoutsModule {}
