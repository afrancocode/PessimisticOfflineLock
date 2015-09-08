using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure.Locking;

namespace Concurrency.PessimisticOffline.Infrastructure
{
	public interface ISessionManager
	{
		ISession Current { get; }
		IExclusiveReadLockManager LockManager { get; }

		ISession Open(string owner);
		void Close(Guid id);
		void SetCurrent(ISession session);
		void Close();
	}
}
