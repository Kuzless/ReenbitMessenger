import { Injectable } from '@angular/core';
import { DataGrid } from '../shared/models/datagrid';
import { DataRetrieval } from '../shared/models/dataretrieval';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, map, of } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private baseUrl = environment.baseUrl;
  private accessToken = localStorage.getItem("accessToken") || '';

  constructor(private http: HttpClient) { }

  getAll<T>(data: DataRetrieval, endpoint: string): Observable<DataGrid<T>> {
    try {
      const headers = new HttpHeaders({
        'Authorization': 'Bearer ' + this.accessToken
      })
      var params = this.setUrlParams(data);
      return this.http.get<DataGrid<T>>(this.baseUrl + endpoint, { headers , params })
      .pipe(
        map(dataGrid => {
        return dataGrid;
      }));
    } catch (error) {
      return of({ data: [], numberOfPages: 1 });
    }
  }
  setUrlParams(data: DataRetrieval) : HttpParams {
    var params = new HttpParams()
      .set('PageNumber', data.pageNumber.toString())
      .set('PageSize', data.pageSize.toString());

    if (data.subs) {
      params = params.set('Subs', data.subs);
    }

    if (data.sort) {
      for (const [key, value] of Object.entries(data.sort)) {
        params = params.set(`Sort[${encodeURIComponent(key)}]`, value);
      }
    }
    return params;
  }
}
