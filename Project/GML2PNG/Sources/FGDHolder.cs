using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	class FGDHolder
	{
		//日本なので、高さの制限を4000にしておく。
		//超えた場合は情報落ちします。
		const int highLimit = 4000;

		Dictionary<Area, AreaData> dicDatas;
		Location<int> high;

		public FGDHolder()
		{
			dicDatas = new Dictionary<Area, AreaData>(); ;
		}

		public void Add(string filename)
		{
			FGDData fgd = new FGDMaker().Make(filename);
			if (dicDatas.Count == 0)
			{
				high = fgd.high;
			}
			else
			{
				if (high.x != fgd.high.x || high.y != fgd.high.y)
				{
					//複数のファイルで異なるグリッドセル配列数の対応はしていません。
					System.Diagnostics.Debug.WriteLine("データサイズにばらつきがある");
					return;
				}
			}
			Area area = new Area(fgd.lowerCorner, fgd.upperCorner);
			if (dicDatas.ContainsKey(area))
			{
				//要素の加工
				dicDatas[area].Add(fgd.high, fgd.startPoint, fgd.tupleList);
			}
			else
			{
				//要素の追加
				dicDatas.Add(area, new AreaData(fgd.high, fgd.startPoint, fgd.tupleList));
			}
		}
		public void Export()
		{
			AreaData[,] areaDataArray = NewAreaDataArray(NewSortedSetX(), NewSortedSetY());
			int areaArrayX = areaDataArray.GetLength(0);
			int areaArrayY = areaDataArray.GetLength(1);
			int areaPixelX = high.x + 1;
			int areaPixelY = high.y + 1;
			int outputImageX = (areaArrayX * areaPixelX);
			int outputImageY = (areaArrayY * areaPixelY);
			byte[] hightMap = new byte[outputImageY * outputImageX * 2];
			for (int areaY = 0; areaY < areaArrayY; ++areaY)
			{
				for (int areaX = 0; areaX < areaArrayX; ++areaX)
				{
					AreaData areaData = areaDataArray[areaX, areaY];
					if (areaData != null)
					{
						for (int y = 0; y < areaPixelY; ++y)
						{
							int i = ((areaY * areaPixelY + y) * outputImageX + (areaX * areaPixelX)) * 2;
							for (int x = 0; x < areaPixelX; ++x)
							{
								UInt16 v = (UInt16)(areaData.elevation[x, y] *65535 / highLimit);
								hightMap[i++] = (byte)(v & 0xff);
								hightMap[i++] = (byte)((v >> 8) & 0xff);
							}
						}
					}
					else
					{
						for (int y = 0; y < areaPixelY; ++y)
						{
							int i = ((areaY * areaPixelY + y) * outputImageX + (areaX * areaPixelX)) * 2;
							for (int x = 0; x < areaPixelX; ++x)
							{
								hightMap[i++] = 0;
								hightMap[i++] = 0;
							}
						}
					}
				}
			}
			var bmp = System.Windows.Media.Imaging.BitmapImage.Create
			(
				outputImageX, outputImageY, 96,96,
				System.Windows.Media.PixelFormats.Gray16, 
				System.Windows.Media.Imaging.BitmapPalettes.Gray16,
				hightMap,
				outputImageX * 16 / 8
			);
			Microsoft.Win32.SaveFileDialog sd = new Microsoft.Win32.SaveFileDialog();
			sd.Filter = "pngファイル(*.png)|*.png";
			sd.FileName = "heightMap.png";
			sd.Title = "pngGファイル名を指定してください。";
			bool? result = sd.ShowDialog();
			if (result == true)
			{
				using (System.IO.FileStream stream = new System.IO.FileStream(sd.FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write))
				{
					var encoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
					encoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(bmp));
					encoder.Save(stream);
				}
			}
		}
		SortedSet<float> NewSortedSetX()
		{
			SortedSet<float> ret = new SortedSet<float>();
			foreach (KeyValuePair<Area, AreaData> v in dicDatas)
			{
				ret.Add(v.Key.lowerCorner.x);
				ret.Add(v.Key.upperCorner.x);
			}
			return ret;
		}
		SortedSet<float> NewSortedSetY()
		{
			SortedSet<float> ret = new SortedSet<float>();
			foreach (KeyValuePair<Area, AreaData> v in dicDatas)
			{
				ret.Add(v.Key.lowerCorner.y);
				ret.Add(v.Key.upperCorner.y);
			}
			return ret;
		}
		Tuple<float, float>[] NewPairValueArray(SortedSet<float> v)
		{
			float[] va = v.ToArray<float>();
			float[] v1 = new float[v.Count - 1];
			float[] v2 = new float[v.Count - 1];
			Array.Copy(va, 0, v1, 0, v.Count - 1);
			Array.Copy(va, 1, v2, 0, v.Count - 1);

			Tuple<float, float>[] ret = new Tuple<float, float>[v.Count - 1];
			for (int i = 0; i < v.Count - 1; ++i)
			{
				ret[i] = new Tuple<float, float>(v1[i], v2[i]);
			}
			return ret;
		}
		AreaData[,] NewAreaDataArray(SortedSet<float> xSet, SortedSet<float> ySet)
		{
			Tuple<float, float>[] xTuple = NewPairValueArray(xSet);
			Tuple<float, float>[] yTuple = NewPairValueArray(ySet);
			System.Array.Reverse(yTuple);

			AreaData[,] areadata = new AreaData[xTuple.Length, yTuple.Length];
			int y = 0;
			foreach (var yt in yTuple)
			{
				int x = 0;
				foreach (var xt in xTuple)
				{
					Area area = new Area(new Location<float>(xt.Item1, yt.Item1), new Location<float>(xt.Item2, yt.Item2));
					if (dicDatas.ContainsKey(area))
					{
						areadata[x, y] = dicDatas[area];
					}
					++x;
				}
				++y;
			}

			return areadata;
		}
		public void Clear()
		{
			dicDatas.Clear();
			high.x = 0;
			high.y = 0;
		}
	}
}
