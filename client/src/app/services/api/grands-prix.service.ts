import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../../api/api-configuration';
import {GrandPrixDetailDto} from '../../api/models/grand-prix-detail-dto';
import {Observable} from 'rxjs';
import {
  apiGrandPrixCreatePost,
  apiGrandPrixGetAllCircuitsGet$Json,
  apiGrandPrixGetGrandPrixIdGet$Json, apiGrandPrixUpdatePost
} from '../../api/functions';
import {map} from 'rxjs/operators';
import {CircuitListDto} from '../../api/models/circuit-list-dto';
import {GrandPrixCreateDto} from '../../api/models/grand-prix-create-dto';
import {GrandPrixUpdateDto} from '../../api/models/grand-prix-update-dto';

@Injectable({
  providedIn: 'root',
})
export class GrandsPrixService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getGrandPrixById(id: string): Observable<GrandPrixDetailDto> {
    return apiGrandPrixGetGrandPrixIdGet$Json(this.http, this.apiConfig.rootUrl, {grandPrixId: id}).pipe(
      map(r => r.body as GrandPrixDetailDto));
  }

  getAllCircuitsToList(): Observable<CircuitListDto[]> {
    return apiGrandPrixGetAllCircuitsGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(r => r.body as CircuitListDto[])
    );
  }

  createGrandPrix(dto: GrandPrixCreateDto): Observable<void> {
    return apiGrandPrixCreatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  updateGrandPrix(dto: GrandPrixUpdateDto): Observable<void> {
    return apiGrandPrixUpdatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }
}
