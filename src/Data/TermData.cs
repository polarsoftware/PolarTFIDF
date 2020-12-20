using LiteDB;

namespace Polar.ML.TfIdf
{
    public class TermData
    {
        public string Id { get; set; } = ObjectId.NewObjectId().ToString();
        
        /// <summary>
        /// Name of term 
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Count of term appier in document 
        /// </summary>
        public long Count { get; set; }
    }
}