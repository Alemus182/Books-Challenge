import { Component, OnInit, ViewChild } from '@angular/core';
import { Story } from 'app/models/story.model';
import { StoriesService } from 'app/services/stories.service';
import {MatTableDataSource,} from '@angular/material/table';
import {MatPaginator, } from '@angular/material/paginator';
import { merge, Observable, of as observableOf, pipe } from 'rxjs';
import { catchError, map, startWith, switchMap } from 'rxjs/operators';



@Component({
  selector: 'app-newest',
  templateUrl: './newest.component.html',
  styleUrls: ['./newest.component.scss']
})
export class NewestComponent implements OnInit {

    /* Table Settings */
    dataSource = new MatTableDataSource<Story>();
    @ViewChild('paginator') paginator: MatPaginator;
    pageSizes = [20];
    displayedColumns: string[] = [
      'id',
      'title',
      'url',
    ];

    totalData: number;

    storiesData: Story[];

    isLoadingResults = true;
    
  constructor(private storiesService: StoriesService) { }

      ngOnInit(): void {
    
      };

      ngAfterViewInit() {
    
      this.dataSource.paginator = this.paginator;
  
      this.paginator.page
        .pipe(
          startWith({}),
          switchMap(() => {
            this.isLoadingResults = true;
            return this.getTableData(
              this.paginator.pageIndex,
            ).pipe(catchError(() => observableOf(null)));
          }),
          map((storiesData) => {
            if (storiesData == null) return [];
            this.totalData = storiesData.totalRecords;
            this.isLoadingResults = false;
            return storiesData.data;
          })
        )
        .subscribe((storyData) => {
          this.storiesData = storyData;
          this.dataSource = new MatTableDataSource(this.storiesData);
        });
    }

    getTableData(pageNumber: Number) {
      return this.storiesService.getNewEst(pageNumber);
    }
}
