using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace GML2PNG
{
	class FGDMaker
	{
		public FGDMaker()
		{
		}
		public FGDData Make(string filename)
		{
			XmlDocument myXmlDocument = new XmlDocument();
			myXmlDocument.Load(filename);
			//{
			//	XmlNode node;
			//	node = myXmlDocument.DocumentElement;
			//	DisplayTree(node, "");
			//	Console.Read();
			//}
			//XmlNodeList list = myXmlDocument.SelectNodes("/Dataset/DEM/coverage/boundedBy/Envelope");
			XmlNamespaceManager nsmgr = new XmlNamespaceManager(myXmlDocument.NameTable);
			nsmgr.AddNamespace("fgd", "http://fgd.gsi.go.jp/spec/2008/FGD_GMLSchema");
			nsmgr.AddNamespace("gml", "http://www.opengis.net/gml/3.2");
			nsmgr.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
			nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
			nsmgr.AddNamespace("xsi", "http://www.w3.org/2001/XMLSchema-instance");

			//XmlNodeList list = myXmlDocument.SelectNodes("/a:Dataset/a:DEM/a:coverage/a:boundedBy", nsmgr);
			//XmlNodeList list = myXmlDocument.SelectNodes("/fgd:Dataset/fgd:DEM/gml:coverage", nsmgr);
			//XmlNodeList list = myXmlDocument.SelectNodes("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:boundedBy/gml:Envelope/gml:lowerCorner", nsmgr);
			//XmlNode node = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:boundedBy/gml:Envelope/gml:lowerCorner", nsmgr);
			//string lowerCorner = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:boundedBy/gml:Envelope/gml:lowerCorner", nsmgr).InnerText;
			//string upperCorner = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:boundedBy/gml:Envelope/gml:upperCorner", nsmgr).InnerText;
			//string low = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:gridDomain/gml:Grid/gml:limits/gml:GridEnvelope/gml:low", nsmgr).InnerText;
			//string high = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:gridDomain/gml:Grid/gml:limits/gml:GridEnvelope/gml:high", nsmgr).InnerText;
			//string startPointeList = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:coverageFunction/gml:GridFunction/gml:startPoint", nsmgr).InnerText;
			//string tupleList = myXmlDocument.SelectSingleNode("/fgd:Dataset/fgd:DEM/fgd:coverage/gml:rangeSet/gml:DataBlock/gml:tupleList", nsmgr).InnerText;

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

		//private void DisplayTree(XmlNode node, string tab)
		//{
		//	if (node != null)
		//		Format(node,tab);

		//	if (node.HasChildNodes)
		//	{
		//		node = node.FirstChild;
		//		while (node != null)
		//		{
		//			DisplayTree(node, tab +"\t");
		//			node = node.NextSibling;
		//		}
		//	}
		//}

		//private void Format(XmlNode node, string tab)
		//{
		//	if (!node.HasChildNodes)
		//	{
		//		System.Diagnostics.Debug.WriteLine(tab + node.Name + "<" + node.Value + ">");
		//	}
		//	else
		//	{
		//		System.Diagnostics.Debug.WriteLine(tab + node.Name);
		//		if (XmlNodeType.Element == node.NodeType)
		//		{
		//			XmlNamedNodeMap map = node.Attributes;
		//			foreach (XmlNode attrnode in map)
		//				System.Diagnostics.Debug.WriteLine(tab + "\tatttr:" + attrnode.Name + "<" + attrnode.Value + "> ");
		//		}
		//		//System.Diagnostics.Debug.WriteLine(tab);
		//	}
		//}

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
