import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { CheckboxModule } from 'primeng/checkbox';
import { MultiSelectModule } from 'primeng/multiselect';

import { BehaviorSubject } from 'rxjs';
import { Fornecedor } from '../../shared/models/fornecedor.model';
import { FornecedorCreate, FornecedorService } from '../../core/services/fornecedor.service';
import { EmpresaService } from '../../core/services/empresa.service';
import { Empresa } from '../../shared/models/empresa.model';

import { MessageService } from 'primeng/api';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  standalone: true,
  selector: 'app-fornecedores',
  imports: [
    CommonModule,
    FormsModule,
    TableModule,
    ButtonModule,
    InputTextModule,
    DialogModule,
    CheckboxModule,
    MultiSelectModule
  ],
  templateUrl: './fornecedores.html'
})
export class FornecedoresPage {
  private readonly fornecedorService = inject(FornecedorService);
  private readonly empresaService = inject(EmpresaService);
  private readonly messageService = inject(MessageService);

  private readonly fornecedoresSubject = new BehaviorSubject<Fornecedor[]>([]);
  fornecedores$ = this.fornecedoresSubject.asObservable();

  editingId: number | null = null;

  // filtros
  filtroNome = '';
  filtroDocumento = '';

  // dialog/form fornecedor
  showDialog = false;
  saving = false;

  form: FornecedorCreate = {
    nome: '',
    documento: '',
    email: '',
    cep: '',
    pessoaFisica: false,
    rg: null,
    dataNascimento: null
  };
  
  showVinculoDialog = false;
  fornecedorSelecionadoId: number | null = null;

  empresasTodas: Empresa[] = [];
  empresasSelecionadasIds: number[] = [];

  loadingVinculo = false;
  savingVinculo = false;

