﻿using System;

namespace VersionOne.SDK.APIClient
{
	/// <summary>
	/// Provides connectors for a VersionOne instance.
	/// </summary>
	public interface IConnectors
	{
		IAPIConnector MetaConnector { get; }
		IAPIConnector MetaConnectorWithProxy { get; }
		IAPIConnector DataConnector { get; }
		IAPIConnector DataConnectorWithProxy { get; }
		IAPIConnector ConfigurationConnector { get; }
		IAPIConnector ConfigurationConnectorWithProxy { get; }
	}

	public sealed class Connectors : IConnectors
	{

		private readonly IUrls _urls;
		private readonly ICredentials _credentials;
		private readonly OAuth2Client.IStorage _storage;
		private bool _useWindowsIntegratedAuth;

		public Connectors()
		{
			_urls = new Urls();
			_credentials = new Credentials();
			InitializeInternal();
		}

		public Connectors(IUrls urls, ICredentials credentials)
		{
			if (urls == null) throw new ArgumentNullException("urls");
			if (credentials == null) throw new ArgumentNullException("credentials");
			_urls = urls;
			_credentials = credentials;
			InitializeInternal();
		}

		public Connectors(IUrls urls, OAuth2Client.IStorage storage)
		{
			if (urls == null) throw new ArgumentNullException("urls");
			if (storage == null) throw new ArgumentNullException("storage");
			_urls = urls;
			_storage = storage;
			InitializeInternal();
		}

		private void InitializeInternal()
		{
			_useWindowsIntegratedAuth = V1ConfigurationManager.GetValue(Settings.UseWindowsIntegratedAuth, false);
		}

		private ProxyProvider GetProxyProvider()
		{
			var proxyUri = new Uri(_urls.ProxyUrl);
			return new ProxyProvider(proxyUri, _credentials.ProxyUserName, _credentials.ProxyPassword);
		}

		private IAPIConnector MakeConnector(string path, ProxyProvider proxy = null)
		{
			var connector = new VersionOneAPIConnector(path, proxyProvider: proxy);

			if (_storage != null)
				return connector.WithOAuth2(_storage);

			if (_useWindowsIntegratedAuth)
				return connector.WithWindowsIntegratedAuthentication();

			if (!string.IsNullOrWhiteSpace(_credentials.V1UserName))
				return connector
					.WithVersionOneUsernameAndPassword(_credentials.V1UserName, _credentials.V1Password);

			throw new Exception("It was not possible to create a connector with the current authentication configuration.");

		}

		private IAPIConnector _metaConnector;
		public IAPIConnector MetaConnector
		{
			get
			{
				if (_metaConnector != null) return _metaConnector;
				_metaConnector = MakeConnector(_urls.MetaUrl);
				return _metaConnector;
			}
		}

		private IAPIConnector _metaConnectorWithProxy;
		public IAPIConnector MetaConnectorWithProxy
		{
			get
			{
				if (_metaConnectorWithProxy != null) return _metaConnectorWithProxy;
				_metaConnectorWithProxy = MakeConnector(_urls.MetaUrl, GetProxyProvider());
				return _metaConnectorWithProxy;
			}
		}

		private IAPIConnector _dataConnector;
		public IAPIConnector DataConnector
		{
			get
			{
				if (_dataConnector != null) return _dataConnector;
				_dataConnector = MakeConnector(_urls.DataUrl);
				return _dataConnector;
			}
		}

		private IAPIConnector _dataConnectorWithProxy;
		public IAPIConnector DataConnectorWithProxy
		{
			get
			{
				if (_dataConnectorWithProxy != null) return _dataConnectorWithProxy;
				_dataConnectorWithProxy = MakeConnector(_urls.DataUrl, GetProxyProvider());
				return _dataConnectorWithProxy;
			}
		}

		private IAPIConnector _configurationConnector;
		public IAPIConnector ConfigurationConnector
		{
			get
			{
				if (_configurationConnector != null) return _configurationConnector;
				_configurationConnector = MakeConnector(_urls.ConfigUrl);
				return _configurationConnector;
			}
		}

		private IAPIConnector _configurationConnectorWithProxy;
		public IAPIConnector ConfigurationConnectorWithProxy
		{
			get
			{
				if (_configurationConnectorWithProxy != null) return _configurationConnectorWithProxy;
				_configurationConnectorWithProxy = MakeConnector(_urls.ConfigUrl, GetProxyProvider());
				return _configurationConnectorWithProxy;
			}
		}

	}
}
