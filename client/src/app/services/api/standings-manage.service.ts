import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../../api/api-configuration';
import {apiSeriesGetAllSeriesGet$Json} from '../../api/functions';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {SeriesListDto} from '../../api/models/series-list-dto';

@Injectable({
  providedIn: 'root',
})
export class StandingsManageService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllSeries(): Observable<SeriesListDto[]> {
    return apiSeriesGetAllSeriesGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(r => r.body as SeriesListDto[])
    )
  }



}
