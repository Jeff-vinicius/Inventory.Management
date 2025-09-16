# Interação com GenAI no Desenvolvimento

### Objetivo
Este documento descreve como GenAI foi utilizado durante o desenvolvimento do protótipo **Inventory Management API**.  
O objetivo é demonstrar o apoio da IA na **tomada de decisões técnicas**, **implementação de código** e **documentação**.

## GenAI Utilizada
- **ChatGPT-4**: Principal ferramenta para brainstorming, geração de prompts.
- **GitHub Copilot (Claude Sonnet 3.5)**: Auxílio direto na escrita de código dentro do Visual Studio 2022.

## Prompts Utilizados

```
// Defina os principais conceitos de DDD (Domain-Driven Design) para um sistema de gerenciamento de inventário,
// Defina as operações principais (endpoints) para um sistema de gerenciamento de inventário,
```

```
// Crie os agregados, entidades e objetos de valor para o domínio de um sistema de Inventário,
// utilizando os conceitos de DDD com domínio rico. Linguagem: C# .NET 9.
// Estrutura do namespace: Inventory.Management.Domain
//
// Requisitos principais:
//
// 1. Agregado Raiz: InventoryItem
//    - Responsável por controlar o estoque de um SKU em uma loja.
//    - Propriedades: StoreId, Sku, AvailableQuantity, ReservedQuantity
//    - Métodos de negócio (comportamento rico, validações internas):
//       - Reserve(quantity, orderId): tenta reservar unidades. Retorna Reservation ou lança exceção de domínio.
//       - Commit(reservationId, quantity): confirma a baixa definitiva do estoque.
//       - Release(reservationId): libera reserva cancelada.
//       - Replenish(quantity, batchId): adiciona unidades ao estoque.
//    - Deve garantir consistência: não permitir reserva acima do disponível.
//
// 2. Entidade: Reservation
//    - Propriedades: ReservationId (Guid), OrderId, Quantity, Status (Pending, Committed, Released), CreatedAt.
//    - Relacionada ao agregado InventoryItem.
//    - Deve validar transições de status (ex: não pode "commit" se já estiver Released).
//
// 3. Objeto de Valor: Sku
//    - Representa o código do produto.
//    - Deve garantir que não seja vazio e siga um formato válido
//
// 4. Objeto de Valor: StoreId
//    - Identificador da loja (Guid ou string).
//    - Validação: não pode ser vazio.
//
// 5. Objetos de Valor adicionais (se fizer sentido):
//    - Quantity (valor inteiro positivo, encapsula validações de negócio).
//    - BatchId (identificador da reposição).
//
// Boas práticas:
// - Use records para Value Objects (imutabilidade).
// - Use classes para entidades e agregados.
// - Encapsule as invariantes de negócio nos métodos (não expor setters públicos).
// - O domínio deve ser totalmente independente de infraestrutura.
// - Adicione métodos privados auxiliares se necessário para manter o domínio rico.
//
// Exemplo de estilo esperado:
// public class InventoryItem {
//     public void Reserve(Quantity quantity, OrderId orderId) {
//         // validações + regras de negócio
//     }
// }
```

```
// Criação dos endpoints RESTful para o sistema de gerenciamento de inventário de acordo as operações principais
// definidas anteriormente. Utilize ASP.NET Core (.NET 9) com controller.
// Estrutura do namespace: Inventory.Management.API.Controllers
```

```
// Implementar estrutura base command, command handlers e command validators de forma genérica para ser utilizada
```

```
// Criar estrutura de Command, CommandHandlers e CommandValidator com base no request e response de cada endpoint definido no #'InventoryController.cs'
// seguindo o exemplo da estrutura feita na camada de #'Inventory.Management.Application.csproj' na pasta Inventory,
// também implemente toda a lógica dentro do CommandHandler baseada na definição feita anteriormente na camada de #'Inventory.Management.Domain.csproj' 
```

```
// Implementar a persistência utilizando Entity Framework Core com SQLite In-Memory para o protótipo.
// Criar DbContext, entidades de mapeamento e repositórios necessários.
// Estrutura do namespace: Inventory.Management.Infra.Data
```

```
// Implementar Unit of Work para controle de transações na camada de persistência.
// Estrutura do namespace: Inventory.Management.Infra.Data
```

```
// Implementar teste unitários para os CommandHandlers e agregados do domínio.
// Estrutura do namespace: Inventory.Management.UnitTests
```

```
// Implementar OpenTelemetry para rastreamento distribuído (tracing) na API.
// Configurar Jaeger como backend de tracing.
// Configurar docker-compose para rodar Jaeger localmente.
```
