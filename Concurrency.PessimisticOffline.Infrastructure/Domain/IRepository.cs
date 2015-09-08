using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOffline.Infrastructure
{
	public interface IRepository<T> where T : IAggregateRoot
	{
		T FindBy(Guid id);
		void Save(T entity);
	}
}
