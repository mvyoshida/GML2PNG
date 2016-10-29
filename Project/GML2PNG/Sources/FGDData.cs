using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	struct FGDData
	{
		public Location<float> lowerCorner;
		public Location<float> upperCorner;
		public Location<int> low;
		public Location<int> high;
		public Location<int> startPoint;
		public string[] tupleList;
		public FGDData(Location<float> lowerCorner_, Location<float> upperCorner_, Location<int> low_, Location<int> high_, Location<int> startPointeList_, string[] tupleList_)
		{
			lowerCorner = lowerCorner_;
			upperCorner = upperCorner_;
			low = low_;
			high = high_;
			startPoint = startPointeList_;
			tupleList = tupleList_;
		}
	}
}
