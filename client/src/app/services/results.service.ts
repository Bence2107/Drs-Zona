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
  apiStandingsGetAllSeriesGet$Json, apiStandingsGetByConstructorChampionshipIdConstructsChampIdGet$Json,
  apiStandingsGetByDriversChampionshipIdDriversChampIdGet$Json,
  apiStandingsGetConstructorsResultsBySeasonConstructorIdConstructorChampIdGet$Json,
  apiStandingsGetDriverResultsBySeasonDriverIdDriverChampIdGet$Json,
  apiStandingsGetGrandPrixByChampionshipDriverChampIdGet$Json,
  apiStandingsGetGrandPrixResultsGrandPrixIdSessionGet$Json, apiStandingsGetSeasonOverviewDriverChampIdGet$Json,
  apiStandingsGetSeasonsBySeriesSeriesIdGet$Json,
  apiStandingsGetSessionsByGrandPrixGrandPrixIdGet$Json
} from '../api/functions';
import {DriverStandingsDto} from '../api/models/driver-standings-dto';
import {ConstructorStandingsDto} from '../api/models/constructor-standings-dto';
import { SeasonOverviewDto } from '../api/models/season-overview-dto';
import {DriverSeasonResultDto} from '../api/models/driver-season-result-dto';
import {ConstructorSeasonResultDto} from '../api/models/constructor-season-result-dto';
import { DriverLookUpDto } from "../api/models";
import { ConstructorLookUpDto } from "../api/models";
import {
  apiStandingsGetConstructorsByConstructorsChampionshipConstChampIdGet$Json
} from '../api/fn/standings/api-standings-get-constructors-by-constructors-championship-const-champ-id-get-json';

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
    return apiStandingsGetByDriversChampionshipIdDriversChampIdGet$Json(this.http, this.apiConfig.rootUrl, { driversChampId: driversChampId}).pipe(
      map(r => r.body as DriverLookUpDto[])
    )
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
      map(r => r.body as DriverSeasonResultDto[])
    );
  }
}