  constructor() {
    this.buscar();
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

  buscar(): void {
    const nome = this.filtroNome.trim() || undefined;
    const documento = this.onlyDigits(this.filtroDocumento) || undefined;

    this.fornecedorService.getFornecedores(nome, documento).subscribe({
      next: (res) => this.fornecedoresSubject.next(res),
      error: (err) => this.showApiError(err, 'Não foi possível buscar fornecedores.')
    });
  }

  limparFiltros(): void {
    this.filtroNome = '';
    this.filtroDocumento = '';
    this.buscar();
  }

  openCreate(): void {
    this.editingId = null;
    this.saving = false;

    this.form = {
      nome: '',
      documento: '',
      email: '',
      cep: '',
      pessoaFisica: false,
      rg: null,
      dataNascimento: null
    };

    this.showDialog = true;
  }

  openEdit(f: Fornecedor): void {
    this.editingId = f.id;
    this.saving = false;

    this.form = {
      nome: f.nome ?? '',
      documento: f.documento ?? '',
      email: f.email ?? '',
      cep: f.cep ?? '',
      pessoaFisica: !!f.pessoaFisica,
      rg: f.rg ?? null,
      dataNascimento: f.dataNascimento ?? null
    };

    this.showDialog = true;
  }

  closeDialog(): void {
    this.showDialog = false;
  }

  isFormValid(): boolean {
    const doc = this.onlyDigits(this.form.documento);
    const cep = this.onlyDigits(this.form.cep);

    const docOk = doc.length === 11 || doc.length === 14; // CPF 11 / CNPJ 14
    const cepOk = cep.length === 8;
    const emailOk = this.form.email.includes('@') && this.form.email.includes('.');
    const nomeOk = this.form.nome.trim().length >= 2;

    if (!this.form.pessoaFisica) return docOk && cepOk && emailOk && nomeOk;

    // PF: exige RG e data nascimento
    const rgOk = (this.form.rg ?? '').trim().length >= 3;
    const dnOk = !!this.form.dataNascimento;

    return docOk && cepOk && emailOk && nomeOk && rgOk && dnOk;
  }

  save(): void {
    if (this.saving) return;

    if (!this.isFormValid()) {
      alert('Preencha corretamente os campos obrigatórios.');
      return;
    }

    this.saving = true;

    const payload: FornecedorCreate = {
      nome: this.form.nome.trim(),
      documento: this.onlyDigits(this.form.documento),
      email: this.form.email.trim(),
      cep: this.onlyDigits(this.form.cep),
      pessoaFisica: this.form.pessoaFisica,
      rg: this.form.pessoaFisica ? (this.form.rg ?? '').trim() : null,
      dataNascimento: this.form.pessoaFisica ? this.form.dataNascimento : null
    };

    // CREATE
    if (this.editingId === null) {
      this.fornecedorService.createFornecedor(payload).subscribe({
        next: (created) => {
          this.fornecedoresSubject.next([created, ...this.fornecedoresSubject.value]);
          this.saving = false;
          this.showDialog = false;
        },
        error: (err) => {
          this.saving = false;
          this.showApiError(err, 'Não foi possível salvar o fornecedor.');
        }
      });
      return;
    }

    // UPDATE
    const id = this.editingId;
    this.fornecedorService.updateFornecedor(id, payload).subscribe({
      next: () => {
        const updated = this.fornecedoresSubject.value.map(x =>
          x.id === id ? { ...x, ...payload } : x
        );
        this.fornecedoresSubject.next(updated);
        this.saving = false;
        this.showDialog = false;
      },
      error: (err) => {
        this.saving = false;
        this.showApiError(err, 'Não foi possível atualizar o fornecedor.');
      }
    });
  }

  remove(f: Fornecedor): void {
    const ok = confirm(`Excluir o fornecedor "${f.nome}"?`);
    if (!ok) return;

    this.fornecedorService.deleteFornecedor(f.id).subscribe({
      next: () => {
        this.fornecedoresSubject.next(this.fornecedoresSubject.value.filter(x => x.id !== f.id));
      },
      error: (err) => {
        this.showApiError(err, 'Não foi possível excluir o fornecedor.');
      }
    });
  }

  openVinculosEmpresas(fornecedorId: number): void {
    this.fornecedorSelecionadoId = fornecedorId;
    this.showVinculoDialog = true;
    this.loadingVinculo = true;

    // Carrega empresas + ids vinculados
    this.empresaService.getEmpresas().subscribe({
      next: (empresas) => {
        this.empresasTodas = empresas;

        this.fornecedorService.getEmpresasIdsByFornecedor(fornecedorId).subscribe({
          next: (ids) => {
            this.empresasSelecionadasIds = ids ?? [];
            this.loadingVinculo = false;
          },
          error: (err) => {
            this.empresasSelecionadasIds = [];
            this.loadingVinculo = false;
            this.showApiError(err, 'Não foi possível carregar os vínculos do fornecedor.');
          }
        });
      },
      error: (err) => {
        this.empresasTodas = [];
        this.empresasSelecionadasIds = [];
        this.loadingVinculo = false;
        this.showApiError(err, 'Não foi possível carregar empresas.');
      }
    });
  }

  saveVinculosEmpresas(): void {
    if (this.fornecedorSelecionadoId == null) return;
    if (this.savingVinculo) return;

    const fornecedorId = this.fornecedorSelecionadoId;
    const ids = [...this.empresasSelecionadasIds];

    this.savingVinculo = true;
    
    this.fornecedorService.vincularEmpresasAoFornecedor(fornecedorId, ids).subscribe({
      next: (count) => {        
        const updated = this.fornecedoresSubject.value.map(f =>
          f.id === fornecedorId ? { ...f, empresasCount: count } : f
        );
        this.fornecedoresSubject.next(updated);

        this.savingVinculo = false;
        this.showVinculoDialog = false;
        this.fornecedorSelecionadoId = null;
        this.empresasSelecionadasIds = [];
      },
      error: (err) => {
        this.savingVinculo = false;
        this.showApiError(err, 'Não foi possível salvar os vínculos.');
      }
    });
  }

  closeVinculosEmpresas(): void {
    this.showVinculoDialog = false;
    this.fornecedorSelecionadoId = null;
    this.empresasSelecionadasIds = [];
  }
}