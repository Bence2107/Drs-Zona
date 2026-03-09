import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ApiConfiguration } from '../api/api-configuration';
import { SeriesLookupDto } from '../api/models/series-lookup-dto';
import { YearLookupDto } from '../api/models/year-lookup-dto';
import { GrandPrixLookupDto } from '../api/models/grand-prix-lookup-dto';
import { GrandPrixResultsDto } from '../api/models/grand-prix-results-dto';
import {
  apiGrandPrixCreatePost,
  apiGrandPrixGetAllCircuitsGet$Json,
  apiStandingsAddParticipationsPost,
  apiStandingsCreateChampionshipPost,
  apiStandingsGetAllChampionshipsBySeriesSeriesIdGet$Json,
  apiStandingsGetAllSeriesGet$Json,
  apiStandingsGetByConstructorChampionshipIdConstructsChampIdGet$Json,
  apiStandingsGetByDriversChampionshipIdDriversChampIdGet$Json,
  apiStandingsGetConstructorsResultsBySeasonConstructorIdConstructorChampIdGet$Json,
  apiStandingsGetDriverResultsBySeasonDriverIdDriverChampIdGet$Json,
  apiStandingsGetDriversByDriversChampionshipDriverChampIdGet$Json,
  apiStandingsGetGrandPrixByChampionshipDriverChampIdGet$Json, apiStandingsGetGrandPrixContextGrandPrixIdGet$Json,
  apiStandingsGetGrandPrixResultsGrandPrixIdSessionGet$Json,
  apiStandingsGetParticipationsDriversChampIdConstructorsChampIdGet$Json,
  apiStandingsGetSeasonOverviewDriverChampIdGet$Json,
  apiStandingsGetSeasonsBySeriesSeriesIdGet$Json, apiStandingsGetSessionForEditGrandPrixIdSessionGet$Json,
  apiStandingsGetSessionsByGrandPrixGrandPrixIdGet$Json,
  apiStandingsInsertResultsPost, apiStandingsRecalculateSessionGrandPrixIdSessionPost,
  apiStandingsRemoveConstructorCompetitionConstructorIdConstructorsChampIdDelete,
  apiStandingsRemoveDriverParticipationDriverIdDriversChampIdDelete, apiStandingsSaveSessionResultsPost,
  apiStandingsUpdateChampionshipStatusDriversChampIdConstructorsChampIdStatusPost, apiStandingsUpdateSingleResultPost
} from '../api/functions';
import {DriverStandingsDto} from '../api/models/driver-standings-dto';
import {ConstructorStandingsDto} from '../api/models/constructor-standings-dto';
import { SeasonOverviewDto } from '../api/models/season-overview-dto';
import {DriverSeasonResultDto} from '../api/models/driver-season-result-dto';
import {ConstructorSeasonResultDto} from '../api/models/constructor-season-result-dto';
import {
  AddParticipationsDto, BatchResultCreateDto,
  ChampionshipCreateDto,
  ChampionshipRowDto, CircuitListDto,
  ConstructorLookUpDto,
  DriverLookUpDto, GrandPrixChampionshipContextDto, GrandPrixCreateDto, SessionEditDto, SingleResultUpdateDto,
} from "../api/models";
import {
  apiStandingsGetConstructorsByConstructorsChampionshipConstChampIdGet$Json
} from '../api/fn/standings/api-standings-get-constructors-by-constructors-championship-const-champ-id-get-json';
import {ParticipationListDto} from '../api/models/participation-list-dto';

