using LiteDB;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// </summary>
    public class TermDocumentCountData
    {
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