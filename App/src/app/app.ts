import { Component, signal, OnInit, inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ButtonModule } from 'primeng/button';
import { EmpresaService } from './core/services/empresa.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, ButtonModule],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App implements OnInit {
  protected readonly title = signal('App');

  private readonly empresaService = inject(EmpresaService);

  ngOnInit(): void {
    this.empresaService.getEmpresas().subscribe({
      next: (res) => console.log('Empresas:', res),
      error: (err) => console.error('Erro ao buscar empresas:', err)
    });
  }
}