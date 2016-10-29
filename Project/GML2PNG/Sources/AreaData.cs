using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	class AreaData
	{
		public Location<int> high;
		public float[,] elevation;
		public AreaData(Location<int> high_, Location<int> startPoint, string[] tupleList)
		{
			high = high_;
			elevation = new float[high.x + 1, high.y + 1];
			UpdateElevation(startPoint, tupleList);
		}
		public void Add(Location<int> high_, Location<int> startPoint, string[] tupleList)
		{
			UpdateElevation(startPoint, tupleList);
		}
		private void UpdateElevation(Location<int> startPoint, string[] tupleList)
		{
			int sx = startPoint.x;
			if (tupleList.Length == 0)
				return;
			int t = 0;
			for (int y = startPoint.y; y < high.y + 1; ++y)
			{
				for (int x = sx; x < high.x + 1; ++x)
				{
					elevation[x, y] = ReadTuple(tupleList[t++]);
					if (t == tupleList.Length - 1)
						return;
				}
				sx = 0;
			}
		}
		private float ReadTuple(string str)
		{
			string[] values = str.Split(',');
			return (values[0] == "データなし") ? 0 : float.Parse(values[1]);
		}
	}
}
