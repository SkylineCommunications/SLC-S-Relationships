namespace Skyline.DataMiner.Solutions.Relationships.GQI.Shared
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Skyline.DataMiner.Analytics.GenericInterface;

	public class GQIPageEnumerator : IDisposable
	{
		private readonly IEnumerable<GQIRow>[] _sources;
		private IEnumerator<GQIRow> _currentEnumerator;
		private int _currentSourceIndex;

		private bool _hasNext;
		private GQIRow _nextRow;

		private bool _isDisposed;

		public GQIPageEnumerator(params IEnumerable<GQIRow>[] rows)
		{
			if (rows == null)
			{
				throw new ArgumentNullException(nameof(rows));
			}

			_sources = rows;
			_currentSourceIndex = 0;
			_currentEnumerator = _sources.Length > 0
				? _sources[0].GetEnumerator()
				: Enumerable.Empty<GQIRow>().GetEnumerator();

			TryMoveNext();
		}

		public GQIPage GetNextPage(int pageSize)
		{
			var page = new List<GQIRow>(pageSize);

			for (int i = 0; i < pageSize && _hasNext; i++)
			{
				page.Add(_nextRow);
				TryMoveNext();
			}

			if (!_hasNext)
			{
				Dispose();
			}

			return new GQIPage(page.ToArray())
			{
				HasNextPage = _hasNext,
			};
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_isDisposed)
			{
				return;
			}

			if (disposing)
			{
				_currentEnumerator?.Dispose();
			}

			_isDisposed = true;
		}

		private void TryMoveNext()
		{
			while (true)
			{
				if (_currentEnumerator.MoveNext())
				{
					_hasNext = true;
					_nextRow = _currentEnumerator.Current;
					return;
				}

				// Current enumerator exhausted, dispose and try next source
				_currentEnumerator.Dispose();
				_currentSourceIndex++;

				if (_currentSourceIndex >= _sources.Length)
				{
					// No more sources
					_hasNext = false;
					_nextRow = default;
					return;
				}

				_currentEnumerator = _sources[_currentSourceIndex].GetEnumerator();
			}
		}
	}
}
