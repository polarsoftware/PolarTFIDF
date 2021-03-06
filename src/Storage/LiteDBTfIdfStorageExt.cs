﻿using LiteDB;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:14:34
    /// </summary>
    public class LiteDBTfIdfStorageExt
    {
        private object _lockerDocumentTermsColl = new Object();
        private object _lockerTermDocumentCountColl = new Object();
        private object _lockerTermDocumentColl = new Object();
                   
        /// <summary>
        /// Path for root dir for all databasess 
        /// </summary>
        public string PathDirRootDataBases { get; set; }

        //Database default name        
        public const string TfIdfDb = nameof(TfIdfDb);

        public string _databaseName;

        //Collectiona names
        public const string DocumentTerms = nameof(DocumentTerms);// {Document, List<TermData>}
        public const string TermDocumentCount = nameof(TermDocumentCount);//{term, count}
        public const string TermDocument = nameof(TermDocument);//{term, document}
        
        public string DatabaseName { get; set; }

        public ConnectionString ConnectionString { get; set; }
        
        public LiteDatabase DB { get; set; }
                
        public ILiteCollection<DocumentTermsData> DocumentTermsColl { get; set; } // {Document, List<TermData>}
        public ILiteCollection<TermDocumentCountData> TermDocumentCountColl { get; set; }//{term, DocumentCount}
        public ILiteCollection<TermDocumentData> TermDocumentColl { get; set; }//{term, document}

        public void PostTermDocument(string termName, string documentName)
        {
        }

        public LiteDBTfIdfStorageExt(string databaseName = null)
        {
            if(string.IsNullOrWhiteSpace(databaseName) == true)
            {
                _databaseName = TfIdfDb;
            }
            else
            {
                _databaseName = databaseName;
            }
            
            Init();
        }
                
        private void Init()
        {
            string dir = DirHandler.CreateDirInRootApp("data", "dbs");
            PathDirRootDataBases = Path.Combine(dir, _databaseName);            

            ConnectionString = new ConnectionString()
            {
                Filename = PathDirRootDataBases,
                Connection = ConnectionType.Shared,//TODO: is it best solution? Probati sa ConnectionType.Direct
                //ReadOnly = true
            };

            using var db = new LiteDatabase(ConnectionString);
            DocumentTermsColl = db.GetCollection<DocumentTermsData>(DocumentTerms);
            DocumentTermsColl.EnsureIndex(nameof(DocumentTermsData.Document), true);//+ SP: staviti da je dokument id unique - ispitati ima li problema negdje radi toga
            //Example using multi key index https://github.com/mbdavid/LiteDB/blob/master/LiteDB.Tests/Database/MultiKey_Mapper_Tests.cs - 2020-12-28T10:14:38
            DocumentTermsColl.EnsureIndex(d => d.Terms.Select(z => z.Term));

            TermDocumentCountColl = db.GetCollection<TermDocumentCountData>(TermDocumentCount);
            TermDocumentCountColl.EnsureIndex(nameof(TermDocumentCountData.Term));

            TermDocumentColl = db.GetCollection<TermDocumentData>(TermDocument);
            TermDocumentColl.EnsureIndex(nameof(TermDocumentData.Term));
        }

        // Region for all method of collection DocumentTermsColl
        #region DocumentTermsColl                 

        /// <summary>
        /// Get one DocumentTermsData filtered by document id.
        /// 2020-11-08T18:07:18
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public DocumentTermsData GetDocumentTerm(string document)
        {
            lock (_lockerDocumentTermsColl)
            {
                return DocumentTermsColl.FindOne(x => x.Document == document);
            }
        }

        /// <summary>
        /// Reduce number for each count in TermDocumentCountColl {term, count}
        /// Delete  {Document, List<TermData>} from  DocumentTermsColl
        /// ?Delete { term, documentId} from TermDocumentColl
        /// 2020-12-23T10:12:38
        /// </summary>
        /// <param name="documentId"></param>
        /// <returns></returns>
        public bool DeleteDocument(string documentId)
        {
            
            lock (_lockerDocumentTermsColl)
            {
                DocumentTermsData documentTermsData = DocumentTermsColl.FindOne(d => d.Document == documentId);
                if (documentTermsData != null)
                {
                    //Reduce number for each count in TermDocumentCountColl {term, count}
                    lock (_lockerTermDocumentCountColl)
                    {
                        foreach (TermData termData in documentTermsData.Terms)
                        {
                            TermDocumentCountData termDocumentCountData = TermDocumentCountColl.FindOne(d => d.Term == termData.Term);
                            if (termDocumentCountData != null)
                            {
                                termDocumentCountData.Count--;
                                TermDocumentCountColl.Update(termDocumentCountData);
                            }
                        }
                        //Delete  {Document, List<TermData>} from  DocumentTermsColl
                        return DocumentTermsColl.Delete(documentTermsData.Id);                                               
                    }                    
                }
                return false;
            }
        }

        public int CountDocumentTerms(string document)
        {
            lock (_lockerDocumentTermsColl)
            {
                return DocumentTermsColl.Count();
            }
        }

        /// <summary>
        /// Insert new documetn in storage.
        /// </summary>
        /// <param name="documentTermsData"></param>
        /// <returns>Return new Id of new document </returns>
        public string PostDocumentTerms(DocumentTermsData documentTermsData)
        {
            lock (_lockerDocumentTermsColl)
            {
                return DocumentTermsColl.Insert(documentTermsData);                               
            }            
        }

        #endregion DocumentTermsColl 

        #region TermDocumentCountColl         

        public TermDocumentCountData GetTermDocumentCount(string term)
        {
            lock (_lockerTermDocumentCountColl)
            {
                return TermDocumentCountColl.FindOne(x => x.Term == term);
            }
        }

        /// <summary>
        /// Add terms in collection of all terms TermDocumentCountColl.
        /// Try find term, if not exist, add with count 1 or add counter to existing term.
        /// </summary>
        /// <param name="terms"></param>
        /// <returns></returns>
        public int PutTermDocumentCounts(List<TermData> terms)
        {
            //TODO: document it why we lock if we user (Connection = ConnectionType.Shared) or we have plan not use share or LiteDB have bug about it? - 2020-12-22T09:18:47
            lock (_lockerTermDocumentCountColl)            
            {
                List<TermDocumentCountData> termDocumentCounts = new List<TermDocumentCountData>();
                foreach (TermData term in terms)
                {
                    TermDocumentCountData termDocumentCountData = GetTermDocumentCount(term.Term);
                    if (termDocumentCountData == null)
                    {
                        termDocumentCountData = new TermDocumentCountData
                        {
                            Term = term.Term,
                            Count = 1
                        };
                        termDocumentCounts.Add(termDocumentCountData);
                        //coll.Insert(tdcd);
                    }
                    else
                    {
                        termDocumentCountData.Count++;
                        termDocumentCounts.Add(termDocumentCountData);
                        //coll.Update(tdcd);
                    }
                }

                return PutTermDocumentCounts(termDocumentCounts);
            }
        }

        /// <summary>
        /// Insert or Update all documents
        /// </summary>
        /// <param name="termDocumentCounts"></param>
        /// <returns>Number of inserted new document </returns>
        public int PutTermDocumentCounts(List<TermDocumentCountData> termDocumentCounts)
        {
            lock (_lockerTermDocumentCountColl)
            {
                return TermDocumentCountColl.Upsert(termDocumentCounts);
            }
        }

        #endregion TermDocumentCountColl         

        #region TermDocumentColl
        #endregion TermDocumentColl
    }
}