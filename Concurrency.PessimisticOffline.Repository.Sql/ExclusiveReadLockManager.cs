using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOffline.Infrastructure;
using Concurrency.PessimisticOffline.Infrastructure.Locking;
using Concurrency.PessimisticOffline.Session;

namespace Concurrency.PessimisticOffline.Repository.Sql
{
	public sealed class ExclusiveReadLockManager : IExclusiveReadLockManager
	{
		private static readonly string INSERT_SQL = "INSERT INTO Lock VALUES('{0}', '{1}')";
		private static readonly string DELETE_SINGLE_SQL = "DELETE FROM Lock WHERE LockableId='{0}' and OwnerId='{1}'";
		private static readonly string DELETE_ALL_SQL = "DELETE FROM Lock WHERE OwnerId='{0}'";
		private static readonly string CHECK_SQL = "SELECT LockableId FROM Lock WHERE LockableId='{0}' AND OwnerId='{1}'";

		public void AcquireLock(Guid lockable, Guid owner)
		{
			var session = SessionManager.Manager.Current;
			Debug.Assert(session != null);
			Debug.Assert(session.Id == owner);
			if (!HasLock(lockable, owner))
			{
				try
				{
					var connection = session.DbInfo.CreateConnection();
					connection.Open();
					try
					{
						using (var command = connection.CreateCommand())
						{
							command.CommandType = CommandType.Text;
							command.CommandText = string.Format(INSERT_SQL, lockable.ToString(), owner.ToString());
							using (var transaction = BeginSystemTransaction(connection))
							{
								command.Transaction = transaction;
								try
								{
									command.ExecuteNonQuery();
									transaction.Commit();
								}
								catch
								{
									transaction.Rollback();
									throw;
								}
							}
						}
					}
					finally
					{
						connection.Close();
					}
				}
				catch (DbException)
				{
					throw new ConcurrencyException("Unable to lock " + lockable);
				}
			}
		}

		private IDbTransaction BeginSystemTransaction(IDbConnection connection)
		{
			return connection.BeginTransaction(IsolationLevel.Serializable);
		}

		private bool HasLock(Guid lockable, Guid owner)
		{
			var session = SessionManager.Manager.Current;
			Debug.Assert(session != null);
			Debug.Assert(session.Id == owner);
			try
			{
				var connection = session.DbInfo.CreateConnection();
				connection.Open();
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandType = CommandType.Text;
						command.CommandText = string.Format(CHECK_SQL, lockable.ToString(), owner.ToString());
						using (var transaction = BeginSystemTransaction(connection))
						{
							command.Transaction = transaction;
							try
							{
								var reader = command.ExecuteReader();
								bool hasLock = reader.Read();
								reader.Close();
								transaction.Commit();
								return hasLock;
							}
							catch
							{
								transaction.Rollback();
								throw;
							}
						}
					}
				}
				finally
				{
					connection.Close();
				}
			}
			catch (DbException)
			{
				throw new Exception("Unexpected exception while checking lock table");
			}
		}

		public void ReleaseLock(Guid lockable, Guid owner)
		{
			var session = SessionManager.Manager.Current;
			Debug.Assert(session != null);
			Debug.Assert(session.Id == owner);
			var connection = session.DbInfo.CreateConnection();
			connection.Open();
			try
			{
				using (var command = connection.CreateCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = string.Format(DELETE_SINGLE_SQL, lockable.ToString(), owner.ToString());
					using (var transaction = BeginSystemTransaction(connection))
					{
						command.Transaction = transaction;
						try
						{
							command.ExecuteNonQuery();
							transaction.Commit();
						}
						catch
						{
							transaction.Rollback();
							throw;
						}
					}
				}
			}
			catch (DbException)
			{
				throw new Exception("Unexpected error releasing lock on " + lockable.ToString());
			}
			finally
			{
				connection.Close();
			}
		}

		public void ReleaseAllLocks(Guid owner)
		{
			var session = SessionManager.Manager.Current;
			Debug.Assert(session != null);
			Debug.Assert(session.Id == owner);
			try
			{
				var connection = session.DbInfo.CreateConnection();
				connection.Open();
				try
				{
					using (var command = connection.CreateCommand())
					{
						command.CommandType = CommandType.Text;
						command.CommandText = string.Format(DELETE_ALL_SQL, owner.ToString());
						using (var transaction = BeginSystemTransaction(connection))
						{
							command.Transaction = transaction;
							try
							{
								command.ExecuteNonQuery();
								transaction.Commit();
							}
							catch
							{
								transaction.Rollback();
								throw;
							}
						}
					}
				}
				finally
				{
					connection.Close();
				}
			}
			catch (DbException)
			{
				throw new Exception("Unexpected error releasing lock on " + session.Owner);
			}
		}
	}
}
