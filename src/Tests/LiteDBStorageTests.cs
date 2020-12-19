using LiteDB;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Polar.ML.TfIdf
{
    public class LiteDBStorageTests
    {
        [Fact]
        public void CreateDB()
        {
            LiteDBTfIdfStorageExt liteDBTfIdfStorageExt = new LiteDBTfIdfStorageExt("db1");

        }
    }
}
