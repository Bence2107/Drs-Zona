import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {ApiConfiguration} from '../../api/api-configuration';
import {
  apiPollCreatePost,
  apiPollDeleteIdDelete,
  apiPollGetAllActiveGet$Json, apiPollGetByCreatorGet$Json,
  apiPollGetIdGet$Json, apiPollVotePollIdPollOptionIdPost,
} from '../../api/functions';
import {map} from 'rxjs/operators';
import {PollListDto} from '../../api/models/poll-list-dto';
import {Observable} from 'rxjs';
import {PollDto} from '../../api/models/poll-dto';
import {PollCreateDto} from '../../api/models/poll-create-dto';

@Injectable({
  providedIn: 'root',
})
export class PollService {
  constructor(private http: HttpClient, private apiConfig: ApiConfiguration) {}

  getPollById(id: string): Observable<PollDto> {
    return apiPollGetIdGet$Json(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(response => {
        return response.body as PollDto;
      })
    )
  }
  getPollsByCreator(tag: string | undefined): Observable<PollListDto[]> {
    return apiPollGetByCreatorGet$Json(this.http, this.apiConfig.rootUrl, {tag: tag }).pipe(
      map(response => {
        return response.body as PollListDto[];

      })
    )
  }

  getAllActive(tag: string | undefined): Observable<PollListDto[]> {
    return apiPollGetAllActiveGet$Json(this.http, this.apiConfig.rootUrl, {tag: tag}).pipe(
      map(response => {
        const body = response.body as any;
        return body.value as PollListDto[] ?? []
      })
    )
  }

  createPoll(dto: PollCreateDto) {
    return apiPollCreatePost(this.http, this.apiConfig.rootUrl, {body: dto}).pipe(
      map(() => void 0)
    );
  }

  vote(pollId: string, pollOptionId: string): Observable<any> {
    return apiPollVotePollIdPollOptionIdPost(this.http, this.apiConfig.rootUrl, {
      pollId, pollOptionId
    });
  }

  deletePoll(id: string){
    return apiPollDeleteIdDelete(this.http, this.apiConfig.rootUrl, {id: id}).pipe(
      map(() => void 0)
    );
  }
}
