using System.Runtime.CompilerServices;
using QuickFix;
using Serilog;
using Log = Serilog.Log;

namespace DeadlockSample
{
    class BaseApplication : MessageCracker, IApplication
    {
        protected readonly ILogger Logger;

        public BaseApplication()
        {
            Logger = Log.ForContext(GetType());
        }

        public virtual void ToAdmin(Message message, SessionID sessionId)
        {
            LogEvent(sessionId, message);
        }

        public virtual void FromAdmin(Message message, SessionID sessionId)
        {
            LogEvent(sessionId, message);
        }

        public virtual void ToApp(Message message, SessionID sessionId)
        {
        }

        public virtual void FromApp(Message message, SessionID sessionId)
        {
            Crack(message, sessionId);
        }

        public virtual void OnCreate(SessionID sessionId)
        {
            LogEvent(sessionId);
        }

        public virtual void OnLogout(SessionID sessionId)
        {
            LogEvent(sessionId);
        }

        public virtual void OnLogon(SessionID sessionId)
        {
            LogEvent(sessionId);
        }

        protected void LogEvent(SessionID sessionId, Message message = null, [CallerMemberName] string method = null)
        {
            Logger.Information($"{method} {message} [{sessionId}]");
        }
    }
}