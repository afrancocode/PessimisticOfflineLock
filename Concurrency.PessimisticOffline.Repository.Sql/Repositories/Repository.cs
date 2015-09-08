using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Concurrency.PessimisticOffline.Infrastructure;
using Concurrency.PessimisticOffline.Infrastructure.Domain;
using Concurrency.PessimisticOffline.Infrastructure.UnitOfWork;
using Concurrency.PessimisticOffline.Repository.Sql.Mapper;

namespace Concurrency.PessimisticOffline.Repository.Sql.Repositories
{
	public abstract class Repository<T> : IRepository<T>, IUnitOfWorkRepository where T : EntityBase, IAggregateRoot
	{
		private IUnitOfWork uow;
		private BaseMapper mapper;

		public Repository(IUnitOfWork uow)
		{
			Debug.Assert(uow != null);
			this.uow = uow;
		}

		protected BaseMapper Mapper
		{
			get
			{
				if (this.mapper == null)
					this.mapper = CreateMapper();
				return this.mapper;
			}
		}

		public T FindBy(Guid id)
		{
			return (T)Mapper.Find(id);
		}

		public void Save(T entity)
		{
			this.uow.RegisterAmended(entity, this);
		}

		protected abstract BaseMapper CreateMapper();

		#region IUnitOfWorkRepository implementation

		void IUnitOfWorkRepository.PersistUpdateOf(IAggregateRoot entity)
		{
			PersistUpdate((T)entity);
		}

		#endregion

		protected virtual void PersistUpdate(T entity)
		{
			this.Mapper.Update(entity);
		}
	}
}
