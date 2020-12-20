using Xunit;

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
