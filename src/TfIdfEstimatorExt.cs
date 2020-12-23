using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:14:34
    /// </summary>
    public class TfIdfEstimatorExt
    {
        public LiteDBTfIdfStorageExt Storage { get; set; }
                

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
        public void AddDocument(string documentId, List<TermData> terms)
        {
            DocumentTermsData documentTermsData = new DocumentTermsData()
            {
                Document = documentId,
                Terms = terms
            };

            Storage.PostDocumentTerms(documentTermsData);
            Storage.PutTermDocumentCounts(terms);
        }

        public DocumentTermsData GetDocument(string document)
        {
            return Storage.GetDocumentTerm(document);
        }

        /// <summary>
        /// Remove document from storage and reduce term count on other storage.
        /// </summary>
        /// <param name="document"></param>
        public void DeleteDocument(string documentId)
        {
            Storage.DeleteDocument(documentId);            
        }

        /// <summary>
        /// Takes a id of the document from which to get all terms with their tf-idf values.
        /// </summary>
        /// <param name="document"></param>
        /// <returns>List of TermScoreData</returns>
        public List<TermScoreData> GetAllTermsInDocument(string document)
        {
            List<TermScoreData> tsds = new List<TermScoreData>();
            foreach (var term in Storage.DocumentTermsColl.FindOne(x => x.Document == document).Terms)
            {
                tsds.Add(GetOneTermInDocument(document, term.Term));
            }
            return tsds;// {term, tfidf score}
        }

        /// <summary>
        /// Takes a names of the document from which to get a specific term with its tf-idf value.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="term"></param>
        /// <returns>Returns a TermsScoreData object</returns>
        public TermScoreData GetOneTermInDocument(string document, string term)
        {                        
            var doc = Storage.DocumentTermsColl.FindOne(x => x.Document == document);
            long countOfTerms = doc.Terms.Sum(x => x.Count);
            var term2 = doc.Terms.Find(x => x.Term == term);
            long countOfTerm = (term2 == null ? 0 : term2.Count);
            double termFrequency = (countOfTerms == 0 ? 0 : countOfTerm / (double)countOfTerms);

            int countOfDocs = Storage.TermDocumentCountColl.Count();
            long countOfDocsWithTerm = Storage.TermDocumentCountColl.FindOne(x => x.Term == term).Count;
            double inverseDocumentFrequency = Math.Log10(countOfDocs / (double)countOfDocsWithTerm);

            double tfidfValue = termFrequency * inverseDocumentFrequency;
            var tsd = new TermScoreData
            {
                Term = term,
                TermScore = tfidfValue
            };
            return tsd;
        }

        /// <summary>
        /// Takes a keyword and returns a set amount of documents in which that keyword is the most valuable.
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="numberOfDocs"></param>
        /// <returns>List of documentId strings</returns>
        public List<string> Search(string keyword, int numberOfDocs)
        {
            //using var db = new LiteDatabase(TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(TfIdfStorage.DocumentTermsColl);
            var docNames = new List<string>();
            List<DocumentTermsData> documentTermsDatas = Storage.DocumentTermsColl.FindAll().ToList();
            var docsWithTerm = new Dictionary<string, double>();
            var sortedList = new List<string>();
            foreach (var doc in documentTermsDatas)
            {
                bool has = false;
                foreach (var term in doc.Terms)
                {
                    if (term.Term == keyword)
                    {
                        var tsd = GetOneTermInDocument(doc.Document, keyword);
                        docsWithTerm.Add(doc.Document, tsd.TermScore);
                        has = true;
                    }
                    if (has)
                    {
                        break;
                    }
                }
            }

            sortedList.AddRange(docsWithTerm.Keys);
            sortedList.OrderByDescending(x => docsWithTerm[x]);
            return sortedList.GetRange(0, Math.Min(numberOfDocs, sortedList.Count));
        }

        /// <summary>
        /// Gets the cosine similarity of vectors of keywords of two documents.
        /// </summary>
        /// <param name="docName1"></param>
        /// <param name="docName2"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>Cosine similarity</returns>
        //public double GetDocumentSimilarity(string docName1, string docName2)
        //{
        //    return DocumentSimilarity.GetDocumentSimilarity(docName1, docName2, this);
        //}

    }
}