namespace Polar.ML.TfIdf
{
    /// <summary>
    /// {Term, Count}
    /// </summary>
    public class TermData
    {        
        //2020-12-28T09:07:17 is this needed anywhere -> public string Id { get; set; } = Guid.NewGuid().ToString("N");

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