import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FindByFilterComponent } from './find-by-filter.component';

describe('FindByFilterComponent', () => {
  let component: FindByFilterComponent;
  let fixture: ComponentFixture<FindByFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FindByFilterComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FindByFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
