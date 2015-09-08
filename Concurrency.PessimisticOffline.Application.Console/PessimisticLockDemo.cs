using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Concurrency.PessimisticOffline.Infrastructure;
using Concurrency.PessimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOffline.Model;
using Concurrency.PessimisticOffline.Repository.Sql;
using Concurrency.PessimisticOffline.Repository.Sql.Repositories;
using Concurrency.PessimisticOffline.Repository.Sql.UnitOfWork;
using Concurrency.PessimisticOffline.Session;

namespace Concurrency.PessimisticOffline.Application.Console
{
	public sealed class TestInfo
	{
		public void Initialize(Guid entityId, string owner)
		{
			this.EntityId = entityId;
			this.Session = SessionManager.Manager.Open(owner);
			this.uow = new UnitOfWork();
			this.repository = new CustomerRepository(this.uow);
		}

		public ISession Session;
		public Guid EntityId;
		public Customer Entity;
		public IUnitOfWork uow;
		public ICustomerRepository repository;

		public void LoadCustomer()
		{
			SessionManager.Manager.SetCurrent(this.Session);
			var connection = Session.DbInfo.Connection;
			connection.Open();
			try
			{
				this.Entity = this.repository.FindBy(EntityId); // Read entity and tries to get an exclusive lock
			}
			catch(Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		public void SaveCustomer()
		{
			SessionManager.Manager.SetCurrent(this.Session);
			var connection = Session.DbInfo.Connection;
			connection.Open();
			try
			{
				this.repository.Save(Entity);
				this.uow.Commit(); // Release the lock
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
			finally
			{
				connection.Close();
			}
		}

		public void CloseSession()
		{
			SessionManager.Manager.SetCurrent(this.Session);
			var closeSession = this.Session;
			this.Session = null;
			SessionManager.Manager.Close(closeSession.Id);
		}
	}

	public sealed class PessimisticLockDemo
	{
		public void EditSameEntity()
		{
			var id = new Guid("a004c037-294d-4796-b51b-070a4e832241");
			SessionManager.SetupManager(new ExclusiveReadLockManager());
			var manager = SessionManager.Manager;

			var user1 = new TestInfo();
			user1.Initialize(id, "alfonso");

			var user2 = new TestInfo();
			user2.Initialize(id, "zaid");

			user1.LoadCustomer();
			user2.LoadCustomer();

			user1.CloseSession();

			user2.LoadCustomer();
			user2.CloseSession();

			manager.Close();
		}
	}
}
