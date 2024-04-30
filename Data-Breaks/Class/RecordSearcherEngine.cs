using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers.Classic;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System.IO;
using Data_Breaks.Interface;
using System;

namespace Data_Breaks.Class
{

    public class RecordSearcherEngine : IRecordSearcherEngine
    {
        private readonly StandardAnalyzer _analyzer;
        private const string indexPath = @"indexFolder";
        private static string[] filePaths = { "0.txt", "1.txt", "2.txt", "4.txt" };
        private const int total = 39000000;
      
        public RecordSearcherEngine()
        {
            //Analyzer Configuration 
            const LuceneVersion version = LuceneVersion.LUCENE_48;
            _analyzer = new StandardAnalyzer(version);

        }

        public void CreateIndex()
        {
            int lineNumber = 0;
            FSDirectory dir = FSDirectory.Open(new DirectoryInfo(indexPath));
            IndexWriterConfig config = new IndexWriterConfig(LuceneVersion.LUCENE_48, _analyzer);
            config.OpenMode = OpenMode.CREATE_OR_APPEND;
            using (IndexWriter writer = new IndexWriter(dir, config))
            {
                Console.Clear();
                Console.WriteLine("Loading...");
                Console.Title = "C# Console Progress Bar";
                Console.CursorVisible = false;
                // Read file, line by line
                foreach (string filePath in filePaths)
                {
                    foreach (string line in File.ReadLines(filePath))
                    {
                        lineNumber++;
                        string pb = "\u2551";
                        Console.Write(pb);

                        double percent = (double)lineNumber / total * 100;
                        Console.Write($"{lineNumber} / {total} ({percent:N2}%)");
                        Console.SetCursorPosition(1, 1);
                        Console.ForegroundColor = ConsoleColor.Red;

                        //Add the line to the Document Index
                        AddToIndexDocument(writer, line);

                        
                    }
                }

                Console.WriteLine("\nLoading File Completed");


            }
        }

        private void AddToIndexDocument(IndexWriter writer, string line)
        {
            try
            {

                string[] fields = line.Split(':');

                string? name = fields[2].Trim();
                string? surname = fields[3].Trim();
                string? phone = fields[0].Trim();
                string? residenceLocation = fields[5].Trim();
                string? originLocation = fields[6].Trim();
                string? date = fields[7].Trim();

                //Create a document with the following informations
                Document doc = new Document();
                doc.Add(new TextField("Name", name, Field.Store.YES));
                doc.Add(new TextField("Surname", surname, Field.Store.YES));
                doc.Add(new StringField("Phone", phone, Field.Store.YES));
                doc.Add(new TextField("ResidenceLocation", residenceLocation, Field.Store.YES));
                doc.Add(new TextField("OriginLocation", originLocation, Field.Store.YES));
                writer.AddDocument(doc);

            }
            catch (IndexOutOfRangeException ex) 
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            catch
            {
                throw;
            }
        }
        public List<Record> Search(string name, string surname)
        {
            try
            {
                // Create query parser
                QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "Nome", _analyzer);

                // Creazione della query per cercare per nome e cognome
                //Create query for the search option
                Query query = parser.Parse($"Name:\"{name}\" AND Surname:\"{surname}\"");

                List<Record> records = new List<Record>();

                using (IndexReader reader = DirectoryReader.Open(FSDirectory.Open(new DirectoryInfo(indexPath))))
                {
                    IndexSearcher searcher = new IndexSearcher(reader);
                    TopDocs topDocs = searcher.Search(query, 10);

                    //scroll through the results
                    foreach (ScoreDoc scoreDoc in topDocs.ScoreDocs)
                    {
                        Document doc = searcher.Doc(scoreDoc.Doc);
                        string nameDoc = doc.Get("Name");
                        string surnameDoc = doc.Get("Surname");
                        string phoneDoc = doc.Get("Phone");
                        string residenceLocationDoc = doc.Get("ResidenceLocation");
                        string originLocationDoc = doc.Get("OriginLocation");

                        //Add the result to the dictionary

                        records.Add(new Record { Phone = phoneDoc, Name = nameDoc, Surname = surnameDoc, ResidenceLocation = residenceLocationDoc, OriginLocation = originLocationDoc });

                    }
                    return records;
                }

            }
            catch
            {
                throw;
            }



        }
    }

}

