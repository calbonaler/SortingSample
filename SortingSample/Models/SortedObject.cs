using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingSample.Models
{
	public class SortedObject
	{
		SortedObject(SortReporter reporter) { this.reporter = reporter; }

		public SortedObject(SortReporter reporter, int min, int max, int length) : this(reporter) { Initialize(min, max, length); }

		public SortedObject(SortReporter reporter, IEnumerable<int> values) : this(reporter) { Initialize(values); }

		int[] sortedArray;
		Random generator;
		SortReporter reporter;
		List<ValueTracker> trackers = new List<ValueTracker>();

		public void Initialize(int min, int max, int length)
		{
			if (generator == null)
				generator = new Random();
			if (sortedArray == null || sortedArray.Length != length)
			{
				sortedArray = new int[length];
				if (reporter != null)
					reporter.SortTarget = new ReadOnlyCollection<int>(sortedArray);
			}
			for (int i = 0; i < length; i++)
				sortedArray[i] = generator.Next(min, max + 1);
			if (reporter != null)
				reporter.Initialize();
			trackers.Clear();
		}

		public void Initialize(IEnumerable<int> values)
		{
			sortedArray = values.ToArray();
			if (reporter != null)
			{
				reporter.SortTarget = new ReadOnlyCollection<int>(sortedArray);
				reporter.Initialize();
			}
			trackers.Clear();
		}

		public async Task Swap(int index1, int index2)
		{
			int tmp = sortedArray[index1];
			sortedArray[index1] = sortedArray[index2];
			sortedArray[index2] = tmp;
			if (reporter != null)
				await reporter.ReportSwap(index1, index2);
			foreach (var tracker in trackers)
				tracker.AffectSwap(index1, index2);
		}

		public async Task<int> Compare(int index1, int index2)
		{
			var res = sortedArray[index2].CompareTo(sortedArray[index1]);
			if (reporter != null)
				await reporter.ReportComparison(index1, index2);
			return res;
		}

		public ValueTracker MarkIndex(int index)
		{
			var tracker = new ValueTracker(index);
			trackers.Add(tracker);
			if (reporter != null)
				reporter.ReportMark(index);
			return tracker;
		}

		public void UnmarkIndex(ValueTracker tracker)
		{
			trackers.Remove(tracker);
			if (reporter != null)
				reporter.ReportUnmark(tracker.TargetIndex);
		}

		public int Count { get { return sortedArray.Length; } }
	}

	public class ValueTracker
	{
		public ValueTracker(int index) { TargetIndex = index; }

		public int TargetIndex { get; private set; }

		public void AffectSwap(int index1, int index2)
		{
			if (TargetIndex == index1)
				TargetIndex = index2;
			else if (TargetIndex == index2)
				TargetIndex = index1;
		}
	}
}
