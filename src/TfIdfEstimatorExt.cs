using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:14:34
    /// </summary>
    public class TfIdfEstimatorExt
    {
        private LiteDBTfIdfStorageExt Storage { get; set; }
                

        public List<TermData> Terms;//TODO: zasto ovo  postoji.. cemu ovo sluzi.. koji je plan s ovim?

        public long SumTermsCount;//TODO: zasto ovo  postoji.. cemu ovo sluzi.. koji je plan s ovim?

        public TfIdfEstimatorExt(string storageName)         
        {
            Storage = new LiteDBTfIdfStorageExt(storageName);
        }

        /// <summary>
        /// Adds a new document along with its terms to the database.
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

            Storage.PostDocumentTerms(documentTermsData);
            Storage.PutTermDocumentCounts(terms);
        }

        public DocumentTermsData GetDocument(string document)
        {
            return Storage.GetDocumentTerm(document);
        }
       
    }    
}