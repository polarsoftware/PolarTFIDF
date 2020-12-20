using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:14:34
    /// </summary>
    public class TfIdfEstimatorExt
    {
        private LiteDBTfIdfStorageExt _storage;
        
        public List<TermData> Terms;

        public long SumTermsCount;
                
        public TfIdfEstimatorExt(string storageName)         
        {
            _storage = new LiteDBTfIdfStorageExt(storageName);
        }

        /// <summary>
        /// Adds a new document al;ong with its terms to the database.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="terms"></param>
        public void AddDocument(string document, List<TermData> terms)
        {
            DocumentTermsData documentTermsData = new DocumentTermsData()
            {
                Document = document,
                Terms = terms
            };

            _storage.PostDocumentTerms(documentTermsData);
            _storage.PutTermDocumentCounts(terms);
        }

        public DocumentTermsData GetDocument(string document)
        {
            return _storage.GetDocumentTerm(document);
        }
       
    }    
}