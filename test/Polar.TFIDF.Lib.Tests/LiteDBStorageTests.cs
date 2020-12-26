using System.IO;
using Polar.ML.TfIdf;
using Xunit;

namespace Polar.ML.TfIdf.Test
{
    public class LiteDBStorageTests
    {
        [Fact]
        public void CreateDB()
        {
            LiteDBTfIdfStorageExt liteDBTfIdfStorageExt = new LiteDBTfIdfStorageExt("db1");
            File.Delete(liteDBTfIdfStorageExt.PathDirRootDataBases);
        }
    }
}
