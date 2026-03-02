Controle de Empresas e Fornecedores

Projeto de estudo de Controle de Empresas e Fornecedores, desenvolvido com foco em aprendizado e boas práticas de desenvolvimento full-stack.

Tecnologias utilizadas

Front-end: Angular (Standalone Components) + TypeScript + PrimeNG

Back-end: .NET 6 (C#)

Banco de dados: SQL Server, utilizando Migrations (Entity Framework Core) para versionamento e controle do esquema

ORM: Entity Framework Core

Estilo: PrimeNG + CSS simples para telas de CRUD e formulários

Arquitetura: API REST + consumo via serviços dedicados no front-end

Funcionalidades

Cadastro, listagem, edição e exclusão de Empresas

Cadastro, listagem, edição e exclusão de Fornecedores

Relacionamento N:N entre Empresas e Fornecedores

Vínculo e desvínculo de fornecedores às empresas

Regras de negócio implementadas:

CNPJ e CPF com unicidade no banco de dados

Fornecedor pode ser Pessoa Física ou Jurídica

Pessoa Física exige RG e Data de Nascimento

Caso a empresa seja do Paraná, não permite cadastrar fornecedor Pessoa Física menor de idade

Validação de CEP via API externa (cep.la) no backend

Filtros na listagem de fornecedores por Nome e CPF/CNPJ

Contagem dinâmica de fornecedores vinculados por empresa

Validações de formulário no front-end

Tratamento de erros da API com exibição de mensagens ao usuário

Estrutura do projeto

ControleEmpresasFornecedores/

Api/ → projeto .NET 6, APIs REST, Entities, Services, Controllers e Migrations

App/ → projeto Angular, telas de CRUD, formulários e vínculo N:N

Serviços dedicados para consumo das APIs (EmpresaService, FornecedorService)

Observações

Projeto desenvolvido para estudo e prática de desenvolvimento full-stack

Banco de dados utilizado: SQL Server, com Migrations para criação e atualização do schema

O front-end consome as APIs do back-end via endpoints locais

Relacionamento N:N implementado com tabela intermediária (EmpresaFornecedor)

Tratamento de regras de negócio realizado no backend, mantendo o front-end responsável apenas pela apresentação e consumo das APIs
