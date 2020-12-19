using LiteDB;
using System;
using System.Collections.Generic;
using Xunit;
using System.Linq;

namespace Polar.ML.TfIdf
{
    public class DocumentTermsDataTests
    {
        [Fact]
        public void AddGetDocumentTest()
        {
            TfIdfEstimatorExt tfIdfEstimator = new TfIdfEstimatorExt(Guid.NewGuid().ToString("N"));
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

            DocumentTermsData documentTermsData = tfIdfEstimator.GetDocument(docName);

            for (int i = 0; i < documentTermsData.Terms.Count; i++)
            {
                Assert.True(documentTermsData.Terms[i].Term == terms[i].Term);
            }            
        }       
    }
}
