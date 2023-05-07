using System;
using System.Data.SqlClient;
using StoreDataAccessLayer;
using StoreDataAccessLayer.selectors;

namespace StoreConsoleApp
{
	class Program
	{
		static void Main(string[] args)
		{
			var repo = new StoreRepository();
			OrderSelectors orderSelector = new OrderSelectors(repo);

			//var ans = orderSelector.GetSalesByYear(2018);
			//foreach (var i in ans)
			//{
			//	Console.WriteLine(i.Sales.ToString() + " " + i.State.ToString());
			//}


			var ans = orderSelector.GetIncrementedSalesByYear(2019);
			// Display the total sales and incremented sales

			orderSelector.DownloadSalesForecastAsCsv(2018);

		}
	}
}
