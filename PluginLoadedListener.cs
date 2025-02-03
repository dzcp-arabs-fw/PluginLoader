﻿using System;

namespace PluginFramework
{
    public partial class PluginLoadedListener : IEventListener
    {
        public void OnEventTriggered(string eventName, object eventData)
        {
            Logger.Log($"🔔 [PluginLoadedListener] تم تشغيل الحدث: {eventName}");
            Console.WriteLine($"🔔 [PluginLoadedListener] تم تشغيل الحدث: {eventName}");
        }
    }
}