using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GML2PNG
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		FGDHolder fgdHolder;

		public MainWindow()
		{
			InitializeComponent();
			fgdHolder = new FGDHolder();
		}

		private void LoadXmlFile(string filename)
		{
			fgdHolder.Add(filename);
		}

		private void Window_DragEnter(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.All;
		}

		private void Window_Drop(object sender, DragEventArgs e)
		{
			if (!e.Data.GetDataPresent(DataFormats.FileDrop))
				return;
			foreach (var filename in (string[])e.Data.GetData(DataFormats.FileDrop))
			{
				LoadXmlFile(filename);
			}
		}

		private void exportButton_Click(object sender, RoutedEventArgs e)
		{
			fgdHolder.Export();
		}

		private void clearButton_Click(object sender, RoutedEventArgs e)
		{
			fgdHolder.Clear();
		}
	}
}
