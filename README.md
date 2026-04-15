# 🚀 TaskFlow — Sistema de Gerenciamento de Tarefas

TaskFlow é um sistema de gerenciamento de tarefas desenvolvido para pequenas equipes organizarem e acompanharem suas atividades, atribuindo tarefas a responsáveis, controlando prioridades e monitorando o status das atividades.

---

## 🧱 Arquitetura

O projeto segue uma arquitetura em camadas com separação clara de responsabilidades:

```
BackEnd
├── BackEnd.Dominio         → Entidades, enumeradores e regras de negócio
├── BackEnd.Repositories    → Acesso ao banco de dados (Entity Framework Core)
├── BackEnd.Application     → Serviços e orquestração das regras de negócio
├── BackEnd.Services        → Serviços externos (JWT)
└── BackEnd.API             → Controllers, Models e configuração da API REST
```

---

## 🛠️ Tecnologias Utilizadas

- **.NET 9** — Framework principal
- **C#** — Linguagem de programação
- **Entity Framework Core 9** — ORM para acesso ao banco de dados
- **SQLite** — Banco de dados
- **BCrypt.Net** — Hash de senhas
- **JWT (JSON Web Token)** — Autenticação e autorização
- **Swagger** — Documentação e teste dos endpoints

---

## 📦 Entidades do Sistema

### Usuario
- `Id`, `Nome`, `Email`, `Funcao`, `SenhaHash`, `Ativo`
- Soft delete — usuários não são removidos, apenas marcados como inativos

### Tarefa
- `Id`, `Titulo`, `Descricao`, `Prazo`, `Prioridade`, `Status`, `DataCriada`

### UsuarioTarefa
- Relacionamento N:N entre usuários e tarefas
- `Id`, `UsuarioId`, `TarefaId`, `DataAtribuicao`

---

## 🔐 Permissões por Função

| Ação | Desenvolvedor | Gerente | Administrador |
|------|:---:|:---:|:---:|
| Visualizar tarefas | ✅ | ✅ | ✅ |
| Atualizar status de tarefas atribuídas | ✅ | ✅ | ✅ |
| Criar tarefas | ❌ | ✅ | ✅ |
| Atribuir responsáveis | ❌ | ✅ | ✅ |
| Definir prioridade | ❌ | ✅ | ✅ |
| Gerenciar usuários | ❌ | ❌ | ✅ |
| Acesso total | ❌ | ❌ | ✅ |

---

## 🚀 Como Rodar o Projeto

### Pré-requisitos
- .NET 9 SDK instalado
- Git

### Passo a passo

```bash
# Clone o repositório
git clone https://github.com/seu-usuario/TaskFlow.git

# Acesse a pasta do backend
cd TaskFlow/BackEnd

# Restaure os pacotes
dotnet restore

# Execute as migrations
cd BackEnd.Repositories
dotnet ef database update

# Execute a API
cd ../BackEnd.API
dotnet run
```

A API estará disponível em `http://localhost:5000`

O Swagger estará disponível em `http://localhost:5000/swagger`

---

## 📡 Endpoints Principais

### Autenticação
| Método | Rota | Descrição |
|--------|------|-----------|
| POST | `/api/auth/Login` | Realiza login e retorna o token JWT |

### Usuários
| Método | Rota | Descrição | Permissão |
|--------|------|-----------|-----------|
| POST | `/api/usuario/CriarUsuario` | Cria um novo usuário | Administrador |
| GET | `/api/usuario/ListarUsuarios` | Lista usuários | Autenticado |
| GET | `/api/usuario/ObterUsuarioPorId/{id}` | Busca usuário por ID | Autenticado |
| PUT | `/api/usuario/AtualizarUsuario` | Atualiza usuário | Administrador, Gerente |
| PUT | `/api/usuario/AtualizarSenhaUsuario` | Atualiza senha | Próprio usuário ou Administrador |
| DELETE | `/api/usuario/DeletarUsuario/{id}` | Soft delete do usuário | Administrador |
| PUT | `/api/usuario/RestaurarUsuario/{id}` | Restaura usuário inativo | Administrador |

### Tarefas
| Método | Rota | Descrição | Permissão |
|--------|------|-----------|-----------|
| POST | `/api/tarefa/CriarTarefa` | Cria uma tarefa | Administrador, Gerente |
| GET | `/api/tarefa/ListarTarefas` | Lista todas as tarefas | Autenticado |
| GET | `/api/tarefa/ObterTarefaPorId/{id}` | Busca tarefa por ID | Autenticado |
| GET | `/api/tarefa/ListarTarefasPorStatus` | Filtra por status | Autenticado |
| GET | `/api/tarefa/ListarTarefasPorPrioridade` | Filtra por prioridade | Autenticado |
| GET | `/api/tarefa/ListarTarefasPorUsuario` | Filtra por usuário | Autenticado |
| PUT | `/api/tarefa/AtualizarTarefa` | Atualiza tarefa completa | Administrador, Gerente |
| PUT | `/api/tarefa/AtualizarStatusTarefa` | Atualiza só o status | Desenvolvedor (tarefas atribuídas) |
| DELETE | `/api/tarefa/DeletarTarefa/{id}` | Remove tarefa | Administrador, Gerente |

### Atribuições
| Método | Rota | Descrição | Permissão |
|--------|------|-----------|-----------|
| POST | `/api/usuariotarefa/CriarRelacionamento` | Atribui usuário a tarefa | Administrador, Gerente |
| GET | `/api/usuariotarefa/ObterRelacionamentoPorUsuario` | Tarefas de um usuário | Autenticado |
| GET | `/api/usuariotarefa/ObterRelacionamentoPorTarefa` | Usuários de uma tarefa | Autenticado |
| DELETE | `/api/usuariotarefa/DeletarRelacionamento/{id}` | Remove atribuição | Administrador, Gerente |

### Relatório
| Método | Rota | Descrição |
|--------|------|-----------|
| GET | `/api/relatorio/ResumoTarefas` | Total e contagem por status |
| GET | `/api/relatorio/TarefasRecentes` | Últimas 5 tarefas |
| GET | `/api/relatorio/ProgressoGeral` | Percentual de conclusão |
| GET | `/api/relatorio/ResumoEquipe` | Usuários e quantidade de tarefas |

---

## 🔑 Enumeradores

**UsuarioFuncao**
- `1` = Desenvolvedor
- `2` = Gerente
- `3` = Administrador

**PrioridadeTarefa**
- `1` = Alta
- `2` = Media
- `3` = Baixa

**StatusTarefa**
- `1` = AFazer
- `2` = EmAndamento
- `3` = Concluido

---

## 🔒 Autenticação

A API utiliza autenticação via JWT Bearer Token.

Para acessar endpoints protegidos, inclua o token no header:

```
Authorization: Bearer {seu_token}
```

Obtenha o token fazendo login em `POST /api/auth/Login` com email e senha.

---

## 👨‍💻 Autor

Desenvolvido por **Nickolas** como projeto de estudo aplicando conceitos de arquitetura em camadas, modelagem de domínio, API REST e autenticação JWT.