import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {DriverListDto} from '../api/models/driver-list-dto';
import {apiDriversGetAllDriversGet$Json} from '../api/fn/drivers/api-drivers-get-all-drivers-get-json';
import {map} from 'rxjs/operators';
import {apiDriversCreatePost, apiDriversGetIdGet$Json, apiDriversUpdatePost} from '../api/functions';
import {DriverDetailDto} from '../api/models/driver-detail-dto';
import {DriverCreateDto} from '../api/models/driver-create-dto';
import {DriverUpdateDto} from '../api/models/driver-update-dto';

@Injectable({
  providedIn: 'root',
})
export class DriverService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getDriverById(id: string): Observable<DriverDetailDto> {
    return apiDriversGetIdGet$Json(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(response => response.body as DriverDetailDto)
    );
  }

  getAllDrivers(): Observable<DriverListDto[]> {
    return apiDriversGetAllDriversGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => response.body as DriverListDto[])
    );
  }

  createDriver(dto: DriverCreateDto) {
    return apiDriversCreatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  updateDriver(dto: DriverUpdateDto) {
    return apiDriversUpdatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }
}
