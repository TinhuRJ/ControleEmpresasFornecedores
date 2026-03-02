import { Routes } from '@angular/router';
import { EmpresasPage } from './pages/empresas/empresas';
import { FornecedoresPage } from './pages/fornecedores/fornecedores';

export const routes: Routes = [
    { path: 'empresas', component: EmpresasPage },
    { path: 'fornecedores', component: FornecedoresPage },
    { path: '', redirectTo: 'empresas', pathMatch: 'full' }
];
