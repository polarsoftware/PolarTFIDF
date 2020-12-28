using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using LiteDB;
using System.Linq;

namespace Polar.ML.TfIdf.Test
{
    public class DocumentProcessingTests
    {
        string docName1 = nameof(docName1);
        string docName2 = nameof(docName2);
        string docName3 = nameof(docName3);
        string banana = nameof(banana);
        string apple = nameof(apple);
        string blueberry = nameof(blueberry);
        string cherry = nameof(cherry);
        string strawberry = nameof(strawberry);

        [Fact]
        public void AddGetDocumentTest()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));

            var terms = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = apple, Count = 2 },
                new TermData(){ Term = strawberry, Count = 3 },
                new TermData(){ Term = cherry, Count = 4 },
                new TermData(){ Term = blueberry, Count = 5 }
            };

            tfIdfEstimator.AddDocument(docName1, terms);

            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //int count = coll.Count();            
            var docterms = tfIdfEstimator.Storage.DocumentTermsColl.FindOne(x => x.Document == docName1);
                        
            for (int i=0;i<terms.Count; i++)
            {
                Assert.True(docterms.Terms[i].Term == terms[i].Term);
            }
            //coll.DeleteAll();
            //coll2.DeleteAll();
            //Delete Database - in case of LiteDB we delete one file.
            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }

        [Fact]
        public void TfIdfTest()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
            var terms1 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = apple, Count = 2 },
            };

            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = blueberry, Count = 5 }
            };

            var terms3 = new List<TermData>()
            {
                new TermData(){ Term = strawberry, Count = 3 },
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);
            tfIdfEstimator.AddDocument(docName3, terms3);

            var t1 = tfIdfEstimator.GetAllTermsInDocument(docName1);
            var t2 = tfIdfEstimator.GetAllTermsInDocument(docName2);
            var t3 = tfIdfEstimator.GetAllTermsInDocument(docName3);
                        
            double banana1 = (1d / 3d) * (Math.Log10(3d/(2d+1d)) + 1d);
            double apple1 = (2d / 3d) * (Math.Log10(3d/(1d + 1d)) + 1d);

            double banana2 = (1d / 6d) * (Math.Log10(3d/(2d + 1d)) + 1d);
            double blueberry2 = (5d / 6d) * (Math.Log10(3d/(1d + 1d)) + 1d);

            double strawberry3 = (1d / 1d) * (Math.Log10(3d/(1d + 1d)) + 1d);

            Assert.True(t1[0].TermScore == banana1);
            Assert.True(t1[1].TermScore == apple1);
            Assert.True(t2[0].TermScore == banana2);
            Assert.True(t2[1].TermScore == blueberry2);
            Assert.True(t3[0].TermScore == strawberry3);

            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //coll.DeleteAll();
            //coll2.DeleteAll();
            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }

        [Fact]
        public void DeleteDocumentTest()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //
            //coll.DeleteAll();
            //coll2.DeleteAll();

            var terms1 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = apple, Count = 2 },
            };

            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = blueberry, Count = 5 }
            };

            var terms3 = new List<TermData>()
            {
                new TermData(){ Term = strawberry, Count = 3 },
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);
            tfIdfEstimator.AddDocument(docName3, terms3);

            tfIdfEstimator.DeleteDocument(docName2);

            var docs = tfIdfEstimator.Storage.DocumentTermsColl.FindAll().ToList();            
            var terms = tfIdfEstimator.Storage.TermDocumentCountColl.FindAll().ToList();

            Assert.True(docs.Find(x => x.Document == docName1) != null);
            Assert.True(docs.Find(x => x.Document == docName2) == null);
            Assert.True(docs.Find(x => x.Document == docName3) != null);
            Assert.True(terms.Find(x => x.Term == banana).Count == 1);
            Assert.True(terms.Find(x => x.Term == blueberry).Count == 0);
            Assert.True(terms.Find(x => x.Term == strawberry).Count == 1);
            Assert.True(terms.Find(x => x.Term == apple).Count == 1);

            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }

        [Fact]
        public void SearchTest()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //
            //coll.DeleteAll();
            //coll2.DeleteAll();

            var terms1 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = apple, Count = 2 },
            };

            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = blueberry, Count = 5 }
            };

            var terms3 = new List<TermData>()
            {
                new TermData(){ Term = strawberry, Count = 3 },
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);
            tfIdfEstimator.AddDocument(docName3, terms3);

            var docs = tfIdfEstimator.Search(banana,10);
            Assert.True(docs.Count == 2);
            Assert.True(docs[0]==docName1);
            Assert.True(docs[1]==docName2);

            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }

        /// <summary>
        /// Testing the similarity calculating algorithm for two documents.
        /// In this scenario terms1 is empty.
        /// </summary>
        [Fact]
        public void TwoDocumentsSimilarity1Test()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
            //TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();
            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //coll.DeleteAll();
            //coll2.DeleteAll();

            var terms1 = new List<TermData>();

            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = banana, Count = 1 },
                new TermData(){ Term = blueberry, Count = 5 }
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);

            double similarity = tfIdfEstimator.GetDocumentSimilarity(docName1, docName2);
            Assert.True(similarity == 0);
            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }

        /// <summary>
        /// Testing the similarity calculating algorithm for two documents.
        /// In this scenario both terms1 and terms2 are empty.
        /// </summary>
        [Fact]
        public void TwoDocumentsSimilarity2Test()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
            //TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();
            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //coll.DeleteAll();
            //coll2.DeleteAll();

            var terms1 = new List<TermData>();

            var terms2 = new List<TermData>();

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);

            double similarity = tfIdfEstimator.GetDocumentSimilarity(docName1, docName2);
            Assert.True(similarity == 0);
            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }

        /// <summary>
        /// Testing the similarity calculating algorithm for two documents.
        /// In this scenario terms1 and terms2 are the same.
        /// </summary>
        [Fact]
        public void TwoDocumentsSimilarity3Test()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
            //TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();
            //using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            //var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            //var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //coll.DeleteAll();
            //coll2.DeleteAll();

            var terms1 = new List<TermData>()
            {
                new TermData(){Term=banana,Count=2},
                new TermData(){Term=blueberry,Count=3},
                new TermData(){Term=apple,Count=7}
            };

            var terms2 = new List<TermData>()
            {
                new TermData(){Term=apple,Count=7},
                new TermData(){Term=banana,Count=2},
                new TermData(){Term=blueberry,Count=3},
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);

            double similarity = tfIdfEstimator.GetDocumentSimilarity(docName1, docName2);
            Assert.True(similarity == 1);
            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }
    }
}
