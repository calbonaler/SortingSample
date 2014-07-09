using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingSample.Models
{
	public static class Sorter
	{
		static async Task<int> BubbleUp(SortedObject obj, int gap, int endOffset)
		{
			int swaps = 0;
			for (int i = 0; i + gap < obj.Count - endOffset; i++)
			{
				if (await obj.Compare(i, i + gap) > 0)
				{
					await obj.Swap(i, i + gap);
					swaps++;
				}
			}
			return swaps;
		}

		public static async Task BubbleSort(SortedObject obj)
		{
			for (int i = 0; i < obj.Count - 1; i++)
				await BubbleUp(obj, 1, i);
		}

		public static async Task CocktailSort(SortedObject obj)
		{
			var begin = 0;
			var end = obj.Count - 1;
			while (true)
			{
				/* 順方向のスキャン */
				var lastSwapIndex = begin;
				for (int i = begin; i < end; i++)
				{
					if (await obj.Compare(i, i + 1) > 0)
					{
						await obj.Swap(i, i + 1);
						lastSwapIndex = i;
					}
				}
				end = lastSwapIndex; /* 後方のスキャン範囲を狭める */
				if (begin == end)
					break;
				/* 逆方向のスキャン */
				lastSwapIndex = end;
				for (int i = end; i > begin; i--)
				{
					if (await obj.Compare(i, i - 1) < 0)
					{
						await obj.Swap(i, i - 1);
						lastSwapIndex = i;
					}
				}
				begin = lastSwapIndex; /* 前方のスキャン範囲を狭める */
				if (begin == end)
					break;
			}
		}

		public static async Task CombSort(SortedObject obj)
		{
			int h = obj.Count * 10 / 13;
			while (true)
			{
				int swaps = 0;
				for (int i = 0; i + h < obj.Count; ++i)
				{
					if (await obj.Compare(i, i + h) > 0)
					{
						await obj.Swap(i, i + h);
						++swaps;
					}
				}
				if (h == 1)
				{
					if (swaps == 0)
						break;
				}
				else
					h = h * 10 / 13;
			}
		}

		public static async Task GnomeSort(SortedObject obj)
		{
			for (int i = 1; i < obj.Count; )
			{
				if (await obj.Compare(i - 1, i) <= 0) // 降順にソートする場合は >= で比較する
					i++; // 正しく並んでいるので何もせずに次に進む
				else
				{
					await obj.Swap(i - 1, i); // 順序が間違っているので交換
					if (i != 1)
						i--; // 交換したので前に戻る
					else
						i++; // 先頭なので戻るのはやめ、次に進む
				}
			}
		}

		public static async Task SelectionSort(SortedObject obj)
		{
			for (int i = 0; i < obj.Count; i++)
			{
				int min = i;
				for (int j = i + 1; j < obj.Count; j++)
				{
					if (await obj.Compare(j, min) < 0)
						min = j;
				}
				await obj.Swap(i, min);
			}
		}

		static async Task InsertionSort(SortedObject obj, int gap)
		{
			for (int i = gap; i < obj.Count; i++)
			{
				for (int j = i; j >= gap && await obj.Compare(j - gap, j) > 0; j -= gap)
					await obj.Swap(j, j - gap);
			}
		}

		public static async Task InsertionSort(SortedObject obj) { await InsertionSort(obj, 1); }

		static readonly int[] shellSortGaps = new int[] { 701, 301, 132, 57, 23, 10, 4, 1 };

		public static async Task ShellSort(SortedObject obj)
		{
			foreach (var gap in shellSortGaps)
				await InsertionSort(obj, gap);
		}

		public static async Task HeapSort(SortedObject obj)
		{
			int i = 0;
			// arr の先頭から順に、ヒープを成長させる
			//  0    1    2  | 3    4    5
			// [  ] [  ] [  ]|[  ] [  ] [  ]
			//    ヒープ     |   未処理の入力
			//              ===>
			// i は、ヒープ中の要素数であると同時に、未処理データの先頭を指してもいる
			// 配列が全部ヒープに入れ替わるまで繰り返す
			while (++i < obj.Count)
			{
				// 配列の先頭要素を、ヒープの最後に移動するわけだが、どちらもちょうど同じ場所に最初からあるので、境界を移動させるだけでよい
				// arr[i] に、ヒープに新しく追加されたデータがあるものとして、先頭から arr[i] までがヒープになるよう再構成する
				await UpHeap(obj, i);
			}
			// arr の末端から順に、ヒープから取り出して並べる
			//  0    1    2  | 3    4    5
			// [  ] [  ] [  ]|[  ] [  ] [  ]
			//    ヒープ     |   ソート済みの配列
			//             <===
			// ヒープが全部配列に入れ替わるまで繰り返す
			while (--i > 0)
			{
				// ヒープの先頭要素を、配列に移動すると同時に、ヒープの最後の要素を、ヒープの先頭に移動する swap
				await obj.Swap(0, i);
				// arr[0] に、ヒープの最後から移動されたデータがあるものとして、先頭から arr[i - 1] までがヒープになるよう再構成する
				await DownHeap(obj, i);
			}
		}

		// arr[n] に、ヒープに新しく追加されたデータがあるものとして、先頭から arr[n] までがヒープになるよう再構成する
		static async Task UpHeap(SortedObject obj, int n)
		{
			while (n > 0)
			{
				var m = (n + 1) / 2 - 1; // get parent index
				if (await obj.Compare(m, n) < 0)
					await obj.Swap(m, n);
				else
					break;
				n = m;
			}
		}

		// arr[0] に、ヒープの最後から移動されたデータがあるものとして、先頭から arr[n - 1] までがヒープになるよう再構成する
		static async Task DownHeap(SortedObject obj, int n)
		{
			int m = 0;
			int tmp = 0;
			for (; ; )
			{
				var leftChild = (m + 1) * 2 - 1; // get left child index
				var rightChild = (m + 1) * 2; // get right child index
				if (leftChild >= n)
					break;
				if (await obj.Compare(leftChild, tmp) > 0)
					tmp = leftChild;
				if (rightChild < n && await obj.Compare(rightChild, tmp) > 0)
					tmp = rightChild;
				if (tmp == m)
					break;
				await obj.Swap(tmp, m);
				m = tmp;
			}
		}

		static async Task<ValueTracker> Median(SortedObject obj, int index1, int index2, int index3)
		{
			if (await obj.Compare(index1, index2) < 0)
				if (await obj.Compare(index2, index3) < 0)
					return obj.MarkIndex(index2);
				else if (await obj.Compare(index3, index1) < 0)
					return obj.MarkIndex(index1);
				else
					return obj.MarkIndex(index3);
			else
				if (await obj.Compare(index3, index2) < 0)
					return obj.MarkIndex(index2);
				else if (await obj.Compare(index1, index3) < 0)
					return obj.MarkIndex(index1);
				else
					return obj.MarkIndex(index3);
		}

		static async Task QuickSort(SortedObject obj, int left, int right)
		{
			if (left < right)
			{
				var pivot = await Median(obj, left, left + (right - left) / 2, right);
				int i = left, j = right;
				while (true)
				{
					// a[] を pivot 以上と以下の集まりに分割する
					while (await obj.Compare(i, pivot.TargetIndex) < 0)
						i++; // a[i] >= pivot となる位置を検索
					while (await obj.Compare(pivot.TargetIndex, j) < 0)
						j--; // a[j] <= pivot となる位置を検索
					if (i >= j)
						break;
					await obj.Swap(i++, j--);
				}
				obj.UnmarkIndex(pivot);
				await QuickSort(obj, left, i - 1);  // 分割した左を再帰的にソート
				await QuickSort(obj, j + 1, right); // 分割した右を再帰的にソート
			}
		}

		public static async Task QuickSort(SortedObject obj) { await QuickSort(obj, 0, obj.Count - 1); }

		public static async Task OddEvenSort(SortedObject obj)
		{
			bool flag;
			do
			{
				flag = false;
				for (int i = 0; i < obj.Count - 1; i += 2)
				{ /* Pair1 */
					if (await obj.Compare(i, i + 1) > 0)
					{
						await obj.Swap(i, i + 1);
						flag = true;
					}
				}
				for (int i = 1; i < obj.Count - 1; i += 2)
				{ /* Pair2 */
					if (await obj.Compare(i, i + 1) > 0)
					{
						await obj.Swap(i, i + 1);
						flag = true;
					}
				}
			} while (flag);
		}

		public static async Task QuickInsertionSort(SortedObject obj)
		{
			int l = 0, r = obj.Count - 1;
			var pivot = await Median(obj, l, l + (r - l) / 2, r);
			while (true)
			{
				// a[] を pivot 以上と以下の集まりに分割する
				while (await obj.Compare(l, pivot.TargetIndex) < 0)
					l++; // a[i] >= pivot となる位置を検索
				while (await obj.Compare(pivot.TargetIndex, r) < 0)
					r--; // a[j] <= pivot となる位置を検索
				if (l >= r)
					break;
				await obj.Swap(l++, r--);
			}
			obj.UnmarkIndex(pivot);
			for (int i = 1; i < obj.Count; i++)
			{
				for (int j = i; j >= 1 && await obj.Compare(j - 1, j) > 0; j -= 1)
					await obj.Swap(j, j - 1);
			}
		}

		public static async Task QuickInsertion2Sort(SortedObject obj) { await QuickInsertion2Sort(obj, 0, obj.Count - 1); }

		static async Task QuickInsertion2Sort(SortedObject obj, int left, int right)
		{
			if (right - left < 36)
			{
				for (int i = left + 1; i <= right; i++)
				{
					for (int j = i; j >= left + 1 && await obj.Compare(j - 1, j) > 0; j -= 1)
						await obj.Swap(j, j - 1);
				}
				return;
			}
			if (left < right)
			{
				int i = left, j = right;
				var pivot = await Median(obj, i, i + (j - i) / 2, j);
				while (true)
				{
					// a[] を pivot 以上と以下の集まりに分割する
					while (await obj.Compare(i, pivot.TargetIndex) < 0)
						i++; // a[i] >= pivot となる位置を検索
					while (await obj.Compare(pivot.TargetIndex, j) < 0)
						j--; // a[j] <= pivot となる位置を検索
					if (i >= j)
						break;
					await obj.Swap(i++, j--);
				}
				obj.UnmarkIndex(pivot);
				await QuickInsertion2Sort(obj, left, i - 1);  // 分割した左を再帰的にソート
				await QuickInsertion2Sort(obj, j + 1, right); // 分割した右を再帰的にソート
			}
		}
	}
}
