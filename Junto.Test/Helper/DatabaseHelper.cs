using Junto.Infra.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Junto.Test.Helper
{
    public static class DatabaseHelper
    {
        public static JuntoContext GetContext()
        {
            var options = new DbContextOptionsBuilder<JuntoContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;
            var databaseContext = new JuntoContext(options);
            return databaseContext;
        }
    }
}
