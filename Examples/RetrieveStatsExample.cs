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
    // - Retrieve number of available connections and symbols, current number of opened connections and subscribed symbols
    // - Subscribe for level1 market data
    // - Unsubscribe from previously subscribed data
    class SubscriptionExample
    {

        // Adjust authentication credentials to your account.
        private static string WMID = "YourWebmasterID";
        private static string USERNAME = "YourUsername";
        private static string PASSWORD = "YourPassword";
        private static string SERVICE_URI = "http://app.quotemedia.com/cache/stream/v1";
        private static string[] SYMBOLS = new string[] { "GOOG", "MSFT" };
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
                // As well as per connection, there is ability to specify conflation per subscription request through SubscriptionOptions.
                // If conflation is not specified, default conflation value is used.
                // Please note. A matching conflation must be supplied when unsubscribing.
                SubscriptionOptions subscriptionOptions = new SubscriptionOptions();
                subscriptionOptions.Conflation = StreamConfig.Conflation.DEFAULT;
                // Subscribe
                SubscribeResponse subscribed = stream.Subscribe(SYMBOLS, DATATYPES, subscriptionOptions);
                if (subscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Subscribe failed."); }

                // Retrieve stats
                StatsResponse stats = stream.GetSessionStats();
                Debug.WriteLine(stats);
                Console.WriteLine("Hit <ENTER> to exit.");
                Console.ReadLine();

                // Unsubscribe
                UnsubscribeResponse unsubscribed = stream.Unsubscribe(SYMBOLS, DATATYPES, subscriptionOptions);
                if (unsubscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Unsubscribe failed"); }
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

