import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {Observable} from 'rxjs';
import {map} from 'rxjs/operators';
import {
  apiBrandGetAllGet$Json,
  apiConstructorsCreatePost,
  apiConstructorsGetAllConstructorsGet$Json,
  apiConstructorsGetIdGet$Json, apiConstructorsUpdatePost,

} from '../api/functions';
import {ConstructorListDto} from '../api/models/constructor-list-dto';
import {ConstructorDetailDto} from '../api/models/constructor-detail-dto';
import {BrandListDto} from '../api/models/brand-list-dto';
import {ConstructorCreateDto} from '../api/models/constructor-create-dto';
import {ConstructorUpdateDto} from '../api/models/constructor-update-dto';

@Injectable({
  providedIn: 'root',
})
export class ConstructorsService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllConstructors(): Observable<ConstructorListDto[]> {
    return apiConstructorsGetAllConstructorsGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => response.body as ConstructorListDto[])
    );
  }

  getConstructorById(id: string): Observable<ConstructorDetailDto> {
    return apiConstructorsGetIdGet$Json(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(response => response.body as ConstructorDetailDto)
    );
  }

  getAllBrands(): Observable<BrandListDto[]> {
    return apiBrandGetAllGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as BrandListDto[];
      })
    )
  }

  createConstructor(dto: ConstructorCreateDto) {
    return apiConstructorsCreatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  updateConstructor(dto: ConstructorUpdateDto) {
    return apiConstructorsUpdatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }
 }
