using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure;

namespace Concurrency.PessimisticOffline.Model
{
	public interface ICustomerRepository : IRepository<Customer>
	{
	}
}
