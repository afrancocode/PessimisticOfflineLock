using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOffline.Infrastructure.Domain;
using Concurrency.PessimisticOffline.Session;

namespace Concurrency.PessimisticOffline.Repository.Sql.Mapper
{
	public abstract class BaseMapper
	{
		protected abstract string Table { get; }

		public EntityBase Find(Guid id)
		{
			Debug.Assert(SessionManager.Manager.Current != null);
			var session = SessionManager.Manager.Current;
			var entity = session.IdentityMap.Get(id);
			if(entity == null)
			{
				try
				{
					var lockManager = SessionManager.Manager.LockManager;
					lockManager.AcquireLock(id, session.Id); // Try to adquire Exclusive Read Lock
					var connection = session.DbInfo.Connection;
					using (var command = connection.CreateCommand())
					{
						command.CommandType = System.Data.CommandType.Text;
						command.CommandText = GetLoadSQL(id);
						var reader = command.ExecuteReader();
						if(reader.Read())
						{
							entity = Load(id, reader);
							session.IdentityMap.Add(id, entity);
						}
					}
				}
				catch (DbException dbe)
				{
					throw new Exception("unexpected error finding " + Table + ": " + id + "\n" + dbe.Message);
				}
			}
			return entity;
		}

		public void Update(EntityBase entity)
		{
			var session = SessionManager.Manager.Current;
			Debug.Assert(session != null);
			try
			{
				var connection = session.DbInfo.Connection;
				var lockManager = SessionManager.Manager.LockManager;
				using(var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = GetUpdateSQL(entity);
					command.Transaction = session.DbInfo.Transaction;
					var rows = command.ExecuteNonQuery();
					lockManager.ReleaseLock(entity.Id, session.Id); // Release lock
				}
			}
			catch(DbException dbe)
			{
				throw new Exception("Unexpected error updating: " + dbe.Message);
			}
		}

		protected abstract EntityBase Load(Guid id, IDataReader reader);

		protected abstract string GetLoadSQL(Guid id);
		protected abstract string GetUpdateSQL(EntityBase entity);
	}
}
