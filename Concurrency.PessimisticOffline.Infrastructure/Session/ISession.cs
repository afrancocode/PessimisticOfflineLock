using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOffline.Infrastructure
{
	public interface ISession
	{
		Guid Id { get; }
		string Owner { get; }
		IDbSessionInfo DbInfo { get; }
		IdentityMap IdentityMap { get; }
		void Close();
	}

	public interface IDbSessionInfo
	{
		IDbConnection Connection { get; }
		IDbTransaction Transaction { get; set; }
		IDbConnection CreateConnection();
	}
}
