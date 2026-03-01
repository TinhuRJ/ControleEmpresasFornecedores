import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TableModule } from 'primeng/table';
import { ButtonModule } from 'primeng/button';
import { InputTextModule } from 'primeng/inputtext';
import { DialogModule } from 'primeng/dialog';
import { EmpresaService, EmpresaCreate } from '../../core/services/empresa.service';
import { Empresa } from '../../shared/models/empresa.model';
import { BehaviorSubject, combineLatest, map, startWith } from 'rxjs';

@Component({
  standalone: true,
  selector: 'app-empresas',
  imports: [CommonModule, FormsModule, TableModule, ButtonModule, InputTextModule, DialogModule],
  templateUrl: './empresas.html'
})
export class EmpresasPage {
  private readonly empresaService = inject(EmpresaService);

  private readonly empresasSubject = new BehaviorSubject<Empresa[]>([]);
  empresas$ = this.empresasSubject.asObservable();

  filtro = '';

  // dialog/form
  showDialog = false;
  saving = false;

  editingId: number | null = null;

  form: EmpresaCreate = {
    cnpj: '',
    nomeFantasia: '',
    cep: ''
  };

  constructor() {
    this.reload();
  }

  reload(): void {
    this.empresaService.getEmpresas().subscribe({
      next: (res) => this.empresasSubject.next(res),
      error: (err) => console.error('Erro ao buscar empresas:', err)
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

  private onlyDigits(value: string): string {
    return (value ?? '').replace(/\D/g, '');
  }

  isFormValid(): boolean {
    const cnpj = this.onlyDigits(this.form.cnpj);
    const cep = this.onlyDigits(this.form.cep);

    return (
      cnpj.length === 14 &&
      this.form.nomeFantasia.trim().length >= 2 &&
      cep.length === 8
    );
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
          console.error('Erro ao criar empresa:', err);
          this.saving = false;
        }
      });
      return;
    }
  
    // UPDATE
    const id = this.editingId;
    this.empresaService.updateEmpresa(id, payload).subscribe({
      next: () => {
        const updated = this.empresasSubject.value.map(e =>
          e.id === id ? { ...e, ...payload } : e
        );
        this.empresasSubject.next(updated);
        this.saving = false;
        this.showDialog = false;
      },
      error: (err) => {
        console.error('Erro ao atualizar empresa:', err);
        this.saving = false;
      }
    });
  }

  remove(empresa: Empresa): void {
    const ok = confirm(`Excluir a empresa "${empresa.nomeFantasia}"?`);
    if (!ok) return;
  
    this.empresaService.deleteEmpresa(empresa.id).subscribe({
      next: () => {
        this.empresasSubject.next(this.empresasSubject.value.filter(e => e.id !== empresa.id));
      },
      error: (err) => console.error('Erro ao excluir empresa:', err)
    });
  }
}