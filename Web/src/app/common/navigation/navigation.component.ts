import {Component, OnDestroy, OnInit} from '@angular/core';
import {Router} from '@angular/router';
import 'jquery-slimscroll';
import {Store} from "@ngrx/store";
import {IAppState} from "../../store/app.reducers";
import {AuthState} from "../../models/auth-state.model";
import {Subscription} from "rxjs/Rx";
import {ClearAuth, SetAuth} from "../../store/actions";
import {AuthService} from "../../services/auth.service";

declare var jQuery:any;

@Component({
  selector: 'navigation',
  templateUrl: 'navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
})

export class NavigationComponent implements OnInit, OnDestroy {
  expanded = false;
  user: AuthState = {
    valid: false,
    id: '',
    token: '',
    refreshToken: '',
    message: '',
  };

  /* OBSERVABLES */
  subAuth: Subscription = new Subscription();
  subStore: Subscription = new Subscription();

  constructor(
    private authService: AuthService,
    private router: Router,
    private store: Store<IAppState>,
  ) {}

  ngAfterViewInit() { }

  ngOnInit() {
    this.subStore = this.store.select('authState').subscribe((authState) => {
      if(authState && authState.user) {
        this.user = authState.user;
      }
    });

    this.authService.getSession().subscribe((authState: AuthState) =>{
      this.store.dispatch(new SetAuth(authState));
    });
  }

  ngOnDestroy() {
    this.subAuth.unsubscribe();
    this.subStore.unsubscribe();
  }

  activeRoute(routename: string): boolean{
    return this.router.url.indexOf(routename) > -1;
  }

  logout() {
    jQuery(".sk-spinner.sk-spinner-wave").addClass("loading");
    sessionStorage.setItem('userData', null);
    this.store.dispatch(new ClearAuth());
    this.router.navigate(['/login']);
    jQuery(".sk-spinner.sk-spinner-wave").removeClass("loading");
  }
}
