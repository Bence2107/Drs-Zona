import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {SeriesListDto} from '../api/models/series-list-dto';
import {apiSeriesGetAllSeriesGet$Json, apiSeriesIdGet$Json, apiSeriesNameNameGet$Json} from '../api/functions';
import {map} from 'rxjs/operators';
import {SeriesDetailDto} from '../api/models/series-detail-dto';

@Injectable({
  providedIn: 'root',
})
export class SeriesService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getSeriesList(): Observable<SeriesListDto[]> {
    return apiSeriesGetAllSeriesGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => {
        return response.body as SeriesListDto[];
      })
    )
  }

  getSerieById(id: string): Observable<SeriesDetailDto> {
    return apiSeriesIdGet$Json(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(response => {
        return response.body as SeriesDetailDto;
      })
    )
  }

  getByName(name: string): Observable<SeriesDetailDto> {
    return apiSeriesNameNameGet$Json(this.http, this.apiConfig.rootUrl, {name: name}).pipe(
      map(response => {
        return response.body as SeriesDetailDto;
      })
    )
  }

}
