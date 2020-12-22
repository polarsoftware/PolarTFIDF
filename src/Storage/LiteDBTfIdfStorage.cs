using LiteDB;
using System.Collections.Generic;
using System.IO;

namespace Polar.ML.TfIdf
{
    /// <summary>
    /// 2020-10-06T12:14:34
    /// </summary>
    public class LiteDBTfIdfStorage //: ITfIdfStorage
    {
        /// <summary>
        /// Path for root dir for all databasess 
        /// </summary>
        public string PathDirRootDataBases { get; set; }

        /// <summary>
        /// Name of database which contain data {TermName, DocumentName} 
        /// </summary>        
        public string TermDocument { get; set; } = nameof(TermDocument);

        /// <summary>
        /// Name of database which contain data { DocumentName, List<TermData>} 
        /// </summary>
        public string DocumentTerms { get; set; } = nameof(DocumentTerms);

        string TfIdfDb =  nameof(TfIdfDb) + ".db";
        
        public string DocumentTermsColl = nameof(DocumentTermsColl);

        /// <summary>
        /// Collection in database which contain TermDocumentCountData object
        /// </summary>
        public string TermDocumentCountColl = nameof(TermDocumentCountColl);//{term, count}
        public string TermDocumentColl = nameof(TermDocumentColl);//{term, document}

        public ConnectionString ConnectionString { get; set; }
        public LiteDatabase DB { get; set; }

        //public ILiteCollection<DocumentTermsData> DocumentTermsColl { get; set; }

        //public static ILiteCollection<DomainMxRecordData> DomainsMxCollection { get; set; }

        public void PostTermDocument(string termName, string documentName)
        {
        }

        /// <summary>
        /// This constructor use for set index of DB during start of app
        /// </summary>
        public LiteDBTfIdfStorage()
        { 
            Init();
        }

        private void Init()
        {
            string dir = DirHandler.CreateDirInRootApp("data", "dbs");
            string destinationFileNamePath = Path.Combine(dir, TfIdfDb);            

            ConnectionString = new ConnectionString()
            {
                Filename = destinationFileNamePath,
                Connection = ConnectionType.Shared,
                //ReadOnly = true
            };

            using var db = new LiteDatabase(ConnectionString);
            var documentTermsColl = db.GetCollection<DocumentTermsData>(DocumentTermsColl);
            documentTermsColl.EnsureIndex(nameof(DocumentTermsData.Document));

            var termCountColl = db.GetCollection<TermDocumentCountData>(TermDocumentCountColl);
            termCountColl.EnsureIndex(nameof(TermDocumentCountData.Term));

            var termDocumentColl = db.GetCollection<TermDocumentData>(TermDocumentColl);
            termDocumentColl.EnsureIndex(nameof(TermDocumentData.Term));
        }

        public void PostDocumentTerms(string document, List<TermData> terms)
        {
            //TODO: save it in DocumentTerms  DB
            //TODO: save it in TermDocument DB
        }

        public bool GetTermDocumentFrequency(string termName, out long termDocumentCount, out long totalDocumentCunt)
        {
            totalDocumentCunt = GetTermDocumentCount(termName);
            termDocumentCount = GetTotalDocumentCunt();
            return true;
        }

        public long GetTermDocumentCount(string termName)
        {
            string databaseName = TermDocument + ".db";
            string coolectionName = TermDocument + "coll";
            string destinationFileNamePath = Path.Combine(PathDirRootDataBases, databaseName);

            using (var db = new LiteDatabase(destinationFileNamePath))
            {
                var colldomaintask = db.GetCollection<TermDocumentData>(coolectionName);
                colldomaintask.EnsureIndex(nameof(TermDocumentData.Term));
                return colldomaintask.LongCount(doc => doc.Term == termName);
            }
        }

        public void PostDocumentTerms(DocumentTermsData documentTermsData) 
        {            
            //2020-10-30T11:17:15 string coolectionName = "DocumentTerms" + "coll";
            using var db = new LiteDatabase(ConnectionString);
            var colldomaintask = db.GetCollection<DocumentTermsData>(DocumentTermsColl);//coolectionName
            var docterms = colldomaintask.Insert(documentTermsData);
        }          

        public long GetTotalDocumentCunt()
        {
            string databaseName = DocumentTerms + ".db";
            string coolectionName = DocumentTerms + "coll";
            string destinationFileNamePath = Path.Combine(PathDirRootDataBases, databaseName);
                        
            using (var db = new LiteDatabase(destinationFileNamePath))
            {
                var colldomaintask = db.GetCollection<DocumentTermsData>(coolectionName);
                colldomaintask.EnsureIndex(nameof(DocumentTermsData.Document));            
                return colldomaintask.Count();
            }
        }
    }
}