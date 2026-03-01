import { Routes } from '@angular/router';
import { EmpresasPage } from './pages/empresas/empresas';

export const routes: Routes = [
    { path: 'empresas', component: EmpresasPage },
    { path: '', redirectTo: 'empresas', pathMatch: 'full' }
];
