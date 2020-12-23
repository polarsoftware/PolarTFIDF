using LiteDB;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// Use for IDF(Inverse document frequency), to count the number of documents containing this term.
    /// {Term, DocumentCount}
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
        /// TODO: change name to DocumentCount
        /// </summary>
        public long Count { get; set; }
    }
}