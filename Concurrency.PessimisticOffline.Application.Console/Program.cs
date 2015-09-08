using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Concurrency.PessimisticOffline.Application.Console
{
	class Program
	{
		static void Main(string[] args)
		{
			new PessimisticLockDemo().EditSameEntity();
		}
	}
}
