import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  ChoreDto,
  CreateChoreInput,
  CreateChoreOutput,
  ListChoresQuery
} from '../../features/chores/models/chore.model';

@Injectable({ providedIn: 'root' })
export class ChoresService {
  private readonly http = inject(HttpClient);

  list(query: ListChoresQuery = {}) {
    let params = new HttpParams();
    if (query.Title) params = params.set('Title', query.Title);
    if (query.Done !== undefined) params = params.set('Done', String(query.Done));
    return this.http.get<ChoreDto[]>('/bff/chores', { params });
  }

  create(input: CreateChoreInput) {
    return this.http.post<CreateChoreOutput>('/bff/chores', input);
  }

  delete(id: string) {
    return this.http.delete<void>(`/bff/chores/${id}`);
  }

  markDone(id: string) {
    return this.http.post<void>(`/bff/chores/${id}`, null);
  }
}
