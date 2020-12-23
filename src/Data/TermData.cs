using LiteDB;

namespace Polar.ML.TfIdf
{

    /// <summary>
    /// {Term, Count}
    /// </summary>
    public class TermData
    {
        //TODO: separate id generate from LiteDB implementation - 2020-12-22T08:45:44
        public string Id { get; set; } = ObjectId.NewObjectId().ToString();
        
        /// <summary>
        /// Name of term in document. 
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Count of term appier in one document.
        /// </summary>
        public long Count { get; set; }
    }
}