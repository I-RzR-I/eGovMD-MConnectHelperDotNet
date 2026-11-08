using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using MConnectHelperDotNet.Models.Client;
using Microsoft.Extensions.Options;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class FakeOptionsMonitor<T> : IOptionsMonitor<T>, IDisposable
    {
        private readonly List<Action<T, string>> _listeners = new List<Action<T, string>>();

        public FakeOptionsMonitor(T currentValue)
        {
            CurrentValue = currentValue;
        }

        public T CurrentValue { get; set; }

        public static FakeOptionsMonitor<ClientCertificateOptions> ForClientCertificate(X509Certificate2 certificate = null)
        {
            return new FakeOptionsMonitor<ClientCertificateOptions>(new ClientCertificateOptions { ClientCertificate = certificate });
        }

        public T Get(string name) => CurrentValue;

        public IDisposable OnChange(Action<T, string> listener)
        {
            if (listener != null) _listeners.Add(listener);

            return this;
        }

        public void Raise(T newValue)
        {
            CurrentValue = newValue;
            foreach (var listener in _listeners)
                listener(newValue, string.Empty);
        }

        public void Dispose()
        {
        }
    }
}
