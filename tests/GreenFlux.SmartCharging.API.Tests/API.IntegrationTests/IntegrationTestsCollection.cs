using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GreenFlux.SmartCharging.API.Tests.API.IntegrationTests
{
    [CollectionDefinition("IntegrationTestsCollection")]
    public class IntegrationTestsCollection : ICollectionFixture<TestHostFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
