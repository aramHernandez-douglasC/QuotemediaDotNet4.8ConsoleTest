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
    // - Change logging configuration on the fly

    // Default logging configuration is set in App.config file.
    // In case if no logging configuration is specified the logging is turned off.
    // By default, StreamerApiExamples project has log level set to TRACE and the output file is application.log.
    // Control messages are logged with log level DEBUG. Market data messages are logged with log level TRACE.
    class Example5
    {
        // Adjust authentication credentials to your account.
        private static string WMID = "YourWebmasterID";
        private static string USERNAME = "YourUsername";
        private static string PASSWORD = "YourPassword";
        private static string SERVICE_URI = "http://app.quotemedia.com/cache/stream/v1";
        private static string[] SYMBOLS = new string[] { "AAPL", "GOOG", "MSFT" };
        private static MarketdataType[] DATATYPES = new MarketdataType[] { MarketdataType.TRADE, MarketdataType.PRICEDATA };

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol |= SecurityProtocolType.Tls12;
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

                SubscriptionOptions subscriptionOptions = new SubscriptionOptions();
                subscriptionOptions.Conflation = StreamConfig.Conflation.DEFAULT;
                // Subscribe
                // With default log level set to TRACE market data messages will be logged into a file.
                SubscribeResponse subscribed = stream.Subscribe(SYMBOLS, DATATYPES, subscriptionOptions);
                if (subscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Subscribe failed."); }

                // Changing log level to see only control messages
                StreamerAPI.UpdateLogLevel(Logging.LogLevel.DEBUG);

                // Retrieve stats
                StatsResponse stats = stream.GetSessionStats();
                Debug.WriteLine(stats);

                // Changing log configuration to see all messages in application2.log file
                StreamerAPI.UpdateLogConfig(Logging.LogLevel.TRACE, "application2.log");

                Console.WriteLine("Hit <ENTER> to exit.");
                Console.ReadLine();

                // Reseting log config to default values specified in App.config file
                StreamerAPI.ResetLogConfig();

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