@Injectable({
  providedIn: 'root'
})
export class ResultsService {

  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }

  getAllSeries(): Observable<SeriesLookupDto[]> {
    return apiStandingsGetAllSeriesGet$Json(this.http, this.apiConfig.rootUrl, {}).pipe(
      map(r => r.body as SeriesLookupDto[])
    );
  }

  getAllChampionshipsBySeries(seriesId: string): Observable<ChampionshipRowDto[]> {
    return apiStandingsGetAllChampionshipsBySeriesSeriesIdGet$Json(this.http, this.apiConfig.rootUrl, {seriesId: seriesId}).pipe(
      map(r => r.body as ChampionshipRowDto[])
    )
  }

  getSeasonsBySeries(seriesId: string): Observable<YearLookupDto[]> {
    return apiStandingsGetSeasonsBySeriesSeriesIdGet$Json(this.http, this.apiConfig.rootUrl, { seriesId }).pipe(
      map(r => r.body as YearLookupDto[])
    );
  }

  getGrandPrixByChampionship(driverChampId: string): Observable<GrandPrixLookupDto[]> {
    return apiStandingsGetGrandPrixByChampionshipDriverChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driverChampId }).pipe(
      map(r => r.body as GrandPrixLookupDto[])
    );
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

  getGrandPrixResults(grandPrixId: string, session: string): Observable<GrandPrixResultsDto> {
    return apiStandingsGetGrandPrixResultsGrandPrixIdSessionGet$Json(this.http, this.apiConfig.rootUrl, {
      grandPrixId,
      session
    }).pipe(
      map(r => r.body as GrandPrixResultsDto)
    );
  }

  getDriverStandings(driversChampId: string): Observable<DriverStandingsDto> {
    return apiStandingsGetByDriversChampionshipIdDriversChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driversChampId }).pipe(
      map(r => r.body as DriverStandingsDto)
    );
  }

  getConstructorStandings(constructsChampId: string): Observable<ConstructorStandingsDto> {
    return apiStandingsGetByConstructorChampionshipIdConstructsChampIdGet$Json(this.http, this.apiConfig.rootUrl, { constructsChampId }).pipe(
      map(r => r.body as ConstructorStandingsDto)
    );
  }

  getSeasonOverview(driversChampId: string): Observable<SeasonOverviewDto[]> {
    return apiStandingsGetSeasonOverviewDriverChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driverChampId: driversChampId}).pipe(
      map(r => r.body as SeasonOverviewDto[])
    )
  }

  getDriversByDriversChampionship(driversChampId: string): Observable<DriverLookUpDto[]> {
    return apiStandingsGetDriversByDriversChampionshipDriverChampIdGet$Json(
      this.http, this.apiConfig.rootUrl, { driverChampId: driversChampId }).pipe(
      map(r => r.body as DriverLookUpDto[])
    );
  }

  getConstructorsByConstChampionship(constChampId: string): Observable<ConstructorLookUpDto[]> {
    return apiStandingsGetConstructorsByConstructorsChampionshipConstChampIdGet$Json(this.http, this.apiConfig.rootUrl, { constChampId: constChampId}).pipe(
      map(r => r.body as ConstructorLookUpDto[])
    )
  }

  getDriverResultsBySeason(driverId:string, driversChampId: string): Observable<DriverSeasonResultDto[]> {
      return apiStandingsGetDriverResultsBySeasonDriverIdDriverChampIdGet$Json(this.http, this.apiConfig.rootUrl,
        {driverId: driverId, driverChampId: driversChampId}).pipe(
        map(r => r.body as DriverSeasonResultDto[])
      );
  }

  getConstructorsResultsBySeason(constructorId: string, constructorChampId: string): Observable<ConstructorSeasonResultDto[]> {
    return apiStandingsGetConstructorsResultsBySeasonConstructorIdConstructorChampIdGet$Json(this.http, this.apiConfig.rootUrl,
      {constructorId: constructorId, constructorChampId: constructorChampId }).pipe(
      map(r => r.body as ConstructorSeasonResultDto[])
    );
  }

  createChampionship(dto: ChampionshipCreateDto) {
    return apiStandingsCreateChampionshipPost(this.http, this.apiConfig.rootUrl, {
      body: dto
    }).pipe(map(() => void 0));
  }

  updateChampionship(driversChampId: string, constructorsChampId: string, status: string) {
    return apiStandingsUpdateChampionshipStatusDriversChampIdConstructorsChampIdStatusPost(this.http, this.apiConfig.rootUrl, {
      driversChampId: driversChampId, constructorsChampId: constructorsChampId, status: status
    }).pipe(map(() => void 0));
  }

  getAllCircuitsToList(): Observable<CircuitListDto[]> {
    return apiGrandPrixGetAllCircuitsGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(r => r.body as CircuitListDto[])
    );
  }

  createGrandPrix(dto: GrandPrixCreateDto) {
    return apiGrandPrixCreatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  addParticipation(dto: AddParticipationsDto) {
    return apiStandingsAddParticipationsPost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  removeDriverParticipation(driverId: string, driversChampId: string) {
    return apiStandingsRemoveDriverParticipationDriverIdDriversChampIdDelete(this.http, this.apiConfig.rootUrl,
      {driverId: driverId, driversChampId: driversChampId},).pipe(map(() => void 0));
  }

  removeConstructorParticipation(constId: string, constChampId: string) {
    return apiStandingsRemoveConstructorCompetitionConstructorIdConstructorsChampIdDelete(this.http, this.apiConfig.rootUrl,
      {constructorId: constId, constructorsChampId: constChampId},).pipe(map(() => void 0));
  }

  getParticipations(driversChampId: string, constructorsChampId: string): Observable<ParticipationListDto> {
    return apiStandingsGetParticipationsDriversChampIdConstructorsChampIdGet$Json(this.http, this.apiConfig.rootUrl,
      { driversChampId, constructorsChampId }
    ).pipe(map(r => r.body as ParticipationListDto));
  }

  getSessionForEdit(gpId: string, session: string): Observable<SessionEditDto> {
    return apiStandingsGetSessionForEditGrandPrixIdSessionGet$Json(this.http, this.apiConfig.rootUrl,
      {grandPrixId: gpId, session: session}).pipe(
        map(r => r.body as SessionEditDto));
  }

  getGrandPrixContext(grandPrixId: string): Observable<GrandPrixChampionshipContextDto> {
    return apiStandingsGetGrandPrixContextGrandPrixIdGet$Json(this.http, this.apiConfig.rootUrl, {grandPrixId: grandPrixId})
      .pipe(map(r => r.body as GrandPrixChampionshipContextDto));
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
