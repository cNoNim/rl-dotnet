using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Threading;
using RL.Core;

namespace RL.Progress;

public struct ProgressGenerator<TG, T>(TG generator, string? title = null, CancellationToken token = default) :
    IGenerator<ProgressGenerator<TG, T>, T>
    where TG : IGenerator<T>
{
    private StringBuilder? _sb;
    private Stopwatch? _sw;
    private int _lastLength;
    private TimeSpan? _lastElapsed;

    public T this[int index]
    {
        get
        {
            if (token.IsCancellationRequested)
            {
                Console.WriteLine();
                token.ThrowIfCancellationRequested();
            }

            var result = generator[index];
            if (Console.IsOutputRedirected)
                return result;

            if (index == 0)
            {
                _lastLength = 0;
                _lastElapsed = null;
                if (title != null)
                    Console.Write($"{title}: ");
            }

            if (index == Count - 1)
            {
                _sb ??= new StringBuilder();
                _sb.Clear();
                _sb.Append('\b', _lastLength);
                _sb.Append($"100% {Count,10}/{Count}");
                var elapsed = _sw?.Elapsed;
                if (elapsed.HasValue)
                    _sb.Append($@" {elapsed:%m\:ss\.ffff} {Count / elapsed.Value.TotalSeconds,6:#.00}it/s");
                var currentLength = _sb.Length - _lastLength;
                var overlap = _lastLength - currentLength;
                if (overlap > 0)
                {
                    _sb.Append(' ', overlap);
                    _sb.Append('\b', overlap);
                }

                _sb.AppendLine();
                Console.Write(_sb);
                _sw?.Stop();
                _sb = null;
                _sw = null;
                return result;
            }
            else
            {
                var period = (Count + 99) / 100;
                if (index % period != 0)
                    return result;
                _sb ??= new StringBuilder();
                _sw ??= new Stopwatch();
                _sb.Clear();
                var progress = index * 100 / (double)Count;
                _sb.Append('\b', _lastLength);
                _sb.Append($"{progress,3:#}% {index,10}/{Count}");
                if (!_lastElapsed.HasValue)
                {
                    _sw.Start();
                    _lastElapsed = _sw.Elapsed;
                }
                else
                {
                    var elapsed = _sw.Elapsed;
                    _sb.Append(
                        $@" {elapsed:%m\:ss\.ffff} {period / (elapsed - _lastElapsed.Value).TotalSeconds,6:#.00}it/s");
                    _lastElapsed = elapsed;
                }

                var currentLength = _sb.Length - _lastLength;
                var overlap = _lastLength - currentLength;
                if (overlap > 0)
                {
                    _sb.Append(' ', overlap);
                    _sb.Append('\b', overlap);
                }

                _lastLength = currentLength;
                Console.Write(_sb);
                return result;
            }
        }
    }

    public int Count { get; } = generator.Count;

    public GeneratorEnumerator<ProgressGenerator<TG, T>, T> GetEnumerator() => new(this);

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsFinite => true;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool TryGetNext(int current, out int next) =>
        generator.TryGetNext(current, out next);
}