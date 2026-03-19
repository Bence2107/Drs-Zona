import { Injectable } from '@angular/core';
import {Observable} from 'rxjs';
import {ChampionshipRowDto} from '../../api/models/championship-row-dto';
import {
  apiChampionshipAddParticipationsPost, apiChampionshipCreateChampionshipPost,
  apiChampionshipGetAllChampionshipsBySeriesSeriesIdGet$Json,
  apiChampionshipGetConstructorsByConstructorsChampionshipConstChampIdGet$Json,
  apiChampionshipGetDriversByDriversChampionshipDriverChampIdGet$Json,
  apiChampionshipGetGrandPrixByChampionshipDriverChampIdGet$Json,
  apiChampionshipGetParticipationsDriversChampIdConstructorsChampIdGet$Json,
  apiChampionshipGetSeasonsBySeriesSeriesIdGet$Json,
  apiChampionshipRemoveConstructorCompetitionConstructorIdConstructorsChampIdDelete,
  apiChampionshipRemoveDriverParticipationDriverIdDriversChampIdDelete,
  apiChampionshipUpdateChampionshipStatusDriversChampIdConstructorsChampIdStatusPost
} from '../../api/functions';
import {map} from 'rxjs/operators';
import {GrandPrixLookupDto} from '../../api/models/grand-prix-lookup-dto';
import {DriverLookUpDto} from '../../api/models/driver-look-up-dto';
import {ConstructorLookUpDto} from '../../api/models/constructor-look-up-dto';
import {ParticipationListDto} from '../../api/models/participation-list-dto';
import {ChampionshipCreateDto} from '../../api/models/championship-create-dto';
import {ParticipationAddDto} from '../../api/models/participation-add-dto';
import {YearLookupDto} from '../../api/models/year-lookup-dto';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../../api/api-configuration';

@Injectable({
  providedIn: 'root',
})
export class ChampionshipService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllChampionshipsBySeries(seriesId: string): Observable<ChampionshipRowDto[]> {
    return apiChampionshipGetAllChampionshipsBySeriesSeriesIdGet$Json(this.http, this.apiConfig.rootUrl, {seriesId: seriesId}).pipe(
      map(r => r.body as ChampionshipRowDto[])
    )
  }

  getConstructorsByConstChampionship(constChampId: string): Observable<ConstructorLookUpDto[]> {
    return apiChampionshipGetConstructorsByConstructorsChampionshipConstChampIdGet$Json(this.http, this.apiConfig.rootUrl, { constChampId: constChampId}).pipe(
      map(r => r.body as ConstructorLookUpDto[])
    )
  }

  getDriversByDriversChampionship(driversChampId: string): Observable<DriverLookUpDto[]> {
    return apiChampionshipGetDriversByDriversChampionshipDriverChampIdGet$Json(
      this.http, this.apiConfig.rootUrl, { driverChampId: driversChampId }).pipe(
      map(r => r.body as DriverLookUpDto[])
    );
  }

  getGrandPrixByChampionship(driverChampId: string): Observable<GrandPrixLookupDto[]> {
    return apiChampionshipGetGrandPrixByChampionshipDriverChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driverChampId }).pipe(
      map(r => r.body as GrandPrixLookupDto[])
    );
  }

  getParticipations(driversChampId: string, constructorsChampId: string): Observable<ParticipationListDto> {
    return apiChampionshipGetParticipationsDriversChampIdConstructorsChampIdGet$Json(this.http, this.apiConfig.rootUrl,
      { driversChampId, constructorsChampId }
    ).pipe(map(r => r.body as ParticipationListDto));
  }

  getSeasonsBySeries(seriesId: string): Observable<YearLookupDto[]> {
    return apiChampionshipGetSeasonsBySeriesSeriesIdGet$Json(this.http, this.apiConfig.rootUrl, { seriesId }).pipe(
      map(r => r.body as YearLookupDto[])
    );
  }

  addParticipation(dto: ParticipationAddDto) {
    return apiChampionshipAddParticipationsPost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  createChampionship(dto: ChampionshipCreateDto) {
    return apiChampionshipCreateChampionshipPost(this.http, this.apiConfig.rootUrl, {
      body: dto
    }).pipe(map(() => void 0));
  }

  updateChampionship(driversChampId: string, constructorsChampId: string, status: string) {
    return apiChampionshipUpdateChampionshipStatusDriversChampIdConstructorsChampIdStatusPost(this.http, this.apiConfig.rootUrl, {
      driversChampId: driversChampId, constructorsChampId: constructorsChampId, status: status
    }).pipe(map(() => void 0));
  }

  removeDriverParticipation(driverId: string, driversChampId: string) {
    return apiChampionshipRemoveDriverParticipationDriverIdDriversChampIdDelete(this.http, this.apiConfig.rootUrl,
      {driverId: driverId, driversChampId: driversChampId},).pipe(map(() => void 0));
  }

  removeConstructorParticipation(constId: string, constChampId: string) {
    return apiChampionshipRemoveConstructorCompetitionConstructorIdConstructorsChampIdDelete(this.http, this.apiConfig.rootUrl,
      {constructorId: constId, constructorsChampId: constChampId},).pipe(map(() => void 0));
  }
}
