import { Injectable } from '@angular/core';
import {apiBrandGetAllGet$Json} from '../api/functions';
import {map} from 'rxjs/operators';
import {BrandListDto} from '../api/models/brand-list-dto';
import {Observable} from 'rxjs';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';

@Injectable({
  providedIn: 'root',
})
export class BrandsService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllBrands(): Observable<BrandListDto[]> {
    return apiBrandGetAllGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as BrandListDto[];
      })
    )
  }
}
