using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure;
using Concurrency.PessimisticOffline.Infrastructure.Domain;

namespace Concurrency.PessimisticOffline.Model
{
	public sealed class Customer : EntityBase, IAggregateRoot
	{
		private Customer() { }

		public string Name { get; set; }

		public static Customer Activate(Guid id, string name)
		{
			return new Customer() { Id = id, Name = name };
		}
	}
}
