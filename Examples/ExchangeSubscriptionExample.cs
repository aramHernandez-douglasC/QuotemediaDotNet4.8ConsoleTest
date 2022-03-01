using QuoteMedia.Streamer.Client.Auth;
using QuoteMedia.Streamer.Message.Control;
using QuoteMedia.Streamer.Message.Marketdata;
using System;
using System.Net;
using System.Diagnostics;
using static QuoteMedia.Streamer.Client.MarketdataStream;

namespace QuoteMedia.Streamer.Client.Examples
{
    // Demonstrates how to:
    // - Configure and create a market data stream
    // - Connect the market data stream to the streaming service using enduser credentials
    // - Retrieve connection stats
    // - Subscribe/unsubscribe to an exchange
    class Example6
    {

        // Adjust authentication credentials to your account.
        private static string WMID = "YourWebmasterID";
        private static string USERNAME = "YourUsername";
        private static string PASSWORD = "YourPassword";
        private static string SERVICE_URI = "http://app.quotemedia.com/cache/stream/v1";
        private static string EXCHANGE = "NYE";
        private static MarketdataType[] DATATYPES = new MarketdataType[] { MarketdataType.QUOTE, MarketdataType.PRICEDATA };

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            // Create configuration, use fluent api to set properties.
            // If conflation is not specified, default value is used.
            StreamConfig cfg = new StreamConfig()
                .ConflationMs(StreamConfig.Conflation.DEFAULT)
                // Set format value to MimeTypes.QITCH to use this binary protocol.
                // Please note that although QITCH protocol uses less bandwidth it can cause performance degradation.
                // In case if no format was specified QMCI will be used by default.
                .Format(MimeTypes.QMCI);
      
            // Use factory to create stream from configuration.
            using (MarketdataStream stream = StreamerAPI.Create(cfg))
            {
                // Register callbacks
                stream.OnMessage = HandleMessage;
                stream.OnError = HandleError;
                stream.OnCtrlMessage = HandleCtrlMessage;
                // Open the stream
                stream.Open(SERVICE_URI, new EnduserCredentials(WMID, USERNAME, PASSWORD));

                // If conflation is not specified, default conflation value is used.
                // Please note. A matching conflation must be supplied when unsubscribing.
                ExchangeSubscriptionOptions exchangeSubscriptionOptions = new ExchangeSubscriptionOptions();
                exchangeSubscriptionOptions.Conflation = StreamConfig.Conflation.DEFAULT;

                StreamerAPI.UpdateLogLevel(Logging.LogLevel.DEBUG);

                // Subscribe to the exchange
                ExchangeSubscribeResponse subscribed = stream.SubscribeExchange(EXCHANGE, exchangeSubscriptionOptions);
                Console.WriteLine(subscribed);
                if (subscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Exchange Subscription failed."); }

                // Retrieve stats
                StatsResponse stats = stream.GetSessionStats();
                Debug.WriteLine(stats);
                Console.WriteLine("Hit <ENTER> to exit.");
                Console.ReadLine();

                // Unsubscribe from the exchange
                ExchangeUnsubscribeResponse unsubscribed = stream.UnsubscribeExchange(EXCHANGE, exchangeSubscriptionOptions);
                if (unsubscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Exchange Unsubscription failed"); }
                Console.WriteLine(unsubscribed);
            }
        }

        private static void HandleMessage(MarketdataMessage msg)
        {
            if (msg is Quote)
            {
                // Handle quote
            }
            else if (msg is PriceData)
            {
                // Handle pricedata
            }
            else if (msg is Trade)
            {
                Debug.WriteLine("Trade: " + msg);
            }
            else if (msg is ImbalanceStatus)
            {
                // Handle imbalance status
            }
            // ... handle other available message types
        }

        private static void HandleError(Exception e)
        {
            // Handle error
        }

        private static void HandleCtrlMessage(CtrlMessage msg)
        {            
            // Handle control messages.
        }
    }
}

