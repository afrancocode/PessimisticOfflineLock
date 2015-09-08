using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure.Domain;
using Concurrency.PessimisticOffline.Model;

namespace Concurrency.PessimisticOffline.Repository.Sql.Mapper
{
	public sealed class CustomerMapper : BaseMapper
	{
		private static readonly string LOAD_SQL = "SELECT Id, Name FROM Customer WHERE Id = '{0}'";
		private static readonly string UPDATE_SQL = "UPDATE Customer SET Name = '{0}' WHERE Id = '{1}'";

		protected override string Table { get { return "Customer"; } }

		protected override EntityBase Load(Guid id, IDataReader reader)
		{
			var name = reader["Name"].ToString();
			return Customer.Activate(id, name);
		}

		protected override string GetLoadSQL(Guid id)
		{
			return string.Format(LOAD_SQL, id.ToString());
		}

		protected override string GetUpdateSQL(EntityBase entity)
		{
			var customer = (Customer)entity;
			return string.Format(UPDATE_SQL, customer.Name, customer.Id);
		}
	}
}
