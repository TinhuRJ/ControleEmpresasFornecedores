import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { EmpresaService, EmpresaCreate } from '../../core/services/empresa.service';
import { Empresa } from '../../shared/models/empresa.model';
import { BehaviorSubject } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { MessageService } from 'primeng/api';
import { MultiSelectModule } from 'primeng/multiselect';
import { FornecedorService } from '../../core/services/fornecedor.service';
import { Fornecedor } from '../../shared/models/fornecedor.model';

@Component({
  standalone: true,
  selector: 'app-empresas',
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule, DialogModule, MultiSelectModule],
  templateUrl: './empresas.html'
})
export class EmpresasPage {
  private readonly empresaService = inject(EmpresaService);
  private readonly fornecedorService = inject(FornecedorService);
  private readonly messageService = inject(MessageService);

  private readonly empresasSubject = new BehaviorSubject<Empresa[]>([]);
  empresas$ = this.empresasSubject.asObservable();  

  filtro = '';

  // dialog/form (empresa)
  showDialog = false;
  saving = false;
  editingId: number | null = null;

  form: EmpresaCreate = {
    cnpj: '',
    nomeFantasia: '',
    cep: ''
  };

  // dialog vínculo
  showVinculoDialog = false;
  empresaSelecionadaId: number | null = null;

  fornecedoresTodos: Fornecedor[] = [];
  fornecedorSelecionadosIds: number[] = [];

  loadingVinculo = false;
  savingVinculo = false;

  constructor() {
    this.reload();
  }

  private showApiError(err: unknown, fallback: string): void {
    const httpErr = err as HttpErrorResponse;
    const backendMsg =
      (httpErr?.error && (httpErr.error.message || httpErr.error.mensagem || httpErr.error.title)) ||
      httpErr?.message;

    this.messageService.add({
      severity: 'error',
      summary: 'Erro',
      detail: (backendMsg ? String(backendMsg) : fallback)
    });
  }

  private onlyDigits(value: string): string {
    return (value ?? '').replace(/\D/g, '');
  }

  reload(): void {
    this.empresaService.getEmpresas().subscribe({
      next: (res) => this.empresasSubject.next(res),
      error: (err) => this.showApiError(err, 'Não foi possível carregar as empresas.')
    });
  }

  openCreate(): void {
    this.editingId = null;
    this.form = { cnpj: '', nomeFantasia: '', cep: '' };
    this.showDialog = true;
  }

  openEdit(empresa: Empresa): void {
    this.editingId = empresa.id;
    this.form = {
      cnpj: empresa.cnpj ?? '',
      nomeFantasia: empresa.nomeFantasia ?? '',
      cep: empresa.cep ?? ''
    };
    this.showDialog = true;
  }

  closeDialog(): void {
    this.showDialog = false;
  }

  isFormValid(): boolean {
    const cnpj = this.onlyDigits(this.form.cnpj);
    const cep = this.onlyDigits(this.form.cep);

    return cnpj.length === 14 && this.form.nomeFantasia.trim().length >= 2 && cep.length === 8;
  }

  save(): void {
    if (!this.isFormValid()) return;

    this.saving = true;

    const payload: EmpresaCreate = {
      cnpj: this.onlyDigits(this.form.cnpj),
      nomeFantasia: this.form.nomeFantasia.trim(),
      cep: this.onlyDigits(this.form.cep)
    };

    // CREATE
    if (this.editingId === null) {
      this.empresaService.createEmpresa(payload).subscribe({
        next: (created) => {
          this.empresasSubject.next([created, ...this.empresasSubject.value]);
          this.saving = false;
          this.showDialog = false;
        },
        error: (err) => {
          this.saving = false;
          this.showApiError(err, 'Não foi possível salvar a empresa.');
        }
      });
      return;
    }

    // UPDATE
    const id = this.editingId;
    this.empresaService.updateEmpresa(id, payload).subscribe({
      next: () => {
        const updated = this.empresasSubject.value.map((e) => (e.id === id ? { ...e, ...payload } : e));
        this.empresasSubject.next(updated);
        this.saving = false;
        this.showDialog = false;
      },
      error: (err) => {
        this.saving = false;
        this.showApiError(err, 'Não foi possível atualizar a empresa.');
      }
    });
  }

  remove(empresa: Empresa): void {
    const ok = confirm(`Excluir a empresa "${empresa.nomeFantasia}"?`);
    if (!ok) return;
  
    this.empresaService.deleteEmpresa(empresa.id).subscribe({
      next: () => {
        const updated = this.empresasSubject.value
          .filter(e => e.id !== empresa.id);
  
        this.empresasSubject.next(updated);
      },
      error: (err) => 
        this.showApiError(err, 'Não foi possível excluir a empresa.')
    });
  }

  openVinculos(empresaId: number): void {
    this.empresaSelecionadaId = empresaId;
    this.showVinculoDialog = true;
    this.loadingVinculo = true;

    this.fornecedorService.getFornecedores().subscribe({
      next: (todos) => {
        this.fornecedoresTodos = todos;

        this.empresaService.getFornecedoresIdsByEmpresa(empresaId).subscribe({
          next: (ids) => {
            this.fornecedorSelecionadosIds = ids ?? [];
            this.loadingVinculo = false;
          },
          error: (err) => {
            this.fornecedorSelecionadosIds = [];
            this.loadingVinculo = false;
            this.showApiError(err, 'Não foi possível carregar os vínculos.');
          }
        });
      },
      error: (err) => {
        this.loadingVinculo = false;
        this.showApiError(err, 'Não foi possível carregar fornecedores.');
      }
    });
  }

  saveVinculos(): void {
    if (this.empresaSelecionadaId == null) return;
    if (this.savingVinculo) return;
  
    const empresaId = this.empresaSelecionadaId;
    const ids = [...this.fornecedorSelecionadosIds];
  
    this.savingVinculo = true;
  
    this.empresaService.vincularFornecedores(empresaId, ids).subscribe({
      next: (count) => {
        // atualiza a contagem na linha da empresa sem reload
        const updated = this.empresasSubject.value.map(e =>
          e.id === empresaId ? { ...e, fornecedoresCount: count } : e
        );
        this.empresasSubject.next(updated);
  
        // fecha modal e limpa estado
        this.savingVinculo = false;
        this.showVinculoDialog = false;
        this.empresaSelecionadaId = null;
        this.fornecedorSelecionadosIds = [];
      },
      error: (err) => {
        this.savingVinculo = false;
        this.showApiError(err, 'Não foi possível salvar os vínculos.');
      }
    });
  }

  closeVinculoDialog(): void {
    this.showVinculoDialog = false;
    this.empresaSelecionadaId = null;
    this.fornecedorSelecionadosIds = [];
  }
}