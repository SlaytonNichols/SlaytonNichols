using System;
using ServiceStack;
using ServiceStack.Logging;
using SlaytonNichols.ServiceModel;

namespace SlaytonNichols.ServiceInterface
{
    public class MyServices : Service
    {
        public static ILog Log = LogManager.GetLogger(typeof (MyServices));

        public object Any(Hello request)
        {
            Log.Info("Info Log");
            Log.Debug("Debug Log");
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }
    }
}
