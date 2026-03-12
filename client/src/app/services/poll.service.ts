import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../api/api-configuration';
import {
  apiPollCreateUserIdPost,
  apiPollDeleteIdDelete,
  apiPollGetAllActiveGet$Json,
  apiPollGetByCreatorIdIdGet$Json,
  apiPollGetIdGet$Json,
  apiPollVotePollIdPollOptionIdUserIdPost
} from '../api/functions';
import {map} from 'rxjs/operators';
import {PollListDto} from '../api/models/poll-list-dto';
import {Observable} from 'rxjs';
import {PollDto} from '../api/models/poll-dto';
import {PollCreateDto} from '../api/models/poll-create-dto';

@Injectable({
  providedIn: 'root',
})
export class PollService {
  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) {}

  getAllActive(tag: string | undefined): Observable<PollListDto[]> {
    return apiPollGetAllActiveGet$Json(this.http, this.apiConfig.rootUrl, {tag: tag}).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as PollListDto[] ?? []
      })
    )
  }
  getPollsByUser(id: string, tag: string | undefined): Observable<PollListDto[]> {
    return apiPollGetByCreatorIdIdGet$Json(this.http, this.apiConfig.rootUrl, {id: id, tag: tag }).pipe(
      map(response => {
        return response.body as PollListDto[];

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

  createPoll(dto: PollCreateDto, userId: string) {
    return apiPollCreateUserIdPost(this.http, this.apiConfig.rootUrl, {body: dto, userId: userId}).pipe(
      map(() => void 0)
    );
  }

  removePoll(id: string){
    return apiPollDeleteIdDelete(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(() => void 0)
    );
  }
}
