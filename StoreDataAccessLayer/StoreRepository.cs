using System;
using StoreDataAccessLayer.Models;

namespace StoreDataAccessLayer
{
	public class StoreRepository
	{
		public StoreDBContext context;

		public StoreRepository()
		{
			context = new StoreDBContext();
		}
	}
}
