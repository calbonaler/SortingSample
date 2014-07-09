using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingSample.Models
{
	public class SortReporter
	{
		public ReadOnlyCollection<int> SortTarget { get; set; }

		Action onInitialized;
		Func<int, int, Task> onComparisonReported;
		Func<int, int, Task> onSwapReported;
		Action<int> onMarked;
		Action<int> onUnmarked;

		public void SetInitialized(Action initialized) { onInitialized = initialized; }

		public void SetComparisonReported(Func<int, int, Task> comparisonReported) { onComparisonReported = comparisonReported; }

		public void SetSwapReported(Func<int, int, Task> swapReported) { onSwapReported = swapReported; }

		public void SetMarked(Action<int> marked) { onMarked = marked; }

		public void SetUnmarked(Action<int> unmarked) { onUnmarked = unmarked; }

		public SortingMethod SortingMethod { get; set; }

		public void Initialize()
		{
			if (onInitialized != null)
				onInitialized();
		}

		public async Task ReportComparison(int index1, int index2)
		{
			if (onComparisonReported != null)
				await onComparisonReported(index1, index2);
		}

		public async Task ReportSwap(int index1, int index2)
		{
			if (onSwapReported != null)
				await onSwapReported(index1, index2);
		}

		public void ReportMark(int index)
		{
			if (onMarked != null)
				onMarked(index);
		}

		public void ReportUnmark(int index)
		{
			if (onUnmarked != null)
				onUnmarked(index);
		}
	}
}
