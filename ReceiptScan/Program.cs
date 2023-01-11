using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReceiptScan
{

    public class Receipt
    {
        public string Name { get; set; }
        public bool Domestic { get; set; }
        public float Price { get; set; }
        public int? Weight { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("... ").Append(Name).AppendLine();
            sb.Append("    Price: $").Append(Price.ToString("n1")).AppendLine();
            sb.Append("    ").Append(DescriptionCharactersCheck()).AppendLine();
            sb.Append("    Weight: ").Append(Weight != null ? Weight + "g" : "N/A");

            return sb.ToString();
            //return "... " + Name + "\n" +
            //    "   " + "Price: $" + Math.Round(Price, 2) + "\n" + "   " + DescriptionCharactersCheck() + "\n" + "   " + "Weight: " + Weight != null ? Weight + "g" : "N/A";
        }

        private string DescriptionCharactersCheck()
        {
            if (Description.Count() >= 10)
            {

                string cuttedDescription = Description.Substring(0, 10) + "...";
                return cuttedDescription;
            }
            
            return Description;
        }

    }




    public class Program
    {
        static HttpClient client = new HttpClient();

        static async Task<List<Receipt>> GetReceiptAsync(string path)
        {
            List<Receipt> receipts = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                var receiptContent = await response.Content.ReadAsStringAsync();
                receipts = JsonConvert.DeserializeObject<List<Receipt>>(receiptContent);
            }

            return receipts;
        }


        // Printing the result in console
        public static string PrintReceipts(List<Receipt> receipts, string prefix)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append(". ").Append(prefix).AppendLine();

            for (int i = 0; i< receipts.Count; i++)
            {
                if (i == receipts.Count - 1)
                {
                    sb.Append(receipts[i].ToString());
                    continue;
                }
                sb.Append(receipts[i].ToString()).AppendLine();
            }

            return sb.ToString();
        }


        // Function in order to calculate the cost
        public static string CalculationOfCost(List<Receipt> origin, string prefix)
        {
            StringBuilder sb = new StringBuilder();

            float priceSum = origin.Sum(receipt => receipt.Price);
            sb.Append(prefix).Append(" cost: $").Append(priceSum.ToString("n1"));

            return sb.ToString();
        }


        public static string CalculationOfReceipts(List<Receipt> origin, string prefix)
        {
            return prefix + " count: " + origin.Count();
        }



        static async Task Main(string[] args)
        {
            List<Receipt> domesticReceipts = new List<Receipt>();
            List<Receipt> foreignReceipts = new List<Receipt>();

            const string domesticPrefix = "Domestic";
            const string importedPrefix = "Imported";
            // receives the data (fetch)
            var receipts = await GetReceiptAsync("https://interview-task-api.mca.dev/qr-scanner-codes/alpha-qr-gFpwhsQ8fkY1");

            foreach(var receipt in receipts)
            {
                if (receipt.Domestic)
                {
                    domesticReceipts.Add(receipt);
                }
                else
                {
                    foreignReceipts.Add(receipt);
                }
                //CheckIfDomestic(receipt);
            }

            domesticReceipts = domesticReceipts.OrderBy(r => r.Name).ToList();

            foreignReceipts = foreignReceipts.OrderBy(r => r.Name).ToList();

            Console.WriteLine(PrintReceipts(domesticReceipts, domesticPrefix));
            Console.WriteLine(PrintReceipts(foreignReceipts, importedPrefix));


            Console.WriteLine(CalculationOfCost(domesticReceipts, domesticPrefix));
            Console.WriteLine(CalculationOfCost(foreignReceipts, importedPrefix));

            Console.WriteLine(CalculationOfReceipts(domesticReceipts, domesticPrefix));
            Console.WriteLine(CalculationOfReceipts(foreignReceipts, importedPrefix));

            Console.ReadLine();
        }
    }
}
