using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Search.Similarities;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;

namespace TestingConsoleProject
{


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

    class Program
    {
        static void Main(string[] args)
        {

            LuceneVersion luceneVersion = LuceneVersion.LUCENE_48;

            Analyzer analyzer = new StandardAnalyzer(luceneVersion);
            Lucene.Net.Store.Directory directory = new RAMDirectory();
            IndexWriterConfig config = new IndexWriterConfig(luceneVersion, analyzer);
            MySimilarity similarity = new MySimilarity();
            config.Similarity = similarity;
            IndexWriter indexWriter = new IndexWriter(directory, config);
            Document doc = new Document();
            TextField textField = new TextField("content", "", Field.Store.YES);
            String[] contents = { "Humpty Dumpty sat on a wall,", "Humpty Dumpty had a great fall.", "All the king's horses and all the king's men", "Couldn't put Humpty together again." };
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
            QueryParser queryParser = new QueryParser(luceneVersion, "content", analyzer);
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
}
