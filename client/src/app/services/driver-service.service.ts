import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {DriverListDto} from '../api/models/driver-list-dto';
import {apiDriversGetAllDriversGet$Json} from '../api/fn/drivers/api-drivers-get-all-drivers-get-json';
import {map} from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class DriverServiceService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllDrivers(): Observable<DriverListDto[]> {
    return apiDriversGetAllDriversGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => response.body as DriverListDto[])
    );
  }
}
