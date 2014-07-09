using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using SortingSample.Models;

namespace SortScreenSaver
{
	/// <summary>
	/// App.xaml の相互作用ロジック
	/// </summary>
	public partial class App : Application
	{
		void Application_Startup(object sender, StartupEventArgs e)
		{
			if (e.Args.Length > 0)
			{
				string mode = e.Args[0].ToLower(CultureInfo.InvariantCulture);
				if (mode.StartsWith("/c"))
				{
					return;
				}
				else if (mode.StartsWith("/p"))
				{
					ShowPreview();
					return;
				}
			}
			ShowScreenSaver();
		}

		async void ShowScreenSaver()
		{
			foreach (var screen in System.Windows.Forms.Screen.AllScreens)
			{
				var screenSaver = new SortingSample.Views.MainWindow();
				screenSaver.Cursor = System.Windows.Input.Cursors.None;
				screenSaver.ScreenSaverMode = true;
				screenSaver.WindowStyle = WindowStyle.None;
				screenSaver.Topmost = true;
				screenSaver.Left = screen.Bounds.Left;
				screenSaver.Top = screen.Bounds.Top;
				screenSaver.Show();
				screenSaver.WindowState = WindowState.Maximized;
				screenSaver.MouseMove += screenSaver_MouseMove;
				Random random = new Random();
				SortingMethod[] methods = (SortingMethod[])Enum.GetValues(typeof(SortingMethod));
				SortTargetKind[] targetKinds = (SortTargetKind[])Enum.GetValues(typeof(SortTargetKind));
				while (true)
					await screenSaver.Start(methods[random.Next(methods.Length)], targetKinds[random.Next(targetKinds.Length)]);
			}
		}

		Point? lastMousePosition;

		void screenSaver_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var currentMousePosition = e.MouseDevice.GetPosition(null);
			if (lastMousePosition != null)
			{
				if (Math.Abs(lastMousePosition.Value.X - currentMousePosition.X) > 0 ||
					Math.Abs(lastMousePosition.Value.Y - currentMousePosition.Y) > 0)
					Application.Current.Shutdown();
			}
			else
				lastMousePosition = currentMousePosition;
		}

		void ShowPreview()
		{
			Application.Current.Shutdown();
		}
	}
}
