using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Livet;

namespace SortingSample.Models
{
	public class SortDrawer : NotificationObject
	{
		public SortDrawer() { Reporter = new SortReporter(); }

		#region Method変更通知プロパティ
		SortingMethod _Method;

		public SortingMethod Method
		{
			get { return _Method; }
			set
			{
				if (_Method == value)
					return;
				_Method = value;
				RaisePropertyChanged();
			}
		}
		#endregion

		public SortReporter Reporter { get; private set; }

		public async Task Sort()
		{
			var method = typeof(Sorter).GetMethod(Enum.GetName(typeof(SortingMethod), Method) + "Sort");
			var obj = new SortedObject(Reporter, -100, 100, 100);
			await (Task)method.Invoke(null, new object[] { obj });
		}
	}

	public enum SortingMethod
	{
		Bubble,
		Cocktail,
		Comb,
		Gnome,
		Selection,
		Insertion,
		Shell,
		Heap,
		Quick,
		OddEven,
	}
}
