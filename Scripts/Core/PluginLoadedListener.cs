using System;
using PluginFramework;

namespace PluginLoader.Scripts.Core
{
    public partial class PluginLoadedListener : IEventListener
    {
        public void OnEventTriggered(string eventName, object eventData)
        {
            SCPSL_Framework.Utilities.Logger.Log($"🔔 [PluginLoadedListener] تم تشغيل الحدث: {eventName}");
            Console.WriteLine($"🔔 [PluginLoadedListener] تم تشغيل الحدث: {eventName}");
        }
    }
}