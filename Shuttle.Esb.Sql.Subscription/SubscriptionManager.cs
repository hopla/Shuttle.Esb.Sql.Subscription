using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using Shuttle.Core.Data;
using Shuttle.Core.Infrastructure;

namespace Shuttle.Esb.Sql.Subscription
{
    public class SubscriptionManager : ISubscriptionManager
    {
        private static readonly object Padlock = new object();
        private readonly ISubscriptionConfiguration _configuration;
        private readonly IDatabaseContextFactory _databaseContextFactory;

        private readonly IDatabaseGateway _databaseGateway;

        private readonly List<string> _deferredSubscriptions = new List<string>();
        private readonly IScriptProvider _scriptProvider;

        private readonly IServiceBusConfiguration _serviceBusConfiguration;

        private readonly Dictionary<string, List<string>> _subscribers = new Dictionary<string, List<string>>();
        private readonly string _subscriptionConnectionString;
        private readonly string _subscriptionProviderName;

        private bool _deferSubscribtions = true;

        private readonly ILog _log;

        public SubscriptionManager(IServiceBusEvents events, IServiceBusConfiguration serviceBusConfiguration,
            ISubscriptionConfiguration configuration, IScriptProvider scriptProvider,
            IDatabaseContextFactory databaseContextFactory, IDatabaseGateway databaseGateway)
        {
            Guard.AgainstNull(events, "events");
            Guard.AgainstNull(serviceBusConfiguration, "serviceBusConfiguration");
            Guard.AgainstNull(configuration, "configuration");
            Guard.AgainstNull(scriptProvider, "scriptProvider");
            Guard.AgainstNull(databaseContextFactory, "databaseContextFactory");
            Guard.AgainstNull(databaseGateway, "databaseGateway");

            _log = Log.For(this);

            _serviceBusConfiguration = serviceBusConfiguration;
            _configuration = configuration;
            _scriptProvider = scriptProvider;
            _databaseContextFactory = databaseContextFactory;
            _databaseGateway = databaseGateway;

            events.Started += ServiceBus_Started;

            _subscriptionProviderName = configuration.ProviderName;

            if (string.IsNullOrEmpty(_subscriptionProviderName))
            {
                throw new ConfigurationErrorsException(string.Format(SubscriptionResources.ProviderNameEmpty,
                    "SubscriptionManager"));
            }

            _subscriptionConnectionString = configuration.ConnectionString;

            if (string.IsNullOrEmpty(_subscriptionConnectionString))
            {
                throw new ConfigurationErrorsException(string.Format(SubscriptionResources.ConnectionStringEmpty,
                    "SubscriptionManager"));
            }

            using (_databaseContextFactory.Create(_subscriptionProviderName, _subscriptionConnectionString))
            {
                if (_databaseGateway.GetScalarUsing<int>(
                        RawQuery.Create(
                            _scriptProvider.Get(
                                Script.SubscriptionManagerExists))) != 1)
                {
                    try
                    {
                        _databaseGateway.ExecuteUsing(RawQuery.Create(
                            _scriptProvider.Get(
                                Script.SubscriptionManagerCreate)));
                    }
                    catch (Exception ex)
                    {
                        if (
                            !ex.Message.Equals(
                                "There is already an object named 'SubscriberMessageType' in the database.",
                                StringComparison.OrdinalIgnoreCase))
                        {
                            throw new DataException(SubscriptionResources.SubscriptionManagerCreateException, ex);
                        }
                    }
                }
            }
        }

        protected bool HasDeferredSubscriptions
        {
            get { return _deferredSubscriptions.Count > 0; }
        }

        public void Subscribe(IEnumerable<string> messageTypeFullNames)
        {
            Guard.AgainstNull(messageTypeFullNames, "messageTypeFullNames");

            if (_deferSubscribtions)
            {
                _deferredSubscriptions.AddRange(messageTypeFullNames);

                return;
            }

            if (_serviceBusConfiguration.IsWorker || _configuration.Subscribe == SubscribeOption.Ignore)
            {
                return;
            }

            if (!_serviceBusConfiguration.HasInbox
                ||
                _serviceBusConfiguration.Inbox.WorkQueue == null)
            {
                throw new InvalidOperationException(EsbResources.SubscribeWithNoInboxException);
            }

            var missingMessageTypes = new List<string>();

            using (_databaseContextFactory.Create(_subscriptionProviderName, _subscriptionConnectionString))
            {
                foreach (var messageType in messageTypeFullNames)
                {
                    if (_configuration.Subscribe == SubscribeOption.Normal)
                    {
                        _databaseGateway.ExecuteUsing(
                            RawQuery.Create(
                                    _scriptProvider.Get(Script.SubscriptionManagerSubscribe))
                                .AddParameterValue(SubscriptionManagerColumns.InboxWorkQueueUri,
                                    _serviceBusConfiguration.Inbox.WorkQueue.Uri.ToString())
                                .AddParameterValue(SubscriptionManagerColumns.MessageType, messageType));
                    }
                    else
                    {
                        if (_databaseGateway.GetScalarUsing<int>(
                                RawQuery.Create(
                                        _scriptProvider.Get(Script.SubscriptionManagerContains))
                                    .AddParameterValue(SubscriptionManagerColumns.InboxWorkQueueUri,
                                        _serviceBusConfiguration.Inbox.WorkQueue.Uri.ToString())
                                    .AddParameterValue(SubscriptionManagerColumns.MessageType, messageType)) == 0)
                        {
                            missingMessageTypes.Add(messageType);
                        }
                    }
                }
            }

            if (!missingMessageTypes.Any())
            {
                return;
            }

            foreach (var messageType in missingMessageTypes)
            {
                _log.Error(string.Format(SubscriptionResources.MissingSubscription, messageType));
            }

            throw new ApplicationException(string.Format(SubscriptionResources.MissingSubscriptionException, string.Join(",", missingMessageTypes)));
        }

        public void Subscribe(string messageTypeFullName)
        {
            Subscribe(new[] {messageTypeFullName});
        }

        public void Subscribe(IEnumerable<Type> messageTypes)
        {
            Subscribe(messageTypes.Select(messageType => messageType.FullName).ToList());
        }

        public void Subscribe(Type messageType)
        {
            Subscribe(new[] {messageType.FullName});
        }

        public void Subscribe<T>()
        {
            Subscribe(new[] {typeof(T).FullName});
        }

        public IEnumerable<string> GetSubscribedUris(object message)
        {
            Guard.AgainstNull(message, "message");

            var messageType = message.GetType().FullName;

            if (!_subscribers.ContainsKey(messageType))
            {
                lock (Padlock)
                {
                    if (!_subscribers.ContainsKey(messageType))
                    {
                        DataTable table;

                        using (_databaseContextFactory.Create(_subscriptionProviderName, _subscriptionConnectionString))
                        {
                            table = _databaseGateway.GetDataTableFor(
                                RawQuery.Create(
                                        _scriptProvider.Get(
                                            Script.SubscriptionManagerInboxWorkQueueUris))
                                    .AddParameterValue(SubscriptionManagerColumns.MessageType, messageType));
                        }

                        _subscribers.Add(messageType, (from DataRow row in table.Rows
                                select SubscriptionManagerColumns.InboxWorkQueueUri.MapFrom(row))
                            .ToList());
                    }
                }
            }

            return _subscribers[messageType];
        }

        private void ServiceBus_Started(object sender, EventArgs e)
        {
            _deferSubscribtions = false;

            if (HasDeferredSubscriptions)
            {
                Subscribe(_deferredSubscriptions);
            }
        }
    }
}