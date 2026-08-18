# Sabemi — Webhook de Pagamentos

Sistema de recebimento e monitoramento de notificações de pagamento (webhooks) enviadas por banco parceiro.

## Arquitetura

```
banco parceiro
     │
     ▼ POST /webhooks/pagamento (X-Api-Key)
┌────────────────────────────────────┐
│         .NET 8 Web API             │
│  ┌──────────────┐ ┌─────────────┐  │
│  │  Controller  │ │ BG Service  │  │
│  │  (resposta   │ │(proc. 2s +  │  │
│  │   imediata)  │ │ negócio)    │  │
│  └──────┬───────┘ └──────▲──────┘  │
│         │  Channel<Guid>  │        │
│         └────────────────┘         │
└──────────────────┬─────────────────┘
                   │
              PostgreSQL
         ┌─────────┴──────────┐
         │                    │
  log_eventos_brutos   status_contratos
```

```
Next.js Dashboard ──(polling 5s)──► GET /api/pagamentos
                                    GET /api/stats
```

## Pré-requisitos

- Docker + Docker Compose  
  **ou**  
- .NET 8 SDK + Node.js 20 + PostgreSQL 16

---

## Execução com Docker Compose (recomendado)

```bash
docker compose up --build
```

| Serviço  | URL                           |
|----------|-------------------------------|
| API      | http://localhost:5000         |
| Swagger  | http://localhost:5000/swagger |
| Frontend | http://localhost:3000         |
| Postgres | localhost:5432                |

---

## Execução local (sem Docker)

### Backend

```bash
# Suba o PostgreSQL (ou ajuste a connection string em appsettings.Development.json)
cd backend/src/Sabemi.Webhooks.API
dotnet run
```

API disponível em `http://localhost:5000`.

### Frontend

```bash
cd frontend
npm install
NEXT_PUBLIC_API_URL=http://localhost:5000 npm run dev
```

Dashboard disponível em `http://localhost:3000`.

---

## Testando o webhook

### Envio válido

```bash
curl -X POST http://localhost:5000/webhooks/pagamento \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: sabemi-secret-key-2024" \
  -d '{
    "id_transacao": "TXN-001",
    "id_contrato": "CTR-ABC",
    "valor": 1500.00,
    "data_pagamento": "2024-08-18T10:00:00Z",
    "status": "sucesso"
  }'
```

### Idempotência (reenvio do mesmo id_transacao)

```bash
# O segundo envio retorna 200 OK sem reprocessar
curl -X POST http://localhost:5000/webhooks/pagamento \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: sabemi-secret-key-2024" \
  -d '{"id_transacao":"TXN-001","id_contrato":"CTR-ABC","valor":1500.00,"data_pagamento":"2024-08-18T10:00:00Z","status":"sucesso"}'
```

### Status inválido (gera erro de validação de negócio)

```bash
curl -X POST http://localhost:5000/webhooks/pagamento \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: sabemi-secret-key-2024" \
  -d '{"id_transacao":"TXN-ERR-01","id_contrato":"CTR-XYZ","valor":500.00,"data_pagamento":"2024-08-18T10:00:00Z","status":"liquidado_parcial"}'
```

### Sem API Key (retorna 401)

```bash
curl -X POST http://localhost:5000/webhooks/pagamento \
  -H "Content-Type: application/json" \
  -d '{"id_transacao":"TXN-002",...}'
```

---

## Endpoints da API

| Método | Rota                  | Auth     | Descrição                          |
|--------|-----------------------|----------|------------------------------------|
| POST   | /webhooks/pagamento   | X-Api-Key| Recebe notificação do banco        |
| GET    | /api/pagamentos       | —        | Lista eventos (filtros: status, id_contrato) |
| GET    | /api/stats            | —        | Totais (total, processados, erros, pendentes) |
| GET    | /api/contratos        | —        | Status consolidado por contrato    |

---

## Decisões de design

| Requisito          | Solução                                                                        |
|--------------------|--------------------------------------------------------------------------------|
| Segurança          | `X-Api-Key` no header validado por `ApiKeyAuthFilter` (IActionFilter)          |
| Idempotência       | Índice UNIQUE em `id_transacao` + check antes do insert + catch de conflito    |
| Resposta rápida    | Controller persiste o evento e enfileira em `Channel<Guid>`, retorna 202 imediatamente |
| Processamento pesado | `BackgroundService` lê o channel e processa com `Task.Delay(2s)` simulando regra de negócio |
| Persistência       | EF Core + PostgreSQL: `log_eventos_brutos` (log bruto) + `status_contratos` (estado consolidado) |
| Dashboard          | Next.js 14 + Tailwind, polling a cada 5 s, filtros por status e contrato       |
| Alerta visual      | Linhas com `erro != null` recebem borda vermelha e badge de alerta na tabela   |
