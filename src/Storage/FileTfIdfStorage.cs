using System;
using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// Simple File (JSON) storage. Good for prototyping. 
    /// 2020-10-06T12:41:25
    /// </summary>
    public class FileTfIdfStorage : ITfIdfStorage
    {
        /// <summary>
        /// Path for root dir for all databasess 
        /// </summary>
        public string PathDirRootDataBases { get; set; }

        /// <summary>
        /// Name of database which contain data {TermName, DocumentName} 
        /// </summary>        
        public string TermDocument { get; set; } = nameof(TermDocument);

        /// <summary>
        /// Name of database which contain data { DocumentName, List<TermData>} 
        /// </summary>
        public string DocumentTerms { get; set; } = nameof(DocumentTerms);

        public void PostTermDocument(string termName, string documentName)
        {
        }

        public void PostDocumentTerms(string document, List<TermData> terms)
        {
        }

        public bool GetTermDocumentFrequency(string termName, out long termDocumentCount, out long totalDocumentCunt)
        {
            totalDocumentCunt = 0;
            termDocumentCount = 0;
            return false;
        }
        
    }
}