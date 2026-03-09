import { Component } from '@angular/core';

declare var jQuery:any;

@Component({
  selector: 'blank',
  templateUrl: './blankLayout.component.html',
  styleUrls: ['./blankLayout.component.scss'],
})
export class BlankLayoutComponent {

  ngAfterViewInit() {
    jQuery('body').addClass('gray-bg');
  }

  ngOnDestroy() {
    jQuery('body').removeClass('gray-bg');
  }
}
