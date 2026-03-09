import {Component, OnDestroy, OnInit} from '@angular/core';
import {Store} from "@ngrx/store";
import {IAppState} from "../../store/app.reducers";
import {AuthState} from "../../models/auth-state.model";
import {Subscription} from "rxjs/Rx";
declare var jQuery:any;

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit, OnDestroy {
  user: AuthState = {
    valid: false,
    id: '',
    token: '',
    refreshToken: '',
    message: ''
  };

  // OBSERVABLES
  subStore: Subscription = new Subscription();

  constructor(
    private store: Store<IAppState>,
  ) { }

  ngOnInit(): void {
    if (jQuery("body").width() <= 992 ) {
      this.toggleNavhome();
    }

    this.subStore = this.store.select('authState').subscribe((authState) => {
      if(authState && authState.user) {
        this.user = authState.user;
      }
    });
  }

  ngOnDestroy() {
    this.subStore.unsubscribe();
  }

  /* istanbul ignore next */
  toggleNavhome(): void {
    jQuery("app-home").toggleClass("mini-navhome");
  }

}
