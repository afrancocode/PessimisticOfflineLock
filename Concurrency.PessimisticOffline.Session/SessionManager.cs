using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOffline.Infrastructure;
using Concurrency.PessimisticOffline.Infrastructure.Locking;

namespace Concurrency.PessimisticOffline.Session
{
	public sealed class SessionManager : ISessionManager
	{
		private static ISessionManager manager;
		private static readonly string SQL_CONNECTION = @"Data Source=(LocalDB)\v11.0;AttachDbFilename=D:\GitHub\PessimisticOfflineLock\Concurrency.PessimisticOffline.Application.Console\Db\PessimisticDb.mdf;Integrated Security=True";

		public static void SetupManager(IExclusiveReadLockManager lockManager)
		{
			Debug.Assert(manager == null);
			manager = new SessionManager(lockManager);
		}

		public static ISessionManager Manager { get { return manager; } }

		private List<ISession> sessions;
		private ISession current;

		private SessionManager(IExclusiveReadLockManager lockManager)
		{
			Debug.Assert(lockManager != null);
			this.sessions = new List<ISession>();
			this.LockManager = lockManager;
		}

		public ISession Current { get { return current; } }

		public IExclusiveReadLockManager LockManager { get; private set; }

		public ISession Open(string owner)
		{
			var session = new Session(owner, SQL_CONNECTION);
			this.sessions.Add(session);
			return session;
		}

		public void SetCurrent(ISession session)
		{
			this.current = session;
		}

		public void Close(Guid id)
		{
			var session = this.sessions.Find(s => s.Id == id);
			if(session != null)
			{
				var isCurrent = (session == Current);
				this.LockManager.ReleaseAllLocks(session.Id);
				session.Close();
				if (isCurrent)
					SetCurrent(null);
			}
		}

		public void Close()
		{
			while(sessions.Count > 0)
			{
				sessions[0].Close();
				sessions.RemoveAt(0);
			}
		}
	}
}
