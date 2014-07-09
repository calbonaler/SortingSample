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
using SortingSample.Models;
using SortingSample.ViewModels;

namespace SortingSample.Views
{
	/* 
	 * ViewModelからの変更通知などの各種イベントを受け取る場合は、PropertyChangedWeakEventListenerや
     * CollectionChangedWeakEventListenerを使うと便利です。独自イベントの場合はLivetWeakEventListenerが使用できます。
     * クローズ時などに、LivetCompositeDisposableに格納した各種イベントリスナをDisposeする事でイベントハンドラの開放が容易に行えます。
     *
     * WeakEventListenerなので明示的に開放せずともメモリリークは起こしませんが、できる限り明示的に開放するようにしましょう。
     */

	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		public bool ScreenSaverMode
		{
			get { return controlPanel.Visibility == System.Windows.Visibility.Collapsed; }
			set { controlPanel.Visibility = value ? System.Windows.Visibility.Collapsed : System.Windows.Visibility.Visible; }
		}

		public async Task Start(SortingMethod method, SortTargetKind target)
		{
			var vm = (MainWindowViewModel)DataContext;
			vm.Method = method;
			vm.TargetKind = target;
			await vm.Sort();
		}
	}
}
