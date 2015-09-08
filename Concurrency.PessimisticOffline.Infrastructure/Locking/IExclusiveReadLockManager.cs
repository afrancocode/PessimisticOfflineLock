using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOffline.Infrastructure.Locking
{
	public interface IExclusiveReadLockManager
	{
		void AcquireLock(Guid lockable, Guid owner);
		void ReleaseLock(Guid lockable, Guid owner);
		void ReleaseAllLocks(Guid owner);
	}
}
