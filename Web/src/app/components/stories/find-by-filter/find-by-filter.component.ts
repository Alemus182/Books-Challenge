import { Component, OnInit, ViewChild } from '@angular/core';
import { Story } from 'app/models/story.model';
import { StoriesService } from 'app/services/stories.service';
import { AlertService } from 'app/services/alert.service';
import {MatTableDataSource,} from '@angular/material/table';
import {MatPaginator } from '@angular/material/paginator';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-find-by-filter',
  templateUrl: './find-by-filter.component.html',
  styleUrls: ['./find-by-filter.component.scss']
})
export class FindByFilterComponent implements OnInit {

      /* Table Settings */
      public array: Story[];
      public pageSize = 20;
      public currentPage = 0;
      public totalSize = 0; 

      dataSource = new MatTableDataSource<Story>();
      @ViewChild('paginator') paginator: MatPaginator;
      displayedColumns: string[] = [
        'id',
        'title',
        'url',
      ];

      isLoadingResults = false;
      
      searchForm: FormGroup;
 
  constructor
  (private storiesService: StoriesService, 
   private fb: FormBuilder,
   private alertService: AlertService) { 

    this.searchForm = this.fb.group({
      isCategory: ['', Validators.required],
      category: [''],
      id: ['']
    });
  }

  ngOnInit(): void { }
  
  private handlePage(e: any) {
    this.currentPage = e.pageIndex;
    this.pageSize = e.pageSize;
    this.totalSize = e.length;
    this.getData();
    return e;
  }

  private Find(){
    if(this.searchForm.valid)
      this.getData();
  }

  private getData() {

    let data;
    console.log(this.paginator.pageIndex);

    if(this.IsCategory.value){
      data =
      {
         isCategory: true,
         page: this.paginator.pageIndex,
         id:0,
         category:this.Category.value
      }
    }
    else{
     data =
     {
       isCategory: false,
       page: this.paginator.pageIndex,
       id:this.Id.value,
       category:''
     }
   }

  this.isLoadingResults = true;
    this.storiesService.findByFilters(data)
      .subscribe((response) => {
        if(response.data.length == 0)
          this.alertService.info('Not found items');
        else if(response.data.length == 1 && response.data[0].title == null)
           this.alertService.info('Not information associated');

        this.dataSource.data = response.data;
        this.totalSize = response.totalRecords;
        this.isLoadingResults = false;
      }, error =>{
        this.alertService.error(error);
      })
  }

  private ChangeFilter(isCategory){
    this.dataSource = new MatTableDataSource<Story>();;
      if(isCategory.value){
        this.removeIdValidation();
        this.addCategoryValidation();
      }
      else{
        this.removeCategoryValidation();
        this.addIdValidation();
      }
  }

   get IsCategory(): AbstractControl {
    return this.searchForm.get('isCategory');
  }

   get Id(): AbstractControl {
    return this.searchForm.get('id');
   }

   addIdValidation() {
    this.Id.setValidators([Validators.required, Validators.maxLength(8)]);
    this.Id.updateValueAndValidity();
   }

   removeIdValidation() {
    this.Id.setValidators(null);
    this.Id.reset();
    this.Id.updateValueAndValidity();
   }

   get Category(): AbstractControl {
    return this.searchForm.get('category');
   }

   addCategoryValidation() {
    this.Category.setValidators([Validators.required]);
    this.Category.updateValueAndValidity();
   }

   removeCategoryValidation() {
    this.Category.setValidators(null);
    this.Category.reset();
    this.Category.updateValueAndValidity();
   }
}
