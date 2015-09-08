using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOffline.Model;
using Concurrency.PessimisticOffline.Repository.Sql.Mapper;

namespace Concurrency.PessimisticOffline.Repository.Sql.Repositories
{
	public sealed class CustomerRepository : Repository<Customer>, ICustomerRepository
	{
		public CustomerRepository(IUnitOfWork uow)
			: base(uow)
		{ }

		protected override BaseMapper CreateMapper()
		{
			return new CustomerMapper();
		}
	}
}
