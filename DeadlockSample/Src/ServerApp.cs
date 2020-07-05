using System.Threading;
using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;

namespace DeadlockSample
{
    class ServerApp : BaseApplication
    {
        private int _orderSeq = 0;

        public void OnMessage(NewOrderSingle order, SessionID sessionId)
        {
            var orderId = Interlocked.Increment(ref _orderSeq);
            var orderIdString = orderId.ToString().PadLeft(10, '0');

            var report = new ExecutionReport(
                new OrderID(orderIdString),
                new ExecID(orderIdString),
                new ExecType(ExecType.NEW),
                new OrdStatus(OrdStatus.NEW),
                order.Symbol,
                order.Side,
                new LeavesQty(order.OrderQty.Obj),
                new CumQty(0),
                new AvgPx(0)
            )
            {
                ClOrdID = order.ClOrdID
            };

            Session.SendToTarget(report, sessionId);

            if (orderId % 1000 == 0)
            {
                Logger.Information($"Received & responded to {orderId} NewOrderSingle messages");
            }
        }
    }
}