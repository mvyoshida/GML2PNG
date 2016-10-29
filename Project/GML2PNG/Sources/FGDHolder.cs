using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	class FGDHolder
	{
		Dictionary<Area, AreaData> dicDatas;
		public Location<int> high;
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
			//16が返る、つまりは2byte
			//int pixelByteSize = System.Drawing.Image.GetPixelFormatSize(System.Drawing.Imaging.PixelFormat.Format16bppGrayScale) / 8;
			AreaData[,] areadata = NewAreaData(NewSortedSetX(), NewSortedSetY());
			int xArea = areadata.GetLength(0);
			int yArea = areadata.GetLength(1);
			int xHigh = high.x + 1;
			int yHigh = high.y + 1;
			int xImage = (xArea * xHigh);
			int yImage = (yArea * yHigh);
			byte[] hightMap = new byte[yImage * xImage * 2];
			for (int ya = 0; ya < yArea; ++ya)
			{
				for (int xa = 0; xa < xArea; ++xa)
				{
					AreaData a = areadata[xa, ya];
					if (a != null)
					{
						for (int y = 0; y < yHigh; ++y)
						{
							for (int x = 0; x < xHigh; ++x)
							{
								UInt16 v = (UInt16)(a.elevation[x, y] *65535 / 5000 );
								int i = ((ya * yHigh + y) * xImage + (xa * xHigh + x)) * 2;
								hightMap[i + 0] = (byte)(v & 0xff);
								hightMap[i + 1] = (byte)((v >> 8) & 0xff);
							}
						}
					}
					else
					{
						for (int y = 0; y < yHigh; ++y)
						{
							for (int x = 0; x < xHigh; ++x)
							{
								int i = ((ya * yHigh + y) * xImage + (xa * xHigh + x)) * 2;
								hightMap[i + 0] = 0;
								hightMap[i + 1] = 0;
							}
						}
					}
				}
			}
			var bmp = System.Windows.Media.Imaging.BitmapImage.Create
			(
				xImage, yImage, 96,96,
				System.Windows.Media.PixelFormats.Gray16, 
				System.Windows.Media.Imaging.BitmapPalettes.Gray16,
				hightMap,
				xImage * 16 / 8
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
			//using (System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(xArea * xHigh, yArea * yHigh, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale))
			//{
			//	if(false)
			//	{
			//		System.Drawing.Imaging.BitmapData bd = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format16bppGrayScale);
			//		System.Runtime.InteropServices.Marshal.Copy(hightMap, 0, bd.Scan0, hightMap.Length);
			//		bmp.UnlockBits(bd);
			//	}
			//	System.Windows.Forms.SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
			//	sd.Filter = "pngファイル(*.png)|*.png";
			//	sd.FileName = "heightMap.png";
			//	sd.Title = "pngGファイル名を指定してください。";
			//	if (sd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			//	{
			//		bmp.Save(sd.FileName, System.Drawing.Imaging.ImageFormat.Png);
			//	}
			//}
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
		Tuple<float, float>[] NewPairValue(SortedSet<float> v)
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
		AreaData[,] NewAreaData(SortedSet<float> xSet, SortedSet<float> ySet)
		{
			Tuple<float, float>[] xTuple = NewPairValue(xSet);
			Tuple<float, float>[] yTuple = NewPairValue(ySet);
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
