export interface Fornecedor {
    id: number;
    nome: string;
    documento: string;
    email: string;
    cep: string;
    pessoaFisica: boolean;
    rg?: string | null;
    dataNascimento?: string | null;
    empresasCount?: number;
  }