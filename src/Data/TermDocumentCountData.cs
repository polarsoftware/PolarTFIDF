using LiteDB;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// TODO: describe it
    /// </summary>
    public class TermDocumentCountData
    {
        //TODO: separate id generate from LiteDB implementation - 2020-12-22T08:45:44
        public string Id { get; set; } = ObjectId.NewObjectId().ToString();        

        /// <summary>
        /// Name of term 
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Document count in which this term appears.
        /// </summary>
        public long Count { get; set; }
    }
}