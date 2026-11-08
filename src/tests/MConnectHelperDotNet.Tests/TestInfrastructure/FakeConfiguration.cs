using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class FakeConfiguration : IConfiguration, IConfigurationSection
    {
        public const string ThrowMarker = "MARKER-CFG-7F3A";

        private readonly Dictionary<string, string> _data;
        private readonly FakeConfiguration _root;
        private readonly string _path;

        public FakeConfiguration() : this(null)
        {
        }

        public FakeConfiguration(IDictionary<string, string> values)
        {
            _data = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            _root = this;
            _path = string.Empty;
            if (values == null) return;
            foreach (var pair in values)
                _data[pair.Key] = pair.Value;
        }

        private FakeConfiguration(FakeConfiguration root, string path)
        {
            _data = root._data;
            _root = root;
            _path = path;
        }

        public bool ThrowOnRead { get; set; }

        public string Key => _path.Substring(_path.LastIndexOf(':') + 1);

        public string Path => _path;

        public string Value
        {
            get => _path.Length == 0 ? null : (_data.TryGetValue(_path, out var value) ? value : null);
            set => _data[_path] = value;
        }

        public string this[string key]
        {
            get
            {
                ThrowIfRequested();
                return _data.TryGetValue(Combine(_path, key), out var value) ? value : null;
            }
            set => _data[Combine(_path, key)] = value;
        }

        public FakeConfiguration Set(string key, string value)
        {
            _data[Combine(_path, key)] = value;

            return this;
        }

        public IConfigurationSection GetSection(string key)
        {
            ThrowIfRequested();

            return new FakeConfiguration(_root, Combine(_path, key));
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            var prefix = _path.Length == 0 ? string.Empty : _path + ":";
            var names = new List<string>();
            foreach (var key in _data.Keys)
            {
                if (!key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)) continue;

                var rest = key.Substring(prefix.Length);
                var separator = rest.IndexOf(':');
                var name = separator < 0 ? rest : rest.Substring(0, separator);
                if (name.Length > 0 && !names.Contains(name, StringComparer.OrdinalIgnoreCase))
                    names.Add(name);
            }

            return names.Select(name => (IConfigurationSection)new FakeConfiguration(_root, prefix + name)).ToList();
        }

        public IChangeToken GetReloadToken() => new CancellationChangeToken(CancellationToken.None);

        private static string Combine(string path, string key) => path.Length == 0 ? key : path + ":" + key;

        private void ThrowIfRequested()
        {
            if (_root.ThrowOnRead)
                throw new InvalidOperationException(ThrowMarker + " configuration read failed");
        }
    }
}
