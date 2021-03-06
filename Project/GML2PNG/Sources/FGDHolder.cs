﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	class FGDHolder
	{
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
			Export(dicDatas);
		}
		public void Clear()
		{
			dicDatas.Clear();
			high.x = 0;
			high.y = 0;
		}
		void Export(Dictionary<Area, AreaData> dic)
		{
			float[] totalArea = GetTotalArea(dic);
			AreaData[,] areaDataArray = NewAreaDataArray(dic);
			float[] limit = ElevationLimit(areaDataArray);
			//int highLimit = (int)(limit[1] - limit[0]);
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
					//heightMap:Landscape=128:1m
					//例：heightMapの値が35680=2912+0x800でUE4のLandscapeはスケール100の状態で22.75m
					//UE4側でスケールを500にする想定とするとheightMapの値25.6で1m扱いとなる
					//-1280m～1279m程度の表現が可能。
					const float ue4DefaultScale = 100f;
					const float ue4ApplyScale = 500f;
					const float hightMapPerLandscape = 128f;
					const float coefficient = hightMapPerLandscape / (ue4ApplyScale / ue4DefaultScale);
					const UInt16 ue4SeaLevel = 0x8000;
					AreaData areaData = areaDataArray[areaX, areaY];
					if (areaData != null)
					{
						for (int y = 0; y < areaPixelY; ++y)
						{
							int i = ((areaY * areaPixelY + y) * outputImageX + (areaX * areaPixelX)) * 2;
							for (int x = 0; x < areaPixelX; ++x)
							{
								float height = areaData.elevation[x, y];
								UInt16 v = (UInt16)(height * coefficient + ue4SeaLevel);
								hightMap[i++] = (byte)(v & 0xff);
								hightMap[i++] = (byte)((v >> 8) & 0xff);
							}
						}
					}
					else
					{
						const float height =  AreaData.elevationSeaLevel;
						UInt16 v = (UInt16)(height * coefficient + ue4SeaLevel);
						byte v0 = (byte)(v & 0xff);
						byte v1 = (byte)((v >> 8) & 0xff);
						for (int y = 0; y < areaPixelY; ++y)
						{
							int i = ((areaY * areaPixelY + y) * outputImageX + (areaX * areaPixelX)) * 2;
							for (int x = 0; x < areaPixelX; ++x)
							{
								hightMap[i++] = v0;
								hightMap[i++] = v1;
							}
						}
					}
				}
			}
			var bmp = System.Windows.Media.Imaging.BitmapImage.Create
			(
				outputImageX, outputImageY, 96, 96,
				System.Windows.Media.PixelFormats.Gray16,
				System.Windows.Media.Imaging.BitmapPalettes.Gray16,
				hightMap,
				outputImageX * 16 / 8
			);
			Microsoft.Win32.SaveFileDialog sd = new Microsoft.Win32.SaveFileDialog();
			sd.Filter = "pngファイル(*.png)|*.png";
			sd.FileName = "heightMap(" + totalArea[0] + "," + totalArea[2] + ")-(" + totalArea[1] + "," + totalArea[3] + ")[" + limit[0] + "," + limit[1] + "].png";
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
		SortedSet<float> NewSortedSetX(Dictionary<Area, AreaData> dic)
		{
			SortedSet<float> ret = new SortedSet<float>();
			foreach (KeyValuePair<Area, AreaData> v in dic)
			{
				ret.Add(v.Key.lowerCorner.x);
				ret.Add(v.Key.upperCorner.x);
			}
			return ret;
		}
		SortedSet<float> NewSortedSetY(Dictionary<Area, AreaData> dic)
		{
			SortedSet<float> ret = new SortedSet<float>();
			foreach (KeyValuePair<Area, AreaData> v in dic)
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
		AreaData[,] NewAreaDataArray(Dictionary<Area, AreaData> dic)
		{
			Tuple<float, float>[] xTuple = NewPairValueArray(NewSortedSetX(dic));
			Tuple<float, float>[] yTuple = NewPairValueArray(NewSortedSetY(dic));

			//北西から南東に処理していくので、Y軸の順番はひっくり返す
			System.Array.Reverse(yTuple);

			AreaData[,] areadata = new AreaData[xTuple.Length, yTuple.Length];
			int y = 0;
			foreach (var yt in yTuple)
			{
				int x = 0;
				foreach (var xt in xTuple)
				{
					Area area = new Area(new Location<float>(xt.Item1, yt.Item1), new Location<float>(xt.Item2, yt.Item2));
					if (dic.ContainsKey(area))
					{
						areadata[x, y] = dic[area];
					}
					++x;
				}
				++y;
			}
			return areadata;
		}
		float[] ElevationLimit(AreaData[,] areaDataArray)
		{
			float[] ret = { AreaData.elevationMax, AreaData.elevationMin,};
			foreach(var v in areaDataArray)
			{
				if(v != null)
				{
					v.FixLimit();
					if (ret[0] > v.min)
						ret[0] = v.min;
					if (ret[1] < v.max)
						ret[1] = v.max;
				}
			}
			return ret;
		}
		float[] GetTotalArea(Dictionary<Area, AreaData> dic)
		{
			float xmin = 180;
			float xmax = -180;
			float ymin = 90;
			float ymax = -90;
			//foreach (KeyValuePair<Area, AreaData> v in dic)
			foreach (var v in dic)
			{
				if (xmin > v.Key.lowerCorner.x)
					xmin = v.Key.lowerCorner.x;
				if (xmax < v.Key.upperCorner.x)
					xmax = v.Key.upperCorner.x;
				if (ymin > v.Key.lowerCorner.y)
					ymin = v.Key.lowerCorner.y;
				if (ymax < v.Key.upperCorner.y)
					ymax = v.Key.upperCorner.y;
			}
			return new float[4]{ xmin, xmax, ymin, ymax};
		}
	}
}
