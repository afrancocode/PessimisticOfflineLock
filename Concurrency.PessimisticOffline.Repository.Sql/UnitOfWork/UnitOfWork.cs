using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure;
using Concurrency.PessimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOffline.Session;

namespace Concurrency.PessimisticOffline.Repository.Sql.UnitOfWork
{
	public class UnitOfWork : IUnitOfWork
	{
		private Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>> update;

		public UnitOfWork()
		{
			this.update = new Dictionary<IUnitOfWorkRepository, List<IAggregateRoot>>();
		}

		public void RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository)
		{
			List<IAggregateRoot> items = null;
			if (!update.TryGetValue(repository, out items))
			{
				items = new List<IAggregateRoot>();
				update.Add(repository, items);
			}
			items.Add(entity);
		}

		public void Commit()
		{
			var manager = SessionManager.Manager;
			var session = manager.Current;

			var transaction = session.DbInfo.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
			session.DbInfo.Transaction = transaction;
			try
			{
				UpdateDirty();
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

		private void UpdateDirty()
		{
			foreach (var updateInfo in this.update)
			{
				var persistTo = updateInfo.Key;
				while (updateInfo.Value.Any())
				{
					var entity = updateInfo.Value[0];
					persistTo.PersistUpdateOf(entity);
					updateInfo.Value.RemoveAt(0);
				}
			}
		}

		public static readonly IUnitOfWork Empty = new EmptyUnitOfWork();

		private sealed class EmptyUnitOfWork : IUnitOfWork
		{
			void IUnitOfWork.RegisterAmended(IAggregateRoot entity, IUnitOfWorkRepository repository) { }
			void IUnitOfWork.Commit() { }
		}
	}
}
