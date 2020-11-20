using Xunit;

namespace Vertex.Crud.Test.Core
{
    [CollectionDefinition(Name)]
    public class ProviderCollection : ICollectionFixture<ProviderFixture>
    {
        public const string Name = "ProviderCollection";
    }
}
