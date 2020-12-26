using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Polar.ML.TfIdf
{
    public static class DocumentSimilarity
    {
        public static double DotProduct(List<double> v1, List<double> v2)
        {
            if (v1.Count != v2.Count)
            {
                throw new ArgumentException();
            }
            double value = 0;
            for (int i = 0; i < v1.Count; i++)
            {
                value += v1[i] * v2[i];
            }
            return value;
        }

        /// <summary>
        /// Gets the cosine similarity of vectors of keywords of two documents.
        /// Tf-Idf and Cosine similarity: https://janav.wordpress.com/2013/10/27/tf-idf-and-cosine-similarity/
        /// </summary>
        /// <param name="docName1"></param>
        /// <param name="docName2"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>Cosine similarity</returns>
        public static double GetDocumentSimilarityExt(string docName1, string docName2, TfIdfEstimatorExt tfIdfEstimator)
        {
            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var documentTermsColl = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            
            // Get all keywords from the two documents and make a union of them.
            var doc1 = tfIdfEstimator.Storage.DocumentTermsColl.FindOne(x => x.Document == docName1);
            var doc2 = tfIdfEstimator.Storage.DocumentTermsColl.FindOne(x => x.Document == docName2);
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

            double dot = DotProduct(list1, list2);
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
            double similarity = (norm1 * norm2 == 0 ? 0 : dot / (norm1 * norm2));

            return similarity;
        }


        /// <summary>
        /// Returns top N most similar documents to the first document along with its cosine similarities.
        /// </summary>
        /// <param name="document1"></param>
        /// <param name="numberOfDocuments"></param>
        /// <param name="tfIdfEstimator"></param>
        /// <returns>List of most similar documents</returns>
        public static List<DocumentSimilarityScoreData> GetSimilarDocuments(string document1, int numberOfDocuments, TfIdfEstimatorExt tfIdfEstimator)
        {            
            // Get all docs except the one that we are looking the most similar to.
            var docs = tfIdfEstimator.Storage.DocumentTermsColl.FindAll().ToList();//TODO: optimize it!!! - 2020-12-22T10:04:31
            docs.Remove(docs.Find(x => x.Document == document1));

            // Get similarity scores of all those documents.
            var docScores = new List<DocumentSimilarityScoreData>();
            foreach (var doc in docs)
            {
                docScores.Add(new DocumentSimilarityScoreData()
                {
                    Document = doc.Document,
                    Score = GetDocumentSimilarityExt(document1, doc.Document, tfIdfEstimator)
                });
            }

            //Order them by their scores descending and return top N of them.
            docScores.OrderByDescending(x => x.Score);
            return docScores.GetRange(0, numberOfDocuments);
        }

        //OLD 2020-12-26T08:58:30 ------------ 

        /// <summary>
        /// Gets the cosine similarity of vectors of keywords of two documents.
        /// TODO: dodati link na webu koji to objasnjava 
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
            var doc1 = coll.FindOne(x => x.Document==docName1);
            var doc2 = coll.FindOne(x => x.Document == docName2);
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

            double dot = DotProduct(list1,list2);
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
            double similarity = (norm1*norm2==0?0:dot / (norm1 * norm2));

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
            var docs = coll.FindAll().ToList();//TODO: optimize it!!! - 2020-12-22T10:04:31
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
