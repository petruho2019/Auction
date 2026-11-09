import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AuctionResponse } from '../models/auction-response';

@Injectable({ providedIn: 'root' })
export class AuctionService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = '/api/v1/auctions/create';

  create(formData: FormData): Observable<AuctionResponse> {
    return this.http.post<AuctionResponse>(this.apiUrl, formData);
  }
}
