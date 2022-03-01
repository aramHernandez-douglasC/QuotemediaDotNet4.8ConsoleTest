using QuoteMedia.Streamer.Client.Auth;
using QuoteMedia.Streamer.Message.Control;
using QuoteMedia.Streamer.Message.Marketdata;
using System;
using System.Net;
using System.Diagnostics;
using static QuoteMedia.Streamer.Client.MarketdataStream;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace QuoteMedia.Streamer.Client.Examples
{
    // Demonstrates how to:
    // - Configure and create a market data stream
    // - Connect the market data stream to the streaming service using enduser credentials
    // - Retrieve number of available connections and symbols, current number of opened connections and subscribed symbols asynchronously
    // - Subscribe for level1 market data asynchronously
    // - Unsubscribe from previously subscribed data asynchronously
    class Example4
    {

        // Adjust authentication credentials to your account.
        private static string WMID = "102383";
        private static string USERNAME = "QuotemediaAPITest4";
        private static string PASSWORD = "QuotemediaAPITest4";
        private static string SERVICE_URI = "http://app.quotemedia.com/cache/stream/v1";
        private static string[] SYMBOLS = new string[] { "GOOG", "MSFT" };
        private static MarketdataType[] DATATYPES = new MarketdataType[] { MarketdataType.QUOTE, MarketdataType.PRICEDATA };

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
            // Create configuration, use fluent api to set properties.
            // If conflation is not specified, default value is used.
            StreamConfig cfg = new StreamConfig().ConflationMs(StreamConfig.Conflation.DEFAULT);
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
                Task<SubscribeResponse> subscriptionTask = stream.SubscribeAsync(SYMBOLS, DATATYPES, subscriptionOptions);
                // Handle subscription response
                subscriptionTask.Wait();
                if (subscriptionTask.Result.Code != ResponseCodes.OK_CODE) { throw new Exception("Subscribe failed."); }

                // Retrieve stats
                Task<StatsResponse> stats = stream.GetSessionStatsAsync();
                stats.Wait();
                Debug.WriteLine(stats.Result);
                Console.WriteLine("Hit <ENTER> to exit.");
                Console.ReadLine();

                // Unsubscribe
                Task<UnsubscribeResponse> unsubscriptionTask = stream.UnsubscribeAsync(SYMBOLS, DATATYPES, subscriptionOptions);
                // Handle unsubscription response
                unsubscriptionTask.Wait();
                if (unsubscriptionTask.Result.Code != ResponseCodes.OK_CODE) { throw new Exception("Unsubscribe failed"); }
            }
        }

        private static void HandleMessage(MarketdataMessage msg)
        {
            if (msg is Quote)
            {
                // Handle quote
                Console.WriteLine(msg.ToString());

            }
            else if (msg is PriceData)
            {
                // Handle pricedata
                Console.WriteLine(msg.ToString());

            }
            else if (msg is Trade)
            {
                Console.WriteLine(msg.ToString());
            }
            // ... handle other available message types
        }

        private static void HandleError(Exception e)
        {
            // Handle error
            Console.WriteLine(e.ToString());

        }

        private static void HandleCtrlMessage(CtrlMessage msg)
        {
            // Handle control messages.
            Console.WriteLine(msg.ToString());

        }
    }
}

