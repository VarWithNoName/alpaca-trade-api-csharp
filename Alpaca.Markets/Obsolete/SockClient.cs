﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Alpaca.Markets
{
    /// <summary>
    /// Provides unified type-safe access for Alpaca streaming API.
    /// </summary>
    [SuppressMessage(
        "Globalization","CA1303:Do not pass literals as localized parameters",
        Justification = "We do not plan to support localized exception messages in this SDK.")]
    [Obsolete("This class is deprecated and will be removed in the upcoming releases. Use the AlpacaStreamingClient class instead.", false)]
    public sealed class SockClient : IDisposable
    {
        private readonly AlpacaStreamingClient _client;

        /// <summary>
        /// Creates new instance of <see cref="SockClient"/> object.
        /// </summary>
        /// <param name="keyId">Application key identifier.</param>
        /// <param name="secretKey">Application secret key.</param>
        /// <param name="alpacaRestApi">Alpaca REST API endpoint URL.</param>
        /// <param name="webSocketFactory">Factory class for web socket wrapper creation.</param>
        public SockClient(
            String keyId,
            String secretKey,
            String? alpacaRestApi = null,
            IWebSocketFactory? webSocketFactory = null)
            : this(createConfiguration(
                keyId, secretKey, alpacaRestApi.GetUrlSafe(Environments.Live.AlpacaTradingApi), webSocketFactory))
        {
        }

        /// <summary>
        /// Creates new instance of <see cref="SockClient"/> object.
        /// </summary>
        /// <param name="keyId">Application key identifier.</param>
        /// <param name="secretKey">Application secret key.</param>
        /// <param name="alpacaRestApi">Alpaca REST API endpoint URL.</param>
        /// <param name="webSocketFactory">Factory class for web socket wrapper creation.</param>
        public SockClient(
            String keyId,
            String secretKey,
            Uri alpacaRestApi,
            IWebSocketFactory? webSocketFactory)
            : this(createConfiguration(keyId, secretKey, alpacaRestApi, webSocketFactory))
        {
        }

#if NETSTANDARD2_0 || NETSTANDARD2_1
        /// <summary>
        /// Creates new instance of <see cref="SockClient"/> object.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="webSocketFactory">Factory class for web socket wrapper creation.</param>
        public SockClient(
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IWebSocketFactory? webSocketFactory = null)
            : this(createConfiguration(configuration, webSocketFactory)) =>
            System.Diagnostics.Contracts.Contract.Requires(configuration != null);

        private static AlpacaStreamingClientConfiguration createConfiguration(
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            IWebSocketFactory? webSocketFactory = null) =>
            createConfiguration(
                configuration?["keyId"] ?? throw new ArgumentException("Provide 'keyId' configuration parameter.", nameof(configuration)),
                configuration["secretKey"] ?? throw new ArgumentException("Provide 'secretKey' configuration parameter.", nameof(configuration)),
                configuration["alpacaRestApi"].GetUrlSafe(Environments.Live.AlpacaTradingApi),
                webSocketFactory);
#endif

        /// <summary>
        /// Creates new instance of <see cref="SockClient"/> object.
        /// </summary>
        /// <param name="configuration">Configuration parameters object.</param>
        private SockClient(
            AlpacaStreamingClientConfiguration configuration) =>
            _client = new AlpacaStreamingClient(configuration);


        /// <summary>
        /// Occured when new account update received from stream.
        /// </summary>
        [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Compiler issue")]
        public event Action<IAccountUpdate>? OnAccountUpdate
        {
            add => _client.OnAccountUpdate += value;
            remove => _client.OnAccountUpdate -= value;
        }

        /// <summary>
        /// Occured when new trade update received from stream.
        /// </summary>
        [SuppressMessage("Design", "CA1030:Use events where appropriate", Justification = "Compiler issue")]
        public event Action<ITradeUpdate>? OnTradeUpdate
        {
            add => _client.OnTradeUpdate += value;
            remove => _client.OnTradeUpdate -= value;
        }
        
        /// <inheritdoc/>
        public void Dispose() => _client.Dispose();

        private static AlpacaStreamingClientConfiguration createConfiguration(
            String keyId,
            String secretKey,
            Uri alpacaRestApi,
            IWebSocketFactory? webSocketFactory) =>
            new AlpacaStreamingClientConfiguration
            {
                KeyId = keyId ?? throw new ArgumentException("Application key id should not be null.", nameof(keyId)),
                SecretKey = secretKey ?? throw new ArgumentException("Application secret key should not be null.", nameof(secretKey)),
                ApiEndpoint = alpacaRestApi ?? Environments.Live.AlpacaTradingApi,
                WebSocketFactory = webSocketFactory ?? WebSocket4NetFactory.Instance,
            };
    }
}
