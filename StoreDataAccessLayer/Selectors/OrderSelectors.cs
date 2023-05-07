using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StoreDataAccessLayer;
using StoreDataAccessLayer.Models;
using CsvHelper;


namespace StoreDataAccessLayer.selectors
{
	public class OrderSelectors
	{
		StoreRepository repo;

		public OrderSelectors(StoreRepository _repo)
		{
			repo = _repo;
		}

		public class StateSale
		{
			public string State { get; set; }
			public double? Sales { get; set; }
		}

		public List<StateSale> GetSalesByYear(int year)
		{
			var orderReturns = repo.context.OrdersReturns.Select(o => o.OrderId).ToList();
			var orders = repo.context.Orders.Where(o => o.OrderDate.Year == year && !orderReturns.Contains(o.OrderId));
			var mergedProductAndOrders = repo.context.Products.Join(orders, p => p.OrderId, o => o.OrderId, (p, o) => new { Order = o, Product = p });
			var salesByState = mergedProductAndOrders.GroupBy(t => t.Order.State)
				.Select(g => new StateSale
				{
					State = g.Key,
					Sales = g.Sum(opr => opr.Product.Sales)
				})
				.ToList();


			return salesByState;

		}

		public double GetIncrementedSalesByYear(int year)
		{
			var orders = repo.context.Orders
				.Where(o => o.OrderDate.Year == year)
				.ToList();

			var orderIds = orders.Select(o => o.OrderId).ToList();

			var products = repo.context.Products
				.Where(p => orderIds.Contains(p.OrderId))
				.ToList();

			var returns = repo.context.OrdersReturns
				.Where(r => orderIds.Contains(r.OrderId))
				.ToList();

			var productsReturned = repo.context.Products
				.Where(p => returns.Select(x => x.OrderId).Contains(p.OrderId))
				.ToList();

			var totalSales = products.Sum(p => p.Sales ?? 0) - productsReturned.Sum(x => x.Sales ?? 0);

			// Apply a 10% increase to total sales
			var incrementedSales = totalSales * (double)1.1m;

			// Group the orders by state and sum the sales for each state
			var salesByState = orders.GroupBy(o => o.State)
				.Select(g => new
				{
					State = g.Key,
					Sales = g.Sum(o => products.Where(p => p.OrderId == o.OrderId).Sum(p => p.Sales) ?? 0)
				})
				.ToList();

			// Group the orders by state and sum the incremented sales for each state
			var incrementedSalesByState = orders.GroupBy(o => o.State)
				.Select(g => new
				{
					State = g.Key,
					IncrementedSales = g.Sum(o => products.Where(p => p.OrderId == o.OrderId).Sum(p => p.Sales) ?? 0) * (double)1.1m
				})
				.ToList();

			Console.WriteLine($"Incremented Sales: {incrementedSales:C}");
			Console.WriteLine($"Total Sales: {totalSales:C}");
			// Display the sales by state and incremented sales by state
			Console.WriteLine("Sales by State:");
			foreach (var item in salesByState)
			{
				Console.WriteLine($"{item.State}: {item.Sales:C}");
			}

			Console.WriteLine("Incremented Sales by State:");
			foreach (var item in incrementedSalesByState)
			{
				Console.WriteLine($"{item.State}: {item.IncrementedSales:C}");
			}

			return totalSales;
		}

		public void DownloadSalesForecastAsCsv(int year)
		{
			var salesForecast = GetSalesByYear(year);

			if (salesForecast == null || !salesForecast.Any())
			{
				Console.WriteLine("No sales forecast data available for the specified year.");
				return;
			}

			var csvFilePath = "C:\\Users\\Public\\Documents";

			using (var writer = new StreamWriter(csvFilePath))
			using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
			{
				csv.WriteHeader<SalesForecastCsvRow>();
				csv.NextRecord();

				foreach (var row in salesForecast)
				{
					var csvRow = new SalesForecastCsvRow
					{
						State = row.State,
						SalesValue = row.Sales
					};

					csv.WriteRecord(csvRow);
					csv.NextRecord();
				}
			}

			Console.WriteLine("Sales forecast data successfully written to CSV file.");
		}


		public class SalesForecastCsvRow
		{
			public string State { get; set; }
			public double? SalesValue { get; set; }
		}

		//		public decimal GetSalesForYear(int yearToQuery)
		//		{
		//			using(var db = new StoreRepository().context)
		//{
		//				var year = 2022; // The year for which to retrieve sales

		//				// Create the parameters for the stored procedure
		//				var yearParam = new SqlParameter("@year", SqlDbType.Int) { Value = year };

		//				// Execute the stored procedure and retrieve the results
		//				var salesByState = db.StateSale
		//									.FromSqlRaw("EXECUTE GetSalesByYear @year", yearParam)
		//									.ToList();
		//				return salesByState;
		//			}
		//		}


	}


}
