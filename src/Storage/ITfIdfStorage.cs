using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:49:16
    /// </summary>
    public interface ITfIdfStorage
    {
        /// <summary>
        /// Path for root dir for all databasess 
        /// </summary>
        string PathDirRootDataBases { get; set; }

        /// <summary>
        /// Contain {TermName, DocumentName} 
        /// </summary>
        string DocumentTerms { get; set; }
        
        /// <summary>
        /// Contain { DocumentName, List<TermData>} 
        /// </summary>
        string TermDocument { get; set; }

        bool GetTermDocumentFrequency(string termName, out long termDocumentCount, out long totalDocumentCunt);
        void PostDocumentTerms(string document, List<TermData> terms);
        void PostTermDocument(string termName, string documentName);
    }
}