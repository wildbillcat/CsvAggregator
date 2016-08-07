using System;
using System.IO;
using CsvHelper;
using System.Text.RegularExpressions;

namespace CsvAggregator
{
  public class Program
  {
    public static void Main(string[] args)
    {
      string InFolderPath = @"C:\Users\wildbillcat\Downloads\PricingStrategy\DataDump\PricingBackupCsvs";
      string OutPath = @"C:\Users\wildbillcat\Downloads\PricingStrategy\DataDump\output.csv";
      Regex rgx = new Regex(@"pricing-backup-(\S+)\.");
      Console.WriteLine("Start");
      try
      {
        using (CsvWriter csvOut = new CsvWriter(System.IO.File.CreateText(OutPath)))
        {
          foreach (string csvFile in Directory.EnumerateFiles(InFolderPath))
          {
            try
            {
              using (CsvReader csv = new CsvReader(File.OpenText(csvFile)))
              {
                string dateText = rgx.Matches(csvFile)[0].Groups[1].Value;
                Console.WriteLine(dateText);
                DateTime fileDate = DateTime.Parse(dateText);
                bool friday = fileDate.DayOfWeek.Equals(DayOfWeek.Friday);
                DateTime fileDatePlus1 = fileDate.AddDays(1);
                DateTime fileDatePlus2 = fileDate.AddDays(2);
                while (csv.Read())
                {
                  long id = 0;
                  double price = 0;
                  if (csv.TryGetField("coresense_id", out id) && csv.TryGetField("price", out price))
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
              Console.WriteLine("Failed to open read stream on %s", csvFile);
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


