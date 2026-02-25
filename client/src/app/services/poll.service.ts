import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {
  apiPollGetAllActiveGet$Json, apiPollGetIdGet$Json,
  apiPollRemoveVotePollIdPollOptionIdUserIdPost, apiPollVotePollIdPollOptionIdUserIdPost
} from '../api/functions';
import {map} from 'rxjs/operators';
import {PollListDto} from '../api/models/poll-list-dto';
import {Observable} from 'rxjs';
import {PollDto} from '../api/models/poll-dto';

@Injectable({
  providedIn: 'root',
})
export class PollService {
  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) {}


  getAllActive(): Observable<PollListDto[]> {
    return apiPollGetAllActiveGet$Json(this.http, this.apiConfig.rootUrl).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as PollListDto[] ?? []
      })
    )
  }

  getPoll(id: string): Observable<PollDto> {
    return apiPollGetIdGet$Json(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(response => {
        return response.body as PollDto;
      })
    )
  }

  vote(pollId: string, pollOptionId: string, userId: string): Observable<any> {
    return apiPollVotePollIdPollOptionIdUserIdPost(this.http, this.apiConfig.rootUrl, {
      pollId, pollOptionId, userId
    });
  }

  removeVote(pollId: string, pollOptionId: string, userId: string): Observable<any> {
    return apiPollRemoveVotePollIdPollOptionIdUserIdPost(this.http, this.apiConfig.rootUrl, {
      pollId, pollOptionId, userId
    });
  }
}
