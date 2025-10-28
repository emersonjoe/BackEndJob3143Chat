# SignalR Minimal API - Chat em Tempo Real

Esta é uma aplicação .NET 7 com Minimal API que utiliza SignalR para enviar mensagens em tempo real para todos os clientes conectados.

## Funcionalidades

- ✅ **Hub SignalR** para gerenciar conexões
- ✅ **Minimal API** com endpoints simples
- ✅ **Envio de mensagens** para todos os clientes conectados
- ✅ **Notificações** de conexão/desconexão
- ✅ **Página de teste** integrada
- ✅ **CORS configurado** para desenvolvimento

## Endpoints

### WebSocket Hub
- **`/chathub`** - Hub do SignalR para conexões WebSocket

### API REST
- **`GET /`** - Informações sobre a aplicação
- **`POST /api/sendmessage`** - Enviar mensagem para todos os clientes
- **`GET /test`** - Página de teste com interface web

## Como usar

### 1. Executar a aplicação

```bash
cd SignalRMinimalApijob3143
dotnet run
```

A aplicação estará disponível em: `https://localhost:7xxx` ou `http://localhost:5xxx`

### 2. Testar via interface web

Acesse `http://localhost:5xxx/test` no navegador para usar a interface de teste.

### 3. Enviar mensagem via API

```bash
curl -X POST http://localhost:5xxx/api/sendmessage \
  -H "Content-Type: application/json" \
  -d '{"user": "João", "message": "Olá, pessoal!"}'
```

### 4. Conectar cliente JavaScript

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

await connection.start();

// Escutar mensagens
connection.on("ReceiveMessage", (user, message) => {
    console.log(`${user}: ${message}`);
});

// Enviar mensagem
await connection.invoke("SendMessageToAll", "Meu Nome", "Minha mensagem");
```

## Estrutura do Projeto

```
SignalRMinimalApijob3143/
├── Program.cs          # Configuração da aplicação e endpoints
├── ChatHub.cs          # Hub SignalR para gerenciar conexões
├── *.csproj           # Arquivo do projeto .NET
└── README.md          # Este arquivo
```

## Métodos do Hub

### `SendMessageToAll(user, message)`
Envia uma mensagem para todos os clientes conectados.

### `OnConnectedAsync()`
Executado quando um cliente se conecta ao hub.

### `OnDisconnectedAsync()`
Executado quando um cliente se desconecta do hub.

## Eventos SignalR

### `ReceiveMessage`
Recebido quando uma nova mensagem é enviada.
- **Parâmetros**: `user` (string), `message` (string)

### `UserConnected`
Recebido quando um usuário se conecta.
- **Parâmetros**: `message` (string)

### `UserDisconnected`
Recebido quando um usuário se desconecta.
- **Parâmetros**: `message` (string)

## Tecnologias Utilizadas

- **.NET 7**
- **ASP.NET Core Minimal API**
- **SignalR**
- **HTML/JavaScript** (para teste)

## Desenvolvimento

Para desenvolvimento, a aplicação está configurada com CORS permitindo conexões de:
- `http://localhost:3000`
- `https://localhost:3001`

Você pode adicionar mais origens conforme necessário no arquivo `Program.cs`.