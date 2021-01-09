using System;
using Xunit;
using System.Globalization;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
//using Version = Lucene.Net.Util.Version;
using Lucene.Net.Util;
using System.IO;
using Lucene.Net.Search.Similarities;
using Lucene.Net.Analysis;
using Lucene.Net.QueryParsers.Classic;

namespace Polar.ML.TfIdf.Test
{
    public class LuceneExamplesTests
    {
        /// <summary>
        /// Create database, oen document with terms, store in database, check is storage correctly accept it.
        /// </summary>
        [Fact]
        public void LuceneMinimalExampleTest()
        {
            //var dir = new RAMDirectory();
            //var analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            //X) Create an index and define a text analyzer ------------------------
            // Ensures index backward compatibility
            const LuceneVersion AppLuceneVersion = LuceneVersion.LUCENE_48;

            // Construct a machine-independent path for the index
            var basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var indexPath = Path.Combine(basePath, "index");

            using var dir = FSDirectory.Open(indexPath);

            // Create an analyzer to process the text
            var analyzer = new StandardAnalyzer(AppLuceneVersion);

            // Create an index writer
            var indexConfig = new IndexWriterConfig(AppLuceneVersion, analyzer);
            using var writer = new IndexWriter(dir, indexConfig);

            //X) Add to the index ------------------------
            var source = new
            {
                Name = "Kermit the Frog",
                FavoritePhrase = "The quick brown fox jumps over the lazy dog"
            };

            var doc = new Document
            {
                // StringField indexes but doesn't tokenize
                new StringField("name", source.Name, Field.Store.YES),
                new TextField("favoritePhrase", source.FavoritePhrase, Field.Store.YES)
            };

            writer.AddDocument(doc);
            writer.Flush(triggerMerge: false, applyAllDeletes: false);

            //X) Construct a query ------------------------

            // Search with a phrase
            var phrase = new MultiPhraseQuery
            {
                new Term("favoritePhrase", "brown"),
                new Term("favoritePhrase", "fox")
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

        [Fact]
        public void LuceneDocSimilarityTest() {


            LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

            Analyzer analyzer = new StandardAnalyzer(luceneVersion);
            Lucene.Net.Store.Directory directory = new RAMDirectory();
            IndexWriterConfig config = new IndexWriterConfig(luceneVersion, analyzer);
            MySimilarity similarity = new MySimilarity();
            config.Similarity = similarity;
            IndexWriter indexWriter = new IndexWriter(directory, config);
            Document doc = new Document();
            TextField textField = new TextField("content", "", Field.Store.YES);
            String[] contents = {"Humpty Dumpty sat on a wall,", "Humpty Dumpty had a great fall.", "All the king's horses and all the king's men", "Couldn't put Humpty together again."};
            foreach (String content in contents)
            {
                textField.SetStringValue(content);
                doc.RemoveField("content");
                doc.Add(textField);
                indexWriter.AddDocument(doc);
            }
            indexWriter.Commit();
            IndexReader indexReader = DirectoryReader.Open(directory);
            IndexSearcher indexSearcher = new IndexSearcher(indexReader);
            indexSearcher.Similarity = similarity;
            QueryParser queryParser = new QueryParser(luceneVersion,"content", analyzer);
            Query query = queryParser.Parse("humpty dumpty");
            TopDocs topDocs = indexSearcher.Search(query, 100);
            foreach (ScoreDoc scoreDoc in topDocs.ScoreDocs)
            {
                doc = indexReader.Document(scoreDoc.Doc);
                Console.WriteLine(scoreDoc.Score + ": " +
                doc.GetField("content").GetStringValue());
            }

        }



    }



    public class MySimilarity : DefaultSimilarity
    {
      
        override public float Coord(int overlap, int maxOverlap)
        {
                return base.Coord(overlap, maxOverlap);
        }

        override public float Idf(long docFreq, long numDocs)
        {
            return base.Idf(docFreq, numDocs);
        }

        override public float LengthNorm(FieldInvertState state)
        {
            return base.LengthNorm(state);
        }

        override public float QueryNorm(float sumOfSquaredWeights)
        {
            return base.QueryNorm(sumOfSquaredWeights);
        }

        override public float ScorePayload(int doc, int start, int end, BytesRef payload)
        {
            return base.ScorePayload(doc, start, end, payload);
        }

        override public float SloppyFreq(int distance)
        {
            return base.SloppyFreq(distance);
        }

        override public float Tf(float freq)
        {
            return base.Tf(freq);
        }
    }
}
