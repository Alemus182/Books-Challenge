import { Component, OnDestroy, OnInit, } from '@angular/core';

@Component({
  selector: 'starter',
  templateUrl: 'starterview.component.html',
  styleUrls: ['./starterview.component.scss'],
})
export class StarterViewComponent implements OnDestroy, OnInit  {

public nav:any;

public constructor() {
  this.nav = document.querySelector('nav.navbar');
}

public ngOnInit():any {

}


public ngOnDestroy():any {

}

}
