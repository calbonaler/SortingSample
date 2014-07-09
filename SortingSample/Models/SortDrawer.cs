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

		static readonly Random random = new Random();

		public SortReporter Reporter { get; private set; }

		public async Task Sort(SortTargetKind kind)
		{
			Reporter.SortingMethod = Method;
			var method = typeof(Sorter).GetMethod(Enum.GetName(typeof(SortingMethod), Method) + "Sort");
			SortedObject obj;
			if (kind == SortTargetKind.Random)
				obj = new SortedObject(Reporter, Enumerable.Range(1, 50).OrderBy(x => random.Next()));
			else if (kind == SortTargetKind.Ascending)
				obj = new SortedObject(Reporter, Enumerable.Range(1, 50));
			else
				obj = new SortedObject(Reporter, Enumerable.Range(1, 50).Reverse());
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

	public enum SortTargetKind
	{
		Random,
		Ascending,
		Descending,
	}
}
