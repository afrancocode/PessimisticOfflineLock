using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure;

namespace Concurrency.PessimisticOffline.Session
{
	public sealed class Session : ISession
	{
		private IdentityMap map;

		public Session(string owner, string dbInfo)
		{
			this.Id = Guid.NewGuid();
			this.Owner = owner;
			this.map = new IdentityMap();
			this.DbInfo = new DbSessionInfo(dbInfo);
		}

		public Guid Id { get; private set; }

		public string Owner { get; private set; }

		public IDbSessionInfo DbInfo { get; private set; }

		public IdentityMap IdentityMap
		{
			get { return this.map; }
		}

		public void Close()
		{
			this.map.Clear();
			if (this.DbInfo.Connection.State == ConnectionState.Open)
				this.DbInfo.Connection.Close();
		}

		private sealed class DbSessionInfo : IDbSessionInfo
		{
			private IDbTransaction transaction;
			private string dbInfo;

			public DbSessionInfo(string dbInfo)
			{
				this.dbInfo = dbInfo;
				this.Connection = new SqlConnection(dbInfo);
			}

			public IDbConnection Connection { get; private set; }

			public IDbTransaction Transaction
			{
				get { return this.transaction; }
				set
				{
					if (this.transaction != null && value != null)
						throw new InvalidOperationException();
					this.transaction = value;
				}
			}

			public IDbConnection CreateConnection()
			{
				return new SqlConnection(this.dbInfo);
			}
		}
	}
}
