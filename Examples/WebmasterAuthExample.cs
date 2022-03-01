using QuoteMedia.Streamer.Client.Auth;
using QuoteMedia.Streamer.Message.Control;
using QuoteMedia.Streamer.Message.Marketdata;
using System;

namespace QuoteMedia.Streamer.Client.Examples
{
    // Demonstrates how to:
    // - Configure and create a market data stream
    // - Connect the market data stream to the streaming service using WebmasterID (WMID) credentials
    // - Subscribe for level 1 market data
    // - Unsubscribe from previously subscribed data
    class Example2
    {
        private static readonly string WMID = "102383";
        private static readonly string SERVICE_URI = "http://app.quotemedia.com/cache/stream/v1";
        private static readonly string[] SYMBOLS = new string[] { "GOOG", "MSFT" };
        private static readonly MarketdataType[] TYPES = new MarketdataType[] { MarketdataType.QUOTE, MarketdataType.PRICEDATA };

        static void Main(string[] args)
        {
            StreamConfig cfg = new StreamConfig();
            using (MarketdataStream stream = StreamerAPI.Create(cfg))
            {
                // Register callbacks
                stream.OnMessage = HandleMessage;
                stream.OnError = HandleError;
                stream.OnCtrlMessage = HandleCtrlMessage;
                // Open stream
                stream.Open(SERVICE_URI, new WebmasterCredentials(WMID));
                // Subscribe
                SubscribeResponse subscribed = stream.Subscribe(SYMBOLS, TYPES);
                if (subscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Subscribe failed"); }

                Console.WriteLine("Hit <ENTER> to exit");
                Console.ReadLine();

                // Unsubscribe
                UnsubscribeResponse unsubscribed = stream.Unsubscribe(SYMBOLS, TYPES);
                if (unsubscribed.Code != ResponseCodes.OK_CODE) { throw new Exception("Unsubscribe failed"); }

            }
        }

        private static void HandleMessage(MarketdataMessage msg)
        {
            // Handle market data
            Console.WriteLine(msg.ToString());
        }

        private static void HandleError(Exception e)
        {
            // Handle error
            Console.WriteLine(e.ToString());

        }

        private static void HandleCtrlMessage(CtrlMessage msg)
        {
            Console.WriteLine(msg.ToString());

        }
    }
}

