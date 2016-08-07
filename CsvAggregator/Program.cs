using System;
using System.IO;
using CsvHelper;

namespace CsvAggregator
{
  public class Program
  {
    public static void Main(string[] args)
    {
      string InFolderPath = "./input";
      string OutPath = "./output.csv";

      Console.WriteLine("test");
      Console.ReadLine();
      Console.WriteLine("exit");
      Environment.Exit(0);

      try
      {
        using (CsvWriter csvOut = new CsvWriter(System.IO.File.CreateText(OutPath)))
        {
          foreach (string CsvFile in Directory.EnumerateFiles(InFolderPath))
          {
            try
            {
              using (CsvReader csv = new CsvReader(System.IO.File.OpenText(CsvFile)))
              {
                DateTime fileDate = new DateTime();
                bool friday = fileDate.DayOfWeek.Equals(DayOfWeek.Friday);
                DateTime fileDatePlus1 = fileDate.AddDays(1);
                DateTime fileDatePlus2 = fileDate.AddDays(2);
                while (csv.Read())
                {
                  long id = 0;
                  double price = 0;
                  if (csv.TryGetField<long>(0, out id) && csv.TryGetField<double>(2, out price))
                  {
                    csvOut.WriteRecord(new PricePoint() {Id = id, Price = price, PriceDate = fileDate});
                    if (friday)
                    {
                      csvOut.WriteRecord(new PricePoint() {Id = id, Price = price, PriceDate = fileDatePlus1});
                      csvOut.WriteRecord(new PricePoint() {Id = id, Price = price, PriceDate = fileDatePlus2});
                    }
                  }
                }
              }
            }
            catch
            {
              Console.WriteLine("Failed to open read stream on %s", CsvFile);
            }
          }
        }
      }
      catch
      {
        Console.WriteLine("Failed to open write stream");
      }
    }
  }

  public class PricePoint
  {
    public long Id { get; set; }
    public double Price { get; set; }
    public DateTime PriceDate { get; set; }
  }
}


