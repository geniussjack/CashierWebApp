using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DrvFRLib;

namespace CashierWebApp
{
    public class CashboxDriver : IDisposable
    {
        private DrvFR frDriver;
        private readonly int password;
        private readonly HttpClient httpClient;

        public CashboxDriver()
        {
            frDriver = new DrvFR();
            password = 30;
            httpClient = new HttpClient();
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Connect();
                    return;
                }
                catch
                {
                    Thread.Sleep(300);
                }
            }
            Connect();
        }

        public async Task<(decimal, bool)> Sale(string product, int price, bool isCash, double quantity = 1.0, int department = 1)
        {
            var convertedPrice = price / 100m;

            if (frDriver.ECRMode == 4)
            {
                OpenSession();
            }

            frDriver.Quantity = quantity;
            frDriver.Price = convertedPrice;
            frDriver.Department = department;
            frDriver.StringForPrinting = product;
            frDriver.Tax1 = 1;
            frDriver.Tax2 = 1;
            frDriver.Tax3 = 1;
            frDriver.Tax4 = 1;
            frDriver.Sale();
            if (frDriver.ResultCode == 0)
            {
                var sum = (decimal)quantity * convertedPrice;
                return await Task.FromResult((sum, isCash));
            }
            else
                throw new Exception(frDriver.ResultCodeDescription);
        }

        public async Task<(decimal, bool)> Sale(IEnumerable<Product> products, int sum, bool isCash, int department = 1)
        {

            if (frDriver.ECRMode == 4)
            {
                OpenSession();
            }


            foreach (var product in products)
            {
                frDriver.Quantity = product.Amount;
                var convertedPrice = product.Price / 100m;
                frDriver.Price = convertedPrice;
                frDriver.Department = department;
                frDriver.StringForPrinting = product.Name;
                frDriver.Tax1 = 1;
                frDriver.Tax2 = 1;
                frDriver.Tax3 = 1;
                frDriver.Tax4 = 1;
                frDriver.Sale();
            }

            if (frDriver.ResultCode == 0)
            {
                return await Task.FromResult((sum / 100m, isCash));
            }
            else
                throw new Exception(frDriver.ResultCodeDescription);
        }

        public async Task<(decimal, bool)> ReturnSale(string product, int price, bool isCash, double quantity = 1.0, int department = 1)
        {
            var convertedPrice = price / 100m;

            if (frDriver.ECRMode == 4)
            {
                OpenSession();
            }

            frDriver.Quantity = quantity;
            frDriver.Price = convertedPrice;
            frDriver.Department = department;
            frDriver.StringForPrinting = product;
            frDriver.Tax1 = 1;
            frDriver.Tax2 = 1;
            frDriver.Tax3 = 1;
            frDriver.Tax4 = 1;
            frDriver.ReturnSale();
            if (frDriver.ResultCode == 0)
            {
                var sum = (decimal)quantity * convertedPrice;
                return await Task.FromResult((sum, isCash));
            }
            else
                throw new Exception(frDriver.ResultCodeDescription);
        }

        public async Task<(decimal, bool)> ReturnSale(IEnumerable<Product> products, int sum, bool isCash, int department = 1)
        {

            if (frDriver.ECRMode == 4)
            {
                OpenSession();
            }


            foreach (var product in products)
            {
                frDriver.Quantity = product.Amount;
                var convertedPrice = product.Price / 100m;
                frDriver.Price = convertedPrice;
                frDriver.Department = department;
                frDriver.StringForPrinting = product.Name;
                frDriver.Tax1 = 0;
                frDriver.Tax2 = 0;
                frDriver.Tax3 = 0;
                frDriver.Tax4 = 0;
                frDriver.ReturnSale();
            }

            if (frDriver.ResultCode == 0)
            {
                return await Task.FromResult((sum / 100m, isCash));
            }
            else
                throw new Exception(frDriver.ResultCodeDescription);
        }

        private void Connect()
        {
            frDriver.ConnectionType = 6;
            frDriver.ProtocolType = 0;
            frDriver.IPAddress = "192.168.137.111";
            frDriver.UseIPAddress = true;
            frDriver.TCPPort = 7778;
            frDriver.Timeout = 5000;
            frDriver.Password = password;
            var result = frDriver.Connect();
            var res = frDriver.ResultCodeDescription;
            if (result != 0)
            {
                throw new CashboxException($"{result}: {res}");
            }
        }

        private void OpenSession()
        {
            frDriver.Password = password;
            frDriver.OpenSession();
        }

        public void DailyReport(bool withCleaning = false)
        {
            if (frDriver.ECRMode == 4)
            {
                throw new CashboxException("Смена не открыта.");
            }
            frDriver.Password = password;
            frDriver.PrintDepartmentReport();

            if (withCleaning)
            {
                frDriver.PrintReportWithCleaning();
            }
            else
            {
                frDriver.PrintReportWithoutCleaning();
            }
            if (frDriver.ResultCode != 0)
            {
                throw new CashboxException(frDriver.ResultCodeDescription);
            }
        }

        public void Dispose()
        {
            frDriver = null;
            httpClient.Dispose();
        }
    }
}