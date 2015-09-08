using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using Concurrency.PessimisticOffline.Infrastructure.Domain;

namespace Concurrency.PessimisticOffline.Infrastructure
{
	public sealed class IdentityMap
	{
		private Dictionary<Guid, EntityBase> entities;

		public IdentityMap()
		{
			this.entities = new Dictionary<Guid, EntityBase>();
		}

		public EntityBase Get(Guid key)
		{
			EntityBase entity = null;
			this.entities.TryGetValue(key, out entity);
			return entity;
		}

		public void Add(Guid key, EntityBase value)
		{
			Debug.Assert(!this.entities.ContainsKey(key));
			this.entities.Add(key, value);
		}

		public void Remove(Guid key)
		{
			this.entities.Remove(key);
		}

		public void Clear()
		{
			this.entities.Clear();
		}
	}
}
