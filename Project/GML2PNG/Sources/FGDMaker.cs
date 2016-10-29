using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GML2PNG
{
	class FGDMaker
	{
		public FGDMaker()
		{
		}
		public FGDData Make(string filename)
		{
			System.Xml.XmlDocument myXmlDocument = new System.Xml.XmlDocument();
			myXmlDocument.Load(filename);
			System.Xml.XmlNamespaceManager nsmgr = new System.Xml.XmlNamespaceManager(myXmlDocument.NameTable);
			nsmgr.AddNamespace("fgd", "http://fgd.gsi.go.jp/spec/2008/FGD_GMLSchema");
			nsmgr.AddNamespace("gml", "http://www.opengis.net/gml/3.2");
			nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
			nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
			nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

			return new FGDData
			(
				Swap<float>(Split<float>(myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:boundedBy/gml:Envelope/gml:lowerCorner", nsmgr).InnerText)),
				Swap<float>(Split<float>(myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:boundedBy/gml:Envelope/gml:upperCorner", nsmgr).InnerText)),
				Split<int>(myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:gridDomain/gml:Grid/gml:limits/gml:GridEnvelope/gml:low", nsmgr).InnerText),
				Split<int>(myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:gridDomain/gml:Grid/gml:limits/gml:GridEnvelope/gml:high", nsmgr).InnerText),
				Split<int>(myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:coverageFunction/gml:GridFunction/gml:startPoint", nsmgr).InnerText),
				SplitStrings(myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:rangeSet/gml:DataBlock/gml:tupleList", nsmgr).InnerText)
			);
		}

		private Location<T> Swap<T>(Location<T> value)
		{
			T tmp = value.x;
			value.x = value.y;
			value.y = tmp;
			return value;
		}
		private Location<T> Split<T>(string str)
		{
			string[] xy = str.Split(' ');
			return new Location<T>((T)Convert.ChangeType(xy[0], typeof(T)), (T)Convert.ChangeType(xy[1], typeof(T)));
		}
		private string[] SplitStrings(string str)
		{
			char[] splits = { '\r', '\n', };
			return str.Split(splits, StringSplitOptions.RemoveEmptyEntries);
		}
	}
}
