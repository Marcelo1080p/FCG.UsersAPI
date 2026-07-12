# FCG.UsersAPI

Microsserviço de **usuários e autenticação** da plataforma FIAP Cloud Games (FCG).

Responsável pelo cadastro de usuários, login com JWT e gerenciamento administrativo (listagem, promoção a administrador e desativação de contas). Ao registrar um usuário, publica o evento `UserCreatedEvent` no RabbitMQ para os demais microsserviços.

## Arquitetura

Clean Architecture / DDD em 4 camadas:

```
src/
├── FCG.UsersAPI.Domain          # Entidades, enums, interfaces de repositório
├── FCG.UsersAPI.Application     # Commands/Queries (MediatR), eventos
├── FCG.UsersAPI.Infrastructure  # EF Core, repositórios, JWT
└── FCG.UsersAPI.API             # Controllers, configuração, Swagger
```

- **.NET 8** / ASP.NET Core
- **EF Core 8** + SQL Server
- **MassTransit** + RabbitMQ (mensageria)
- **MediatR** (CQRS)
- **BCrypt** (hash de senhas)
- **JWT Bearer** (autenticação)

## Endpoints

| Método | Rota | Autorização | Descrição |
|---|---|---|---|
| POST | `/api/auth/register` | Pública | Cadastra usuário e publica `UserCreatedEvent` |
| POST | `/api/auth/login` | Pública | Autentica e retorna token JWT |
| GET | `/api/users` | Admin | Lista todos os usuários |
| PATCH | `/api/users/{id}/promote` | Admin | Promove usuário a administrador |
| DELETE | `/api/users/{id}` | Admin | Desativa usuário |

## Eventos

| Evento | Direção | Descrição |
|---|---|---|
| `UserCreatedEvent` | Publica | Emitido após cadastro de usuário (UserId, Name, Email) |

## Variáveis de ambiente

| Variável | Descrição | Exemplo |
|---|---|---|
| `ConnectionStrings__Default` | Connection string do SQL Server | `Server=localhost\SQLEXPRESS;Database=FCG_UsersDB;Trusted_Connection=True;TrustServerCertificate=True` |
| `Jwt__Secret` | Chave de assinatura do JWT (mín. 32 caracteres) | — |
| `Jwt__Issuer` | Emissor do token | `FCG.UsersAPI` |
| `Jwt__Audience` | Audiência do token | `FCG` |
| `Jwt__ExpirationMinutes` | Validade do token em minutos | `60` |
| `RabbitMQ__Host` | Host do RabbitMQ | `localhost` |
| `RabbitMQ__Username` | Usuário do RabbitMQ | `guest` |
| `RabbitMQ__Password` | Senha do RabbitMQ | `guest` |

## Como executar

### Local

Pré-requisitos: .NET 8 SDK, SQL Server, RabbitMQ.

```bash
dotnet run --project src/FCG.UsersAPI.API
```

As migrations são aplicadas automaticamente na inicialização e um usuário administrador é criado (`admin@fcg.com` / `Admin@123`).

### Docker

```bash
docker build -t fcg-usersapi .
docker run -p 5001:8080 \
  -e ConnectionStrings__Default="..." \
  -e Jwt__Secret="..." \
  -e RabbitMQ__Host="rabbitmq" \
  fcg-usersapi
```

### Kubernetes

```bash
kubectl apply -f k8s/
```

Os manifests incluem Deployment, Service, ConfigMap e Secret.

## Testes

Os testes unitários (xUnit + NSubstitute) estão na branch `feature/testes-unitarios`:

```bash
dotnet test
```
