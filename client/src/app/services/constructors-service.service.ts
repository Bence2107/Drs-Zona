import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {DriverListDto} from '../api/models/driver-list-dto';
import {apiDriversGetAllDriversGet$Json} from '../api/fn/drivers/api-drivers-get-all-drivers-get-json';
import {map} from 'rxjs/operators';
import {apiConstructorsGetAllConstructorsGet$Json} from '../api/functions';
import {ConstructorListDto} from '../api/models/constructor-list-dto';

@Injectable({
  providedIn: 'root',
})
export class ConstructorsServiceService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllConstructors(): Observable<ConstructorListDto[]> {
    return apiConstructorsGetAllConstructorsGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => response.body as ConstructorListDto[])
    );
  }
}
