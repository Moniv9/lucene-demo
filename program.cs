using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Lucene.Net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.QueryParsers;

namespace lucene_demo
{
   class Program
   {
      static void Main(string[] args)
      {
         string[] input_data = { };

         //reading a file from desktop for data
         using (TextReader reader = File.OpenText("d:\\Programs\\input.txt"))
         {
            input_data = reader.ReadToEnd().Split('\n');
         }

         //without date
         WorldEngine engine = new WorldEngine("content", "d:\\Programs\\data_without_date");
         // engine.makeIndex(input_data, true);

         ////with date
         WorldEngine engine2 = new WorldEngine("content", "d:\\Programs\\data_with_date");
         engine2.makeIndex(input_data, false);

         //engine.search();
         //engine.readIndex();

      }
   }

   public class WorldEngine
   {
      private string field_name;
      private string path;

      public WorldEngine(string field_name, string path)
      {
         //initialize
         this.field_name = field_name;
         this.path = path;
      }

      public void makeIndex(string[] input_data, Boolean condition)
      {
         //making index file for search
         FSDirectory directory = FSDirectory.Open(path);
         IndexWriter writer = new IndexWriter(directory, new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30), true, IndexWriter.MaxFieldLength.UNLIMITED);

         if (condition)
         {
            foreach (string n in input_data)
            {
               Document document = new Document();
               document.Add(new Field(field_name, n, Field.Store.YES, Field.Index.ANALYZED));
               writer.AddDocument(document);
            }
         }

         else
         {
            DateTime date = new DateTime(1990, 1, 1);
            Random random = new Random();

            foreach (string n in input_data)
            {
               Document document = new Document();
               document.Add(new Field(field_name, n, Field.Store.YES, Field.Index.ANALYZED));
               document.Add(new Field("date", date.AddDays(random.Next(9999)).ToLongDateString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
               writer.AddDocument(document);
            }
         }

         writer.Optimize();
         writer.Dispose();
         Console.WriteLine("Done Indexing");

      }

      public void search()
      {
         Lucene.Net.Store.Directory directory = FSDirectory.Open(path);
         IndexSearcher searcher = new IndexSearcher(directory, true);
         var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

         Term t = new Term(field_name, Console.ReadLine());
         Query query = new TermQuery(t);
         TopDocs hits = searcher.Search(query, 50);

         int results = hits.ScoreDocs.Length;
         Console.WriteLine("Total results found :" + results);

         for (int i = 0; i < results; i++)
         {
            Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);
            Console.WriteLine("\n\nFound in this sentence : " + doc.Get("content") + "score : " + hits.ScoreDocs[i].Score);
         }

         searcher.Dispose();

      }

      public void readIndex()
      {
         FSDirectory directory = FSDirectory.Open(path);
         var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
         IndexReader reader = IndexReader.Open(directory, true);

         int no = reader.NumDocs();
         Console.WriteLine(no);

         for (int i = 0; i < no; i++)
         {
            Document d = reader.Document(i);
            Console.WriteLine(d.Get("content") + "\n\n");
         }

      }

   }


}
