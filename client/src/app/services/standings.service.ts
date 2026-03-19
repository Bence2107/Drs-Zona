import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiConfiguration } from '../api/api-configuration';
import { SeriesLookupDto } from '../api/models/series-lookup-dto';
import { GrandPrixResultsDto } from '../api/models/grand-prix-results-dto';
import {
  apiStandingsGetAllSeriesGet$Json,
  apiStandingsGetByConstructorChampionshipIdConstructsChampIdGet$Json,
  apiStandingsGetByDriversChampionshipIdDriversChampIdGet$Json,
  apiStandingsGetConstructorsResultsBySeasonConstructorIdConstructorChampIdGet$Json,
  apiStandingsGetDriverResultsBySeasonDriverIdDriverChampIdGet$Json,
  apiStandingsGetGrandPrixContextGrandPrixIdGet$Json,
  apiStandingsGetGrandPrixResultsGrandPrixIdSessionGet$Json,
  apiStandingsGetSeasonOverviewDriverChampIdGet$Json,
  apiStandingsGetSessionForEditGrandPrixIdSessionGet$Json,
  apiStandingsGetSessionsByGrandPrixGrandPrixIdGet$Json,
  apiStandingsInsertResultsPost, apiStandingsRecalculateSessionGrandPrixIdSessionPost,
  apiStandingsSaveSessionResultsPost,
  apiStandingsUpdateSingleResultPost
} from '../api/functions';
import {DriverStandingsDto} from '../api/models/driver-standings-dto';
import {ConstructorStandingsDto} from '../api/models/constructor-standings-dto';
import { SeasonOverviewDto } from '../api/models/season-overview-dto';
import {DriverSeasonResultDto} from '../api/models/driver-season-result-dto';
import {ConstructorSeasonResultDto} from '../api/models/constructor-season-result-dto';
import {
  BatchResultCreateDto,
  GrandPrixChampionshipContextDto,
  SessionEditDto,
  SingleResultUpdateDto,
} from "../api/models";

@Injectable({
  providedIn: 'root'
})
export class StandingsService {

  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllSeries(): Observable<SeriesLookupDto[]> {
    return apiStandingsGetAllSeriesGet$Json(this.http, this.apiConfig.rootUrl, {}).pipe(
      map(r => r.body as SeriesLookupDto[])
    );
  }

  getConstructorStandings(constructsChampId: string): Observable<ConstructorStandingsDto> {
    return apiStandingsGetByConstructorChampionshipIdConstructsChampIdGet$Json(this.http, this.apiConfig.rootUrl, { constructsChampId }).pipe(
      map(r => r.body as ConstructorStandingsDto)
    );
  }

  getConstructorsResultsBySeason(constructorId: string, constructorChampId: string): Observable<ConstructorSeasonResultDto[]> {
    return apiStandingsGetConstructorsResultsBySeasonConstructorIdConstructorChampIdGet$Json(this.http, this.apiConfig.rootUrl,
      {constructorId: constructorId, constructorChampId: constructorChampId }).pipe(
      map(r => r.body as ConstructorSeasonResultDto[])
    );
  }

  getDriverStandings(driversChampId: string): Observable<DriverStandingsDto> {
    return apiStandingsGetByDriversChampionshipIdDriversChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driversChampId }).pipe(
      map(r => r.body as DriverStandingsDto)
    );
  }

  getDriverResultsBySeason(driverId:string, driversChampId: string): Observable<DriverSeasonResultDto[]> {
    return apiStandingsGetDriverResultsBySeasonDriverIdDriverChampIdGet$Json(this.http, this.apiConfig.rootUrl,
      {driverId: driverId, driverChampId: driversChampId}).pipe(
      map(r => r.body as DriverSeasonResultDto[])
    );
  }

  getGrandPrixContext(grandPrixId: string): Observable<GrandPrixChampionshipContextDto> {
    return apiStandingsGetGrandPrixContextGrandPrixIdGet$Json(this.http, this.apiConfig.rootUrl, {grandPrixId: grandPrixId})
      .pipe(map(r => r.body as GrandPrixChampionshipContextDto));
  }

  getGrandPrixResults(grandPrixId: string, session: string): Observable<GrandPrixResultsDto> {
    return apiStandingsGetGrandPrixResultsGrandPrixIdSessionGet$Json(this.http, this.apiConfig.rootUrl, {
      grandPrixId,
      session
    }).pipe(
      map(r => r.body as GrandPrixResultsDto)
    );
  }

  getSeasonOverview(driversChampId: string): Observable<SeasonOverviewDto[]> {
    return apiStandingsGetSeasonOverviewDriverChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driverChampId: driversChampId}).pipe(
      map(r => r.body as SeasonOverviewDto[])
    )
  }

  getSessionForEdit(gpId: string, session: string): Observable<SessionEditDto> {
    return apiStandingsGetSessionForEditGrandPrixIdSessionGet$Json(this.http, this.apiConfig.rootUrl,
      {grandPrixId: gpId, session: session}).pipe(
      map(r => r.body as SessionEditDto));
  }

  getSessionsByGrandPrix(grandPrixId: string): Observable<string[]> {
    return apiStandingsGetSessionsByGrandPrixGrandPrixIdGet$Json(this.http, this.apiConfig.rootUrl, { grandPrixId }).pipe(
      map(response => {
        const data = response.body;

        if (Array.isArray(data)) {
          return data;
        }


        return [];
      })
    );
  }

  insertResults(dto: BatchResultCreateDto): Observable<void> {
    return apiStandingsInsertResultsPost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  saveSessionResults(dto: BatchResultCreateDto): Observable<void> {
    return apiStandingsSaveSessionResultsPost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  updateSingleResult(dto: SingleResultUpdateDto): Observable<void> {
    return apiStandingsUpdateSingleResultPost(this.http, this.apiConfig.rootUrl,
      {body: dto}).pipe(map(() => void 0));
  }

  recalculateSession(gpId: string, session: string): Observable<void> {
    return apiStandingsRecalculateSessionGrandPrixIdSessionPost(this.http, this.apiConfig.rootUrl,
      {grandPrixId: gpId, session: session}).pipe(map(() => void 0));
  }
}
