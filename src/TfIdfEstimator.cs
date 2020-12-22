using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:14:34
    /// </summary>
    public class TfIdfEstimator
    {
        public LiteDBTfIdfStorage TfIdfStorage = new LiteDBTfIdfStorage();
        
        public List<TermData> Terms;

        public long SumTermsCount;

        public TfIdfEstimator(string document, List<TermData> terms)
        {
            Terms = terms;
            SumTermsCount = Terms.Sum(d => d.Count);

            //save all data to storage 
            TfIdfStorage.PostDocumentTerms(document, terms);
        }

        public TfIdfEstimator() { }

        /// <summary>
        /// Adds a new document along with its terms to the database.
        /// </summary>
        /// <param name="document">Unique identifer of the document, like id of the document from other system or unique name.</param>
        /// <param name="terms">List of TermData object from this document.</param>
        public void AddDocument(string document, List<TermData> terms)
        {
            DocumentTermsData documentTermsData = new DocumentTermsData() 
            {
                Document = document,
                Terms = terms
            };            

            TfIdfStorage.PostDocumentTerms(documentTermsData);

            //TODO: describe it!
            //Find each term in???? and add count for this docuiment?
            using var db = new LiteDatabase(TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<TermDocumentCountData>(TfIdfStorage.TermDocumentCountColl);
            foreach (var term in terms)
            {
                var tdcd = coll.FindOne(x => x.Term == term.Term);
                if (tdcd == null)
                {
                    tdcd = new TermDocumentCountData
                    {
                        Term = term.Term,
                        Count = 1
                    };
                    coll.Insert(tdcd);
                }
                else
                {
                    tdcd.Count++;
                    coll.Update(tdcd);
                }
            }            
        }

        /// <summary>
        /// Takes a name of the document to be removed from the database along with its keywords.
        /// </summary>
        /// <param name="document"></param>
        public void DeleteDocument(string document)
        {
            using var db = new LiteDatabase(TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(TfIdfStorage.DocumentTermsColl);
            var coll2 = db.GetCollection<TermDocumentCountData>(TfIdfStorage.TermDocumentCountColl);
            var tsds = GetAllTermsInDocument(document);
            foreach (var tsd in tsds)
            {
                var dtd = coll2.FindOne(x => x.Term == tsd.Term);
                dtd.Count--;
                coll2.Update(dtd);
            }
            DocumentTermsData documentTermsData = coll.FindOne(d => d.Document == document);
            if (documentTermsData != null)
            {
                coll.Delete(documentTermsData.Id);
            }
        }

        /// <summary>
        /// Takes a names of the document from which to get all terms with their tf-idf values.
        /// </summary>
        /// <param name="document"></param>
        /// <returns>List of TermScoreData</returns>
        public List<TermScoreData> GetAllTermsInDocument(string document)
        {
            var tsds = new List<TermScoreData>();
            using var db = new LiteDatabase(TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(TfIdfStorage.DocumentTermsColl);
            foreach (var term in coll.FindOne(x => x.Document == document).Terms)
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
            using var db = new LiteDatabase(TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(TfIdfStorage.DocumentTermsColl);
            
            var doc = coll.FindOne(x => x.Document == document);
            long countOfTerms = doc.Terms.Sum(x => x.Count);
            long countOfTerm = doc.Terms.Find(x => x.Term == term).Count;
            double termFrequency = countOfTerm / (double)countOfTerms;

            var coll2 = db.GetCollection<TermDocumentCountData>(TfIdfStorage.TermDocumentCountColl);
            int countOfDocs = coll.Count();
            long countOfDocsWithTerm = coll2.FindOne(x => x.Term == term).Count;
            double inverseDocumentFrequency = Math.Log10(countOfDocs / (double)countOfDocsWithTerm);

            double tfidfValue = termFrequency * inverseDocumentFrequency;
            var tsd = new TermScoreData
            {
                Term = term,
                TermScore = tfidfValue
            };
            return tsd;
        }

        public List<string> Search(string keyword)
        {
            int numberOfDocs = 10;

            using var db = new LiteDatabase(TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(TfIdfStorage.DocumentTermsColl);
            var docNames = new List<string>();
            var docs = coll.FindAll().ToList();
            var docsWithTerm = new Dictionary<string,double>();
            var sortedList = new List<string>();
            foreach (var doc in docs)
            {
                bool has = false;
                foreach (var term in doc.Terms)
                {
                    if(term.Term == keyword)
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

            return sortedList.GetRange(0,Math.Min(numberOfDocs,sortedList.Count));
        }

        /// <summary>
        /// Gets the cosine similarity of vectors of keywords of two documents.
        /// </summary>
        /// <param name="docName1"></param>
        /// <param name="docName2"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>Cosine similarity</returns>
        public double GetDocumentSimilarity(string docName1, string docName2)
        {
            return DocumentSimilarity.GetDocumentSimilarity(docName1, docName2, this);
        }

        /// <summary>
        /// Returns top N most similar documents to the first document along with its cosine similarities.
        /// </summary>
        /// <param name="document1"></param>
        /// <param name="numberOfDocuments"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>List of most similar documents</returns>
        public List<DocumentSimilarityScoreData> GetSimilarDocuments(string document1, int numberOfDocuments)
        {
            return DocumentSimilarity.GetSimilarDocuments(document1, numberOfDocuments, this);
        }
    }    
}