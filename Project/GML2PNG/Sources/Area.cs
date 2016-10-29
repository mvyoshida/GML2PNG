using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	struct Area
	{
		public Location<float> lowerCorner;
		public Location<float> upperCorner;
		public Area(Location<float> lowerCorner_, Location<float> upperCorner_)
		{
			lowerCorner = lowerCorner_;
			upperCorner = upperCorner_;
		}
	}
}
