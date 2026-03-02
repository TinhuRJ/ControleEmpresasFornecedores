import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Fornecedor } from '../../shared/models/fornecedor.model';

export interface FornecedorCreate {
  nome: string;
  documento: string;
  email: string;
  cep: string;
  pessoaFisica: boolean;
  rg?: string | null;
  dataNascimento?: string | null;
}

@Injectable({ providedIn: 'root' })
export class FornecedorService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Fornecedor`;

  getFornecedores(nome?: string, documento?: string): Observable<Fornecedor[]> {
    let params = new HttpParams();
    if (nome) params = params.set('nome', nome);
    if (documento) params = params.set('documento', documento);

    return this.http.get<Fornecedor[]>(`${this.apiUrl}/GetFornecedores`, { params });
  }

  createFornecedor(payload: FornecedorCreate): Observable<Fornecedor> {
    return this.http.post<Fornecedor>(`${this.apiUrl}/CreateFornecedor`, payload);
  }

  updateFornecedor(id: number, payload: FornecedorCreate) {
    return this.http.put<void>(`${this.apiUrl}/UpdateFornecedor/${id}`, payload);
  }
  
  deleteFornecedor(id: number) {
    return this.http.delete<void>(`${this.apiUrl}/DeleteFornecedor/${id}`);
  }

  getEmpresasIdsByFornecedor(fornecedorId: number): Observable<number[]> {
    return this.http.get<number[]>(`${this.apiUrl}/GetEmpresasIdsByFornecedor/${fornecedorId}`);
  }
  
  vincularEmpresasAoFornecedor(fornecedorId: number, ids: number[]): Observable<number> {
    return this.http.post<number>(`${this.apiUrl}/VincularEmpresasAoFornecedor`, {
      fornecedorId,
      empresasIds: ids
    });
  }
}