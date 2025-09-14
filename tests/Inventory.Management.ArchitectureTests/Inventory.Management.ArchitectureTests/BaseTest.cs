using Inventory.Management.Application.Abstractions.Messaging;
using Inventory.Management.Domain.Aggregates;
using Inventory.Management.Infra.Data.Context;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Reflection;

namespace Inventory.Management.ArchitectureTests
{
    public abstract class BaseTest
    {
        protected static readonly Assembly DomainAssembly = typeof(InventoryItem).Assembly;
        protected static readonly Assembly ApplicationAssembly = typeof(ICommand).Assembly;
        protected static readonly Assembly InfrastructureAssembly = typeof(InventoryDbContext).Assembly;
        protected static readonly Assembly PresentationAssembly = typeof(Program).Assembly;
    }
}
