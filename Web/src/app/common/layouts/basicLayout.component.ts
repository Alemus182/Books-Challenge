import {Component, HostListener} from '@angular/core';
import { detectBody } from '../../app.helpers';
import {AuthService} from "../../services/auth.service";

declare var jQuery:any;

@Component({
  selector: 'basic',
  templateUrl: './basicLayout.component.html',
  styleUrls: ['./basicLayout.component.scss'],
  host: {
    '(window:resize)': 'onResize()'
  }
})
export class BasicLayoutComponent {
  constructor(
    private authService: AuthService,
  ) {
  }

  @HostListener('document:keyup', ['$event'])
  @HostListener('document:click', ['$event'])
  @HostListener('document:wheel', ['$event'])
  @HostListener('document:mousemove', ['$event'])
  @HostListener('document:scroll', ['$event'])
  @HostListener('document:touchstart', ['$event'])
  resetTimer($event) {
    this.authService.notifyUserAction();
  }

  public ngOnInit():any {
    detectBody();
  }

  public onResize(){
    detectBody();
  }

}
