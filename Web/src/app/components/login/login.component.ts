import {Component, OnDestroy, OnInit} from '@angular/core';
import {FormControl} from '@angular/forms';
import {Router} from '@angular/router';

declare var jQuery:any;

import {AuthService} from '../../services/auth.service';
import {Subscription} from 'rxjs/Rx';
import {AuthState} from '../../models/auth-state.model';
import {FormBuilder, FormGroup, Validators} from '@angular/forms';
import {Store} from "@ngrx/store";
import {IAppState} from "../../store/app.reducers";
import {SetAuth} from "../../store/actions";

@Component({
  selector: 'login',
  templateUrl: 'login.component.html',
  styleUrls: ['./login.component.scss'],
})
export class LoginComponent implements OnInit, OnDestroy {
  showConfirm = false;
  msg = '';
  confirmStatus = false;

  /* FORMS */
  loginForm: FormGroup;

  /* OBSERVABLES */
  subAuth: Subscription = new Subscription();

  constructor(
    private router: Router,
    private authService: AuthService,
    private fb: FormBuilder,
    private store: Store<IAppState>,
  ) {
    this.loginForm = this.fb.group({
      username: [null, Validators.compose([Validators.required, Validators.email])],
      pswd: [null, Validators.compose([Validators.required, Validators.minLength(8)])],
    });
  }

  ngOnInit(): void {
    jQuery(".sk-spinner.sk-spinner-wave").removeClass("loading");
  }

  ngOnDestroy() {
    this.subAuth.unsubscribe();
  }

  onSubmit(form){
    if (form.valid) {
      jQuery(".sk-spinner.sk-spinner-wave").addClass("loading");
      const logInData = {
        userName: form.controls['username'].value,
        password: form.controls['pswd'].value,
      };

      this.subAuth = this.authService.login(logInData).subscribe((authState: AuthState) => {
        if(authState.valid) {
          this.router.navigate(['/inicio']);
          sessionStorage.setItem('userData', JSON.stringify(authState));
          this.store.dispatch(new SetAuth(authState));
          this.confirmStatus = authState.valid;
        } else {
          this.msg = authState.message;
          this.showConfirm = true;
        }

        jQuery(".sk-spinner.sk-spinner-wave").removeClass("loading");
      }, () => {
        this.confirmStatus = false;
        this.showConfirm = true;
        jQuery(".sk-spinner.sk-spinner-wave").removeClass("loading");
      });
    }
    else{
      this.validateAllFormFields(this.loginForm); 
    }
  }

  isFieldValid(field: string) {
    return !this.loginForm.get(field).valid && this.loginForm.get(field).touched;
  }
  
  displayFieldCss(field: string) {
    return {
      'has-error': this.isFieldValid(field),
      'has-feedback': this.isFieldValid(field)
    };
  }

  validateAllFormFields(formGroup: FormGroup) {         //{1}
    Object.keys(formGroup.controls).forEach(field => {  //{2}
      const control = formGroup.get(field);             //{3}
      if (control instanceof FormControl) {             //{4}
        control.markAsTouched({ onlySelf: true });
      } 
      else if (control instanceof FormGroup) {        //{5}
        this.validateAllFormFields(control);            //{6}
      }
    });
  }
}
