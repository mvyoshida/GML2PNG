using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	class AreaData
	{
		public  const float elevationMin =    0f;
		public  const float elevationMax = 4000f;		//日本なので、高さの制限を4000にしておく。
		public Location<int> high;
		public float[,] elevation;
		public float min;
		public float max;
		public AreaData(Location<int> high_, Location<int> startPoint, string[] tupleList)
		{
			high = high_;
			elevation = new float[high.x + 1, high.y + 1];
			min = elevationMin;
			max = elevationMax;
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
					var ele = ReadTuple(tupleList[t++]);
					elevation[x, y] = ele;
					if (min > ele)
						min = ele;
					if (max < ele)
						max = ele;
					if (t == tupleList.Length - 1)
						return;
				}
				sx = 0;
			}
		}
		private float ReadTuple(string str)
		{
			string[] values = str.Split(',');
			return (values[0] == "データなし") ? 0 : Math.Max(0, 10 + float.Parse(values[1]));
		}
	}
}
