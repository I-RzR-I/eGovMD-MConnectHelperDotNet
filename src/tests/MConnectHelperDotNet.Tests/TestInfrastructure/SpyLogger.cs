using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class SpyLogger<T> : ILogger<T>
    {
        private readonly object _gate = new object();
        private readonly List<(LogLevel Level, Exception Exception, string Message)> _entries =
            new List<(LogLevel Level, Exception Exception, string Message)>();

        public IReadOnlyList<(LogLevel Level, Exception Exception, string Message)> Entries
        {
            get
            {
                lock (_gate)
                    return _entries.ToArray();
            }
        }

        public int ErrorCount
        {
            get
            {
                lock (_gate)
                    return _entries.FindAll(entry => entry.Level == LogLevel.Error).Count;
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            var message = formatter != null ? formatter(state, exception) : state?.ToString();
            lock (_gate)
                _entries.Add((logLevel, exception, message));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => NullLogger.Instance.BeginScope(state);
    }
}
