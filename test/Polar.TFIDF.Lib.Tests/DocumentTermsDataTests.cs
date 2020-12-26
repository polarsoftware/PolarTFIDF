using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using Polar.ML.TfIdf;

namespace Polar.ML.TfIdf.Test
{
    public class DocumentTermsDataTests
    {
        /// <summary>
        /// Create database, oen document with terms, store in database, check is storage correctly accept it.
        /// </summary>
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
            //TODO: check is all term exist in TermDocumentCountColl - 2020-12-22T09:32:03

            //Delete Database - in case of LiteDB we delete one file.
            File.Delete(tfIdfEstimator.Storage.PathDirRootDataBases);
        }       
    }
}
