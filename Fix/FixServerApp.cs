using QuickFix;
using QuickFix.Fields;
using QuickFix.FIX44;
using System.Collections.Concurrent;
using Message = QuickFix.Message;


namespace OrderAccumulator.Fix
{
    public class FixServerApp : MessageCracker, IApplication
    {
        private readonly ConcurrentDictionary<string, decimal> _exposures = new();
        private const decimal LIMIT = 100_000_000m;

        public void FromAdmin(Message message, SessionID sessionID) { }

        public void FromApp(Message message, SessionID sessionID)
        {
            Crack(message, sessionID);
        }
        public void OnCreate(SessionID sessionID) { }

        public void OnLogon(SessionID sessionID) =>
            Console.WriteLine($"[LOGON] {sessionID}");

        public void OnLogout(SessionID sessionID) =>
            Console.WriteLine($"[LOGOUT] {sessionID}");

        public void ToAdmin(Message message, SessionID sessionID) { }

        public void ToApp(Message message, SessionID sessionID) { }

        public void OnMessage(NewOrderSingle order, SessionID sessionID)
        {
            var symbol = order.Symbol.Value;
            var side = order.Side.Value;
            var qty = order.OrderQty.Value;
            var price = order.Price.Value;
            var value = qty * price;

            var currentExposure = _exposures.GetOrAdd(symbol, 0m);
            var newExposure = side == Side.BUY
                ? currentExposure + value
                : currentExposure - value;

            var execReport = new ExecutionReport(
                new OrderID(Guid.NewGuid().ToString()),
                new ExecID(Guid.NewGuid().ToString()),
                new ExecType(),
                new OrdStatus(),
                order.Symbol,
                order.Side,
                new LeavesQty(0),
                new CumQty(qty),
                new AvgPx(price)
            );

            execReport.SetField(new ClOrdID(order.ClOrdID.Value));

            if (Math.Abs(newExposure) <= LIMIT)
            {
                _exposures[symbol] = newExposure;
                execReport.ExecType = new ExecType(ExecType.NEW);
                execReport.OrdStatus = new OrdStatus(OrdStatus.NEW);
            }
            else
            {
                execReport.ExecType = new ExecType(ExecType.REJECTED);
                execReport.OrdStatus = new OrdStatus(OrdStatus.REJECTED);
                execReport.SetField(new Text("Exposição excedida"));
            }

            var statusTexto = execReport.ExecType.Value == ExecType.NEW ? "ACEITA" :
                  execReport.ExecType.Value == ExecType.REJECTED ? "REJEITADA" : "DESCONHECIDA";

            Console.WriteLine(
                $"[OrderID: {order.ClOrdID.Value}] " +
                $"[{symbol}] {qty} @ {price} ({(side == Side.BUY ? "BUY" : "SELL")}) | " +
                $"EXP: {newExposure} | STATUS: {statusTexto}");

            Session.SendToTarget(execReport, sessionID);
        }

    }
}
