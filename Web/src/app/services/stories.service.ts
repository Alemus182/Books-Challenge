import {Injectable} from '@angular/core';
import {HttpClient, HttpHeaders} from '@angular/common/http';
import endpoints from './endpoints';
import { Observable } from 'rxjs';

@Injectable()
export  class StoriesService {
  constructor(private http: HttpClient) {}

  getNewEst(page) {;
    return this.http.get<any>(
      `${endpoints.STORIES.GET_NEWEST}${page}`,
    ).catch(this.handleError)
  }


  findByFilters(data) {
    return this.http.post<any>(
      `${endpoints.STORIES.FIND_BY_FILTER}`,
      data
    ).catch(this.handleError);
  }

  private handleError(error: any) { 
    let errMsg = (error.message) ? error.message : error.status ? `${error.status} - ${error.statusText}` : 'Server error';
    return Observable.throw(error);
  }
}
