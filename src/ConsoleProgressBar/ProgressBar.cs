using System;
using System.Text;
using System.Threading;

namespace ConsoleProgressBar
{
    public class ProgressBar : IDisposable
    {
        private readonly int _total;
        private readonly int _indent;
        private readonly int _blockCount;
        private readonly TimeSpan _animationInterval = TimeSpan.FromMilliseconds(200);
        private readonly Timer _timer;
        private bool _disposed = false;
        private int _current;

        public ProgressBar(int total = 100)
        {
            if (total <= 0) return;

            // If the console output is redirected to a file, draw nothing.
            if (Console.IsOutputRedirected) return;

            _total = total;
            _current = 0;

            _indent = (int)(Console.WindowWidth * 0.3);
            _blockCount = Console.WindowWidth - _indent - 7;

            _timer = new Timer(TimerHandler);
            ResetTimer();
        }

        private void ResetTimer() => _timer.Change(_animationInterval, Timeout.InfiniteTimeSpan);

        public void Increment() => Interlocked.Increment(ref _current);

        private void TimerHandler(object state)
        {
            lock (_timer)
            {
                if (_disposed) return;
                Render();
                ResetTimer();
            }
        }

        private void Render()
        {
            var percent = (double)_current / _total;
            int progressBlockCount = (int)(percent * _blockCount);

            var bar = string.Format(
                "[{0}{1}] {2,3}%",
                new string('#', progressBlockCount),
                new string('-', _blockCount - progressBlockCount),
                (int)(percent * 100));

            Console.Write($"{_current}/{_total}".PadRight(_indent) + bar + "\r");
        }

        public void Dispose()
        {
            if (_disposed) return;
            lock (_timer)
            {
                _disposed = true;
                Render();
                Console.WriteLine();
            }
        }
    }
}