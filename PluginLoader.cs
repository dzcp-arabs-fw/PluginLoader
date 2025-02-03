using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Loader;

namespace PluginFramework
{
    public partial class PluginLoader
    {
        private static readonly string PluginsDirectory = "Plugins";
        private static readonly string ConfigsDirectory = "Configs";
        private static readonly string LogsDirectory = "Logs";
        private static readonly Dictionary<string, IPlugin> LoadedPlugins = new Dictionary<string, IPlugin>();
        private static readonly Dictionary<string, List<IEventListener>> EventListeners = new Dictionary<string, List<IEventListener>>();

        [PluginEntryPoint("PluginLoader", "1.0.0", "Custom Plugin Loader for SCP:SL", "YourName")]
        public void Init()
        {
            try
            {
                ShowConsoleBanner();
                ServerConsole.AddLog("⚡ [PluginLoader] Plugin Loader has been initialized!", ConsoleColor.Cyan);
                Logger.Log("🚀 [PluginLoader] Plugin Loader has started!");

                InitializeDirectories();
                LoadPlugins();
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ [PluginLoader] Error during initialization: {ex.Message}");
            }
        }

        private static void InitializeDirectories()
        {
            CreateDirectory(PluginsDirectory);
            CreateDirectory(ConfigsDirectory);
            CreateDirectory(LogsDirectory);
        }

        private static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                Logger.Log($"📂 [PluginLoader] Created directory: {path}");
            }
        }

        public static void LoadPlugins()
        {
            Logger.Log("[PluginLoader] Searching for plugins...");
            ServerConsole.AddLog("[PluginLoader] Searching for plugins...", ConsoleColor.Yellow);

            var pluginFiles = Directory.GetFiles(PluginsDirectory, "*.dll");

            if (!pluginFiles.Any())
            {
                Logger.Log("[PluginLoader] No plugins found.");
                ServerConsole.AddLog("[PluginLoader] No plugins found.", ConsoleColor.Red);
                return;
            }

            foreach (var pluginFile in pluginFiles)
            {
                LoadPlugin(pluginFile);
            }

            ServerConsole.AddLog("✅ [PluginLoader] All plugins loaded successfully!", ConsoleColor.Green);
        }

        private static void LoadPlugin(string pluginFile)
        {
            try
            {
                Logger.Log($"🔄 [PluginLoader] Loading plugin: {pluginFile}");
                ServerConsole.AddLog($"🔄 [PluginLoader] Loading plugin: {pluginFile}", ConsoleColor.Green);

                var assembly = Assembly.LoadFrom(pluginFile);
                var pluginTypes = assembly.GetTypes()
                    .Where(type => typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract);

                foreach (var type in pluginTypes)
                {
                    var pluginInstance = (IPlugin)Activator.CreateInstance(type);
                    pluginInstance.OnLoad();
                    LoadedPlugins[pluginInstance.Name] = pluginInstance;

                    Logger.Log($"✅ [PluginLoader] Plugin loaded: {pluginInstance.Name} - Version {pluginInstance.Version}");
                    ServerConsole.AddLog($"✅ [PluginLoader] Plugin loaded: {pluginInstance.Name} - Version {pluginInstance.Version}", ConsoleColor.Green);
                }
            }
            catch (Exception ex)
            {
                Logger.Log($"❌ [PluginLoader] Error loading plugin {pluginFile}: {ex.Message}");
                ServerConsole.AddLog($"❌ [PluginLoader] Error loading plugin {pluginFile}: {ex.Message}", ConsoleColor.Red);
            }
        }

        // ✅ إصلاح `RegisterListener`
        public static void RegisterListener(string eventName, IEventListener listener)
        {
            if (!EventListeners.ContainsKey(eventName))
            {
                EventListeners[eventName] = new List<IEventListener>();
            }
            if (!EventListeners[eventName].Contains(listener))
            {
                EventListeners[eventName].Add(listener);
                ServerConsole.AddLog($"✅ [PluginLoader] Registered event listener for: {eventName}", ConsoleColor.Green);
            }
        }

        // ✅ إصلاح `TriggerEvent`
        public static void TriggerEvent(string eventName, object eventData = null)
        {
            if (EventListeners.ContainsKey(eventName))
            {
                ServerConsole.AddLog($"🔔 [PluginLoader] Triggering event: {eventName}", ConsoleColor.Yellow);
                foreach (var listener in EventListeners[eventName])
                {
                    listener.OnEventTriggered(eventName, eventData);
                }
            }
            else
            {
                ServerConsole.AddLog($"⚠️ [PluginLoader] No listeners found for event: {eventName}", ConsoleColor.Red);
            }
        }

        private static void ShowConsoleBanner()
        {
            string banner = @"
 ██████╗ ██╗      ██████╗  ██████╗ ███████╗
██╔═══██╗██║      ██╔══██╗██╔═══██╗██╔════╝
██║   ██║██║      ██████╔╝██║   ██║███████╗
██║   ██║██║      ██╔══██╗██║   ██║╚════██║
╚██████╔╝███████╗██║  ██║╚██████╔╝███████║
 ╚═════╝ ╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚══════╝
        ";
            ServerConsole.AddLog(banner, ConsoleColor.Blue);
            ServerConsole.AddLog("⚡ [PluginLoader] Plugin Loader is running!", ConsoleColor.Cyan);
        }
    }
}
