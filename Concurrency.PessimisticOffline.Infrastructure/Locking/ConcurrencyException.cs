﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOffline.Infrastructure.Locking
{
	public class ConcurrencyException : Exception
	{
		public ConcurrencyException(string message)
			: base(message) 
		{ }
	}
}
