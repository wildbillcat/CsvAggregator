using System;
using System.Collections.Generic;
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
      string OutPath = @"C:\Users\wildbillcat\Downloads\PricingStrategy\DataDump\PricePoints\";
      Regex rgx = new Regex(@"pricing-backup-(\S+)\.");
      Console.WriteLine("Start");
      List<PricePoint> pricePoints = new List<PricePoint>();
      int fileCount = 0;
      int outCount = 1;
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
                pricePoints.Add(new PricePoint()
                {
                  Id = id,
                  Price = price,
                  PriceDate = fileDate.ToString(
                    "yyyy-MM-dd")
                });
                if (friday)
                {
                  pricePoints.Add(new PricePoint()
                  {
                    Id = id,
                    Price = price,
                    PriceDate = fileDatePlus1.ToString(
                      "yyyy-MM-dd")
                  });
                  pricePoints.Add(new PricePoint()
                  {
                    Id = id,
                    Price = price,
                    PriceDate = fileDatePlus2.ToString(
                      "yyyy-MM-dd")
                  });
                }
              }
            }
          }
        }
        catch
        {
          Console.WriteLine("Failed to open read stream on %s", csvFile);
        }
        fileCount++;
        if(fileCount > 49)
        {
          try
          {
            using (
              CsvWriter csvOut =
                new CsvWriter(System.IO.File.CreateText(string.Concat(OutPath, "output", outCount.ToString(), ".csv"))))
            {
              csvOut.WriteRecords(pricePoints);
              fileCount = 0;
              outCount++;
              pricePoints = new List<PricePoint>();
            }
          }
          catch
          {
            Console.WriteLine("Failed to open write stream");
          }
        }
      }
    }
  }

  public class Product
  {
    public long Id { get; set; }
    private Dictionary<double, long> Prices;

    public Product(long id)
    {
      Id = id;
      Prices = new Dictionary<double, long>();
    }
  }

  public class PricePoint
  {
    public long Id { get; set; }
    public double Price { get; set; }
    public string PriceDate { get; set; }
  }
}


