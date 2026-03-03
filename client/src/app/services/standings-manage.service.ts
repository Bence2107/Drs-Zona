import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';

@Injectable({
  providedIn: 'root',
})
export class StandingsManageService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }



}
