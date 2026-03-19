import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../../api/api-configuration';
import {
  apiContractsCreatePost, apiContractsDeleteIdDelete,
  apiContractsGetAllGet$Json,
  apiContractsUpdateIdDriverIdTeamIdPost
} from '../../api/functions';
import {map} from 'rxjs/operators';
import {DriverListDto} from '../../api/models/driver-list-dto';
import {Observable} from 'rxjs';
import {ContractCreateDto} from '../../api/models/contract-create-dto';

@Injectable({
  providedIn: 'root',
})
export class ContractsService {
  constructor(
    protected http: HttpClient,
    protected apiConfig: ApiConfiguration
  ) { }


  getAllContracts(): Observable<DriverListDto[]> {
    return apiContractsGetAllGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => response.body as DriverListDto[])
    )
  }

  createContract(dto: ContractCreateDto) {
    return apiContractsCreatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(map(() => void 0));
  }

  updateContract(contractId: string, driverId: any, teamId: any) {
    return apiContractsUpdateIdDriverIdTeamIdPost(this.http, this.apiConfig.rootUrl,
      {id: contractId, driverId: driverId, teamId: teamId}).pipe(map(() => void 0));
  }

  deleteContract(contractId: string) {
    return apiContractsDeleteIdDelete(this.http, this.apiConfig.rootUrl,
      {id: contractId}).pipe(map(() => void 0));
  }
}
