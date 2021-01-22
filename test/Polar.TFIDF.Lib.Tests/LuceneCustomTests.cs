using System;
using Xunit;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
//using Version = Lucene.Net.Util.Version;
using Lucene.Net.Util;
using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Util;

namespace Polar.ML.TfIdf.Test
{
    public class LuceneCustomTests
    {
        /// <summary>
        /// Create database, oen document with terms, store in database, check is storage correctly accept it.
        /// </summary>
        [Fact]
        public void LuceneCustomAnalizerTokenizerTest()
        {
            //var dir = new RAMDirectory();
            //var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            //X) Create an index and define a text analyzer ------------------------
            // Ensures index backward compatibility
            const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");
            System.IO.Directory.Delete(indexPath, true);

            using var dir = FSDirectory.Open(indexPath);

            // Create an analyzer to process the text
            var analyzer = new TechKeywordAnalyzer();// StandardAnalyzer(AppLuceneVersion);

            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);

            //X) Add to the index ------------------------
            var source = new
            {
                Name = "Kermit the Frog",
                FavoritePhrase = "quick brown fox jumps over lazy dog"
            };

            var doc = new Document
            {
                // StringField indexes but doesn't tokenize
                new StringField("name", source.Name, Field.Store.YES),
                new TextField("favoritePhrase", source.FavoritePhrase, Field.Store.YES)
            };

            var source2 = new
            {
                Name = "Doc2",
                FavoritePhrase = "the query parser "
            };

            var doc2 = new Document
            {
                // StringField indexes but doesn't tokenize
                new StringField("name", source2.Name, Field.Store.YES),
                new TextField("favoritePhrase", source2.FavoritePhrase, Field.Store.YES)
            };


            writer.AddDocument(doc);
            writer.AddDocument(doc2);
            writer.Flush(triggerMerge: false, applyAllDeletes: false);

            //X) Construct a query ------------------------

            // Search with a phrase
            var phrase = new MultiPhraseQuery
            {
                new Term("favoritePhrase", "the")
                //new Term("favoritePhrase", "brown")
                //,new Term("favoritePhrase", "fox")
            };

            //X) Fetch the results ------------------------

            // Re-use the writer to get real-time updates
            using var reader = writer.GetReader(applyAllDeletes: true);
            var searcher = new IndexSearcher(reader);
            ScoreDoc[] hits = searcher.Search(phrase, 20 /* top 20 */).ScoreDocs;

            // Display the output in a table
            string strDisp = $"{"Score",10}" + $" {"Name",-15}" + $" {"Favorite Phrase",-40}";
            Console.WriteLine(strDisp);

            foreach (var hit in hits)
            {
                var foundDoc = searcher.Doc(hit.Doc);
                string str = $"{hit.Score:f8}" + $" {foundDoc.Get("name"),-15}" + $" {foundDoc.Get("favoritePhrase"),-40}";
                Console.WriteLine(str);
            }
        }
    }

    /// <summary>
    /// http://intelligiblebabble.com/custom-lucene-tokenizer-for-tech-keywords/
    /// </summary>
    public sealed class TechKeywordTokenizer : CharTokenizer
    {
        public TechKeywordTokenizer(LuceneVersion matchVersion, TextReader reader) : base(matchVersion, reader)
        {
        }

        protected override int Normalize(int c)
        {
            return char.ToLower((char)c);
        }

        protected override bool IsTokenChar(int c)        
        {
            // we are splitting tokens on non-letters, so long
            // as they are not "+" or "#", which are often used in tech
            // key words (like C++ and C#)
            return char.IsLetterOrDigit((char)c) || c == '+' || c == '#' || c == '.';
        }
    }
    public sealed class TechKeywordAnalyzer : Analyzer
    {
        public TechKeywordAnalyzer()
        { 
        }

        protected  override TokenStreamComponents CreateComponents(string fieldName, TextReader reader)
        {            
            Tokenizer tokenizer = new TechKeywordTokenizer(LuceneVersion.LUCENE_48, reader);
            //tokenizer = new StopFilter(LuceneVersion.LUCENE_48, tokenizer, StopAnalyzer.ENGLISH_STOP_WORDS_SET);
            return new TokenStreamComponents(tokenizer, tokenizer);            
        }
    }
}
