﻿using LiteDB;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Polar.ML.TfIdf
{
    public class InsertingDocumentTests
    {
        [Fact]
        public void AddGetDocumentTest()
        {

            TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();
            string docName = "TestDoc";

            var terms = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "apple", Count = 2 },
                new TermData(){ Term = "strawberry", Count = 3 },
                new TermData(){ Term = "cherry", Count = 4 },
                new TermData(){ Term = "bluberry", Count = 5 }
            };

            tfIdfEstimator.AddDocument(docName, terms);

            using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            //int count = coll.Count();            
            var docterms = coll.FindOne(x => x.Document == docName);
                        
            for (int i=0;i<terms.Count; i++)
            {
                Assert.True(docterms.Terms[i].Term == terms[i].Term);
            }
            coll.DeleteAll();
            coll2.DeleteAll();
        }

        [Fact]
        public void TfIdfTest()
        {
            TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();

            string docName1 = "TestDoc1";
            var terms1 = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "apple", Count = 2 },
            };

            string docName2 = "TestDoc2";
            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "bluberry", Count = 5 }
            };

            string docName3 = "TestDoc3";
            var terms3 = new List<TermData>()
            {
                new TermData(){ Term = "strawberry", Count = 3 },
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);
            tfIdfEstimator.AddDocument(docName3, terms3);

            var t1 = tfIdfEstimator.GetAllTermsInDocument(docName1);
            var t2 = tfIdfEstimator.GetAllTermsInDocument(docName2);
            var t3 = tfIdfEstimator.GetAllTermsInDocument(docName3);

            double banana1 = (1d / 3d) * Math.Log10(3d / 2d);
            double apple1 = (2d / 3d) * Math.Log10(3d / 1d);

            double banana2 = (1d / 6d) * Math.Log10(3d / 2d);
            double blueberry2 = (5d / 6d) * Math.Log10(3d / 1d);

            double strawberry3 = (1d / 1d) * Math.Log10(3d / 1d);

            Assert.True(t1[0].TermScore == banana1);
            Assert.True(t1[1].TermScore == apple1);
            Assert.True(t2[0].TermScore == banana2);
            Assert.True(t2[1].TermScore == blueberry2);
            Assert.True(t3[0].TermScore == strawberry3);

            using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);
            coll.DeleteAll();
            coll2.DeleteAll();
        }

        [Fact]
        public void DeleteDocumentTest()
        {
            TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();
            using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);

            coll.DeleteAll();
            coll2.DeleteAll();

            string docName1 = "TestDoc1";
            var terms1 = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "apple", Count = 2 },
            };

            string docName2 = "TestDoc2";
            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "blueberry", Count = 5 }
            };

            string docName3 = "TestDoc3";
            var terms3 = new List<TermData>()
            {
                new TermData(){ Term = "strawberry", Count = 3 },
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);
            tfIdfEstimator.AddDocument(docName3, terms3);

            tfIdfEstimator.DeleteDocument(docName2);

            var docs = coll.FindAll().ToList();
            var terms = coll2.FindAll().ToList();

            Assert.True(docs.Find(x => x.Document == docName1) != null);
            Assert.True(docs.Find(x => x.Document == docName2) == null);
            Assert.True(docs.Find(x => x.Document == docName3) != null);
            Assert.True(terms.Find(x => x.Term == "banana").Count == 1);
            Assert.True(terms.Find(x => x.Term == "blueberry").Count == 0);
            Assert.True(terms.Find(x => x.Term == "strawberry").Count == 1);
            Assert.True(terms.Find(x => x.Term == "apple").Count == 1);

            coll.DeleteAll();
            coll2.DeleteAll();
        }

        [Fact]
        public void SearchTest()
        {
            TfIdfEstimator tfIdfEstimator = new TfIdfEstimator();
            using var db = new LiteDatabase(tfIdfEstimator.TfIdfStorage.ConnectionString);
            var coll = db.GetCollection<DocumentTermsData>(tfIdfEstimator.TfIdfStorage.DocumentTermsColl);
            var coll2 = db.GetCollection<TermDocumentCountData>(tfIdfEstimator.TfIdfStorage.TermDocumentCountColl);

            coll.DeleteAll();
            coll2.DeleteAll();

            string docName1 = "TestDoc1";
            var terms1 = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "apple", Count = 2 },
            };

            string docName2 = "TestDoc2";
            var terms2 = new List<TermData>()
            {
                new TermData(){ Term = "banana", Count = 1 },
                new TermData(){ Term = "blueberry", Count = 5 }
            };

            string docName3 = "TestDoc3";
            var terms3 = new List<TermData>()
            {
                new TermData(){ Term = "strawberry", Count = 3 },
            };

            tfIdfEstimator.AddDocument(docName1, terms1);
            tfIdfEstimator.AddDocument(docName2, terms2);
            tfIdfEstimator.AddDocument(docName3, terms3);

            var docs = tfIdfEstimator.Search("banana");
            Assert.True(docs.Count == 2);
            Assert.True(docs[0]==docName1);
            Assert.True(docs[1]==docName2);

        }

    }

}
