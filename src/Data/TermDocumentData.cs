namespace Polar.ML.TfIdf
{
    /// <summary>
    /// Use for IDF(Inverse document frequency), to count the number of documents containing this term.
    /// </summary>
    public class TermDocumentData
    {
        /// <summary>
        /// Term name.
        /// </summary>
        public string Term { get; set; }

        /// <summary>
        /// Document name.
        /// </summary>
        public string Document { get; set; }
    }
}