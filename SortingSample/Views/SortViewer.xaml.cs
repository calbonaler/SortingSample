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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SortingSample.Models;

namespace SortingSample.Views
{
	/// <summary>
	/// SortViewer.xaml の相互作用ロジック
	/// </summary>
	public partial class SortViewer : UserControl
	{
		public SortViewer()
		{
			InitializeComponent();
		}

		public static readonly DependencyProperty ReporterProperty = DependencyProperty.Register("Reporter", typeof(SortReporter), typeof(SortViewer), new PropertyMetadata(null, OnReporterChanged));

		static void OnReporterChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			var viewer = (SortViewer)sender;
			var newV = (SortReporter)e.NewValue;
			var oldV = (SortReporter)e.OldValue;
			if (oldV != null)
			{
				oldV.SetInitialized(null);
				oldV.SetComparisonReported(null);
				oldV.SetSwapReported(null);
				oldV.SetMarked(null);
				oldV.SetUnmarked(null);
			}
			if (newV != null)
			{
				newV.SetInitialized(viewer.OnInitialized);
				newV.SetComparisonReported(viewer.OnComparisonReported);
				newV.SetSwapReported(viewer.OnSwapReported);
				newV.SetMarked(viewer.OnMarked);
				newV.SetUnmarked(viewer.OnUnmarked);
				viewer.OnInitialized();
			}
		}

		public SortReporter Reporter
		{
			get { return (SortReporter)GetValue(ReporterProperty); }
			set { SetValue(ReporterProperty, value); }
		}

		Rectangle[] rects;
		
		void OnInitialized()
		{
			if (rects != null)
			{
				foreach (var rect in rects)
					drawingCanvas.Children.Remove(rect);
			}
			if (Reporter.SortTarget != null)
			{
				rects = new Rectangle[Reporter.SortTarget.Count];
				int max = 0, min = 0;
				foreach (var item in Reporter.SortTarget)
				{
					if (item > max)
						max = item;
					if (item < min)
						min = item;
				}
				const int spacing = 3;
				for (int i = 0; i < rects.Length; i++)
				{
					rects[i] = new Rectangle() { Fill = Brushes.Black, Stroke = null, StrokeThickness = 5 };
					drawingCanvas.Children.Add(rects[i]);
					rects[i].Width = ActualWidth / rects.Length - spacing;
					rects[i].Height = ActualHeight * Math.Abs(Reporter.SortTarget[i]) / (max - min);
					Canvas.SetLeft(rects[i], i * ActualWidth / rects.Length);
					Canvas.SetTop(rects[i], ActualHeight * (max - Math.Max(Reporter.SortTarget[i], 0)) / (max - min));
				}
				sortingMethod.Text = Reporter.SortingMethod.ToString();
			}
		}

		async Task OnComparisonReported(int index1, int index2)
		{
			rects[index1].Stroke = Brushes.Red;
			rects[index2].Stroke = Brushes.Red;
			await Task.Delay(10);
			rects[index1].Stroke = null;
			rects[index2].Stroke = null;
		}

		async Task OnSwapReported(int index1, int index2)
		{
			var rect1Left = Canvas.GetLeft(rects[index1]);
			var rect2Left = Canvas.GetLeft(rects[index2]);
			Canvas.SetLeft(rects[index1], rect2Left);
			Canvas.SetLeft(rects[index2], rect1Left);
			await Task.Delay(10);
			var tmp = rects[index1];
			rects[index1] = rects[index2];
			rects[index2] = tmp;
		}

		void OnMarked(int index) { rects[index].Fill = Brushes.Blue; }

		void OnUnmarked(int index) { rects[index].Fill = Brushes.Black; }
	}
}
