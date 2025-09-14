# Inventory Management API - Intru��es de Execu��o

Este documento descreve como executar a **Inventory Management API** usando **.NET 9**, tanto localmente pelo Visual Studio quanto via **Docker Compose**, incluindo o Jaeger para visualiza��o de traces.

## Pr�-requisitos

1. .NET 9 SDK
2. Visual Studio 2022 ou VS Code
3. Docker Desktop
5. WSL2 (caso use Windows + Docker Desktop)

## Op��o 1: Executando pelo Visual Studio 2022

1. Abra a solu��o `.sln` no Visual Studio 2022.
2. Certifique-se de selecionar o **projeto API** (`Inventory.Management.API`) como projeto de inicializa��o.
3. Configure o **Perfil de Execu��o** para `IIS Express`.
4. Pressione **F5** ou clique em **Run**.
5. A aplica��o ser� executada em **https://localhost:{PORT}**.
6. Para acessar a documenta��o Swagger:

```
https://localhost:{PORT}/swagger
```

7. Para ver m�tricas de traces, � necess�rio ter o Jaeger rodando (veja **Docker Compose** abaixo).

## Op��o 2: Executando com Docker Compose (Jaeger)

1. Certifique-se de ter o Docker Desktop rodando com integra��o WSL2 ativa (se estiver no Windows).
2. Use o arquivo `docker-compose.yml` na raiz do projeto, contendo Jaeger:

```yaml
version: "3"
services:
  jaeger:
    image: jaegertracing/all-in-one:1.54
    ports:
      - "16686:16686"   # UI Jaeger
      - "4317:4317"     # OTLP gRPC
      - "4318:4318"     # OTLP HTTP
```

3. Abra um terminal no VS Code ou no WSL:

```bash
docker-compose up --build
```

4. Ap�s o start, a UI do Jaeger estar� dispon�vel em:

```
http://localhost:16686
```

5. Execute a API normalmente via Visual Studio ou `dotnet run`:

```bash
cd src/Inventory.Management.API
dotnet run
```

6. Os traces enviados via OpenTelemetry ser�o coletados e exibidos no Jaeger.

## Configura��es Importantes

- **Endpoint OTLP Jaeger:** `http://localhost:4317` Este endpoint est� configurado na aplica��o no `Program.cs` para enviar traces.
- **Banco de dados:** a aplica��o utiliza **SQLite in-memory**, portanto cada execu��o inicia com o banco vazio. Use o endpoint de reposi��o (`/replenish`) para popular dados de teste.
- **Swagger & API Versioning:**
  - Swagger: `/swagger`
  - Suporte a versionamento: `v1`

## Executando via CLI sem Visual Studio

1. Abra terminal no diret�rio do projeto:

```bash
cd src/Inventory.Management.API
```

2. Execute a aplica��o:

```bash
dotnet run
```

3. Para compilar e gerar build:

```bash
dotnet build
```

## Links �teis

- [Visual Studio 2022](https://visualstudio.microsoft.com/pt-br/vs/community/)
- [.NET 9](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0)
- [OpenTelemetry .NET](https://opentelemetry.io/docs/languages/net/)
- [Jaeger Tracing](https://www.jaegertracing.io/)
- [Docker Desktop WSL2 Integration](https://docs.docker.com/desktop/wsl/)
- [Swagger Documentation](https://swagger.io/docs/)