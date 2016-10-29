using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	struct Location<T>
	{
		public T x, y;
		public Location(T x_, T y_)
		{
			x = x_;
			y = y_;
		}
	}
}
