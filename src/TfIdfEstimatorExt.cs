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

        //2020-12-28T08:40:26 public List<TermData> Terms;//
        //2020-12-28T08:40:26  public long SumTermsCount;//

        public TfIdfEstimatorExt(string storageName)         
        {
            Storage = new LiteDBTfIdfStorageExt(storageName);
        }

        /// <summary>
        /// Adds a new document along with its terms to the database.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="documentTerms"></param>
        public void AddDocument(string documentId, List<TermData> documentTerms)
        {
            DocumentTermsData documentTermsData = new DocumentTermsData()
            {
                Document = documentId,
                Terms = documentTerms
            };

            Storage.PostDocumentTerms(documentTermsData);
            Storage.PutTermDocumentCounts(documentTerms);
        }

        public DocumentTermsData GetDocument(string documentId)
        {
            return Storage.GetDocumentTerm(documentId);
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
        /// <param name="documentId"></param>
        /// <returns>List of TermScoreData</returns>
        public List<TermScoreData> GetAllTermsInDocument(string documentId)
        {
            List<TermScoreData> tsds = new List<TermScoreData>();
            foreach (var term in Storage.DocumentTermsColl.FindOne(x => x.Document == documentId).Terms)
            {
                tsds.Add(GetOneTermInDocument(documentId, term.Term));
            }
            return tsds;// {term, tfidf score}
        }

        /// <summary>
        /// Takes a names of the document from which to get a specific term with its tf-idf value.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="term"></param>
        /// <returns>Returns a TermsScoreData object</returns>
        public TermScoreData GetOneTermInDocument(string documentId, string term)
        {
            DocumentTermsData documentTermsData = Storage.DocumentTermsColl.FindOne(x => x.Document == documentId);
            long countOfTerms = documentTermsData.Terms.Sum(x => x.Count);
            var term2 = documentTermsData.Terms.Find(x => x.Term == term);
            long countOfTerm = (term2 == null ? 0 : term2.Count);
            double termFrequency = (countOfTerms == 0 ? 0 : countOfTerm / (double)countOfTerms);

            int countOfDocs = Storage.DocumentTermsColl.Count();
            long countOfDocsWithTerm = Storage.TermDocumentCountColl.FindOne(x => x.Term == term).Count;            
            double inverseDocumentFrequencySmooth = Math.Log10(countOfDocs / (double)(countOfDocsWithTerm + 1d)) + 1d;

            double tfidfValue = termFrequency * inverseDocumentFrequencySmooth;
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
        /// <param name="term"></param>
        /// <param name="numberOfDocs"></param>
        /// <returns>List of documentId strings</returns>
        public List<string> SearchNEW(string term, int numberOfDocs)
        {            
            IEnumerable<DocumentTermsData> documentTermsDatas = Storage.DocumentTermsColl.Find(d => d.Terms.Select(z => z.Term).Any(x => (term == x))); //2020-12-28T10:21:27
            var docsWithTerm = new Dictionary<string, double>();
            
            //2020-12-28T10:37:33 List<DocumentTermsData> documentTermsDatas = Storage.DocumentTermsColl.FindAll().ToList();
            foreach (var doc in documentTermsDatas)
            {
                bool has = false;
                foreach (var term1 in doc.Terms)
                {
                    if (term1.Term == term)
                    {
                        var tsd = GetOneTermInDocument(doc.Document, term);
                        docsWithTerm.Add(doc.Document, tsd.TermScore);
                        has = true;
                    }
                    if (has)
                    {
                        break;
                    }
                }
            }

            var sortedList = new List<string>();
            sortedList.AddRange(docsWithTerm.Keys);
            sortedList.OrderByDescending(x => docsWithTerm[x]);
            return sortedList.GetRange(0, Math.Min(numberOfDocs, sortedList.Count));
        }


        /// <summary>
        /// Takes a keyword and returns a set amount of documents in which that keyword is the most valuable.
        /// </summary>
        /// <param name="term"></param>
        /// <param name="numberOfDocs"></param>
        /// <returns>List of documentId strings</returns>
        public List<string> Search(string term, int numberOfDocs)
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
                foreach (var termData in doc.Terms)
                {
                    if (termData.Term == term)
                    {
                        var tsd = GetOneTermInDocument(doc.Document, term);
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
        /// <param name="documentId1"></param>
        /// <param name="documentId2"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>Cosine similarity</returns>
        public double GetDocumentSimilarity(string documentId1, string documentId2)
        {
            return DocumentSimilarity.GetDocumentSimilarityExt(documentId1, documentId2, this);
        }

        /// <summary>
        /// Returns top N most similar documents to the first document along with its cosine similarities.
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="numberOfDocuments"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>List of most similar documents</returns>
        public List<DocumentSimilarityScoreData> GetSimilarDocuments(string documentId, int numberOfDocuments)
        {
            return DocumentSimilarity.GetSimilarDocuments(documentId, numberOfDocuments, this);
        }

    }
}