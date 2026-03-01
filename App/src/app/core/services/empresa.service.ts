import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Empresa } from '../../shared/models/empresa.model';

export interface EmpresaCreate {
  cnpj: string;
  nomeFantasia: string;
  cep: string;
}

@Injectable({ providedIn: 'root' })
export class EmpresaService {
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/Empresas`;

  getEmpresas(): Observable<Empresa[]> {
    return this.http.get<Empresa[]>(`${this.apiUrl}/GetEmpresas`);
  }

  createEmpresa(payload: EmpresaCreate): Observable<Empresa> {
    return this.http.post<Empresa>(`${this.apiUrl}/CreateEmpresa`, payload);
  }

  updateEmpresa(id: number, payload: EmpresaCreate): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/UpdateEmpresa/${id}`, payload);
  }
  
  deleteEmpresa(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/DeleteEmpresa/${id}`);
  }
}