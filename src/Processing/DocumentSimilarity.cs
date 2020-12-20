using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Polar.ML.TfIdf
{
    public static class DocumentSimilarity
    {
        /// <summary>
        /// Gets the cosine similarity of vectors of keywords of two documents.
        /// </summary>
        /// <param name="docName1"></param>
        /// <param name="docName2"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>Cosine similarity</returns>
        public static double GetDocumentSimilarity(string docName1, string docName2, TfIdfEstimator tfIdfEstimator)
        {
            using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);

            // Get all keywords from the two documents and make a union of them.
            var doc1 = coll.FindOne(docName1);
            var doc2 = coll.FindOne(docName2);
            var keywords1 = doc1.Terms;
            var keywords2 = doc2.Terms;
            var set = new HashSet<string>();
            foreach (var keyword in keywords1)
            {
                set.Add(keyword.Term);
            }
            foreach (var keyword in keywords2)
            {
                set.Add(keyword.Term);
            }

            // Get term scores of keywords in a union for both documents.
            var list1 = new List<double>();
            var list2 = new List<double>();
            foreach (var keyword in set)
            {
                list1.Add(tfIdfEstimator.GetOneTermInDocument(docName1, keyword).TermScore);
                list2.Add(tfIdfEstimator.GetOneTermInDocument(docName2, keyword).TermScore);
            }

            // Calculate the cosine similarity. CosSimilarity(v1, v2) = dot(v1,v2) / (norm(v1) * norm(v2)) where v1 and v2 are vectors
            var vect1 = new Vector<double>(list1.ToArray());
            var vect2 = new Vector<double>(list2.ToArray());
            double dot = Vector.Dot(vect1, vect2);
            double norm1 = 0;
            double norm2 = 0;
            foreach (var item in list1)
            {
                norm1 += Math.Pow(item, 2);
            }

            foreach (var item in list2)
            {
                norm2 += Math.Pow(item, 2);
            }
            norm1 = Math.Sqrt(norm1);
            norm2 = Math.Sqrt(norm2);
            double similarity = dot / (norm1 * norm2);

            return similarity;
        }

        /// <summary>
        /// Returns top N most similar documents to the first document along with its cosine similarities.
        /// </summary>
        /// <param name="document1"></param>
        /// <param name="numberOfDocuments"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>List of most similar documents</returns>
        public static List<DocumentSimilarityScoreData> GetSimilarDocuments(string document1, int numberOfDocuments, TfIdfEstimator tfIdfEstimator)
        {
            using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);

            // Get all docs except the one that we are looking the most similar to.
            var docs = coll.FindAll().ToList();
            docs.Remove(docs.Find(x => x.Document == document1));

            // Get similarity scores of all those documents.
            var docScores = new List<DocumentSimilarityScoreData>();
            foreach (var doc in docs)
            {
                docScores.Add(new DocumentSimilarityScoreData()
                {
                    Document = doc.Document,
                    Score = GetDocumentSimilarity(document1, doc.Document, tfIdfEstimator)
                });
            }

            //Order them by their scores descending and return top N of them.
            docScores.OrderByDescending(x => x.Score);
            return docScores.GetRange(0, numberOfDocuments);
        }






    }
}
