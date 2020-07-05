using System;
using System.Threading;
using System.Threading.Tasks;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace DeadlockSample
{
    class ClientApp : BaseApplication
    {
        public int MessagesCount { get; }
        private SessionID _sessionId;

        private int _receivedMessages;

        public ClientApp(int messagesCount)
        {
            MessagesCount = messagesCount;
        }

        public override void OnLogon(SessionID sessionId)
        {
            base.OnLogon(sessionId);
            _sessionId = sessionId;

            Task.Factory.StartNew(Run, TaskCreationOptions.LongRunning);
        }

        public void OnMessage(ExecutionReport report, SessionID sessionId)
        {
            var receivedMessages = Interlocked.Increment(ref _receivedMessages);
            if (receivedMessages % 1000 == 0)
            {
                Logger.Information($"Received {receivedMessages} ExecutionReport messages");
            }
        }

        private void Run()
        {
            var order = new NewOrderSingle(
                new ClOrdID("temp"),
                new Symbol("MSFT"),
                new Side(Side.BUY),
                new TransactTime(DateTime.UtcNow),
                new OrdType(OrdType.LIMIT)
            )
            {
                Price = new Price(100),
                OrderQty = new OrderQty(1)
            };

            Logger.Information("Starting");

            for (var i = 0; i < MessagesCount; ++i)
            {
                var count = i + 1;
                var clOrdId = count.ToString().PadLeft(10, '0');

                order.ClOrdID = new ClOrdID(clOrdId);
                Session.SendToTarget(order, _sessionId);

                if (count % 1_000 == 0)
                {
                    Logger.Information($"Sent {count} NewOrderSingle messages");
                }
            }
        }
    }
}