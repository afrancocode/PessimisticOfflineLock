using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOffline.Infrastructure.UnitOfWork
{
	public interface IUnitOfWorkRepository
	{
		void PersistUpdateOf(IAggregateRoot entity);
	}
}
