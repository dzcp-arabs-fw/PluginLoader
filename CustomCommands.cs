using System;
using ExamplePlugin;
using PluginFramework;
using PluginAPI.Core;
using PluginAPI.Events;
using EventManager = PluginFramework.EventManager;
using IPlugin = PluginFramework.PluginLoader  ;

namespace CustomCommands
{
    public partial class CustomCommandManager : IPlugin
    {
        public string Name => "Custom Command Manager";
        public string Version => "1.0.0";

        public void OnLoad()
        {
            ServerConsole.AddLog($"✅ [{Name}] Loaded successfully!", ConsoleColor.Green);

            // ✅ تسجيل الأوامر داخل `SCPSL Server`
            EventManager.RegisterListener("server_command", new  ExampleEventListener());
            ServerConsole.AddLog($"🔹 [CustomCommands] Registered commands: myhelp, sethp, gethp, teleport, announce", ConsoleColor.Cyan);
        }
    }

    public class CustomCommandListener :  IEventListener
    {
        public void OnEventTriggered(string eventName, object eventData)
        {
            ServerConsole.AddLog($"🔍 Event triggered: {eventName} | Data: {eventData}", ConsoleColor.Yellow);

            if (eventName == "server_command" && eventData is string command)
            {
                ServerConsole.AddLog($"🔍 Received command: {command}", ConsoleColor.Magenta);

                string[] args = command.Split(' ');
                if (args.Length < 1) return;

                string action = args[0].ToLower();

                switch (action)
                {
                    case "myhelp":
                        ShowHelp();
                        break;

                    case "sethp":
                        if (args.Length < 3) return;
                        SetPlayerHealth(args[1], args[2]);
                        break;

                    case "gethp":
                        if (args.Length < 2) return;
                        GetPlayerHealth(args[1]);
                        break;

                    case "teleport":
                        if (args.Length < 3) return;
                        TeleportPlayer(args[1], args[2]);
                        break;

                    case "announce":
                        if (args.Length < 2) return;
                        AnnounceMessage(string.Join(" ", args, 1, args.Length - 1));
                        break;

                    default:
                        ServerConsole.AddLog($"❌ Unknown command: {command}", ConsoleColor.Red);
                        break;
                }
            }
        }

        private void ShowHelp()
        {
            ServerConsole.AddLog("📜 Available Custom Commands:", ConsoleColor.Cyan);
            ServerConsole.AddLog("🔹 myhelp - عرض جميع الأوامر المخصصة", ConsoleColor.White);
            ServerConsole.AddLog("🔹 sethp <player> <hp> - ضبط نقاط حياة اللاعب", ConsoleColor.White);
            ServerConsole.AddLog("🔹 gethp <player> - عرض نقاط حياة اللاعب", ConsoleColor.White);
            ServerConsole.AddLog("🔹 teleport <player> <target> - نقل لاعب إلى موقع لاعب آخر", ConsoleColor.White);
            ServerConsole.AddLog("🔹 announce <message> - إرسال إعلان للجميع", ConsoleColor.White);
        }

        private void SetPlayerHealth(string playerName, string healthValue)
        {
            if (!int.TryParse(healthValue, out int health))
            {
                ServerConsole.AddLog("❌ Invalid health value!", ConsoleColor.Red);
                return;
            }

            Player player = Player.Get(playerName);
            if (player == null)
            {
                ServerConsole.AddLog($"❌ Player {playerName} not found!", ConsoleColor.Red);
                return;
            }

            player.Health = health;
            ServerConsole.AddLog($"✅ Set {player.Nickname}'s HP to {health}!", ConsoleColor.Green);
        }

        private void GetPlayerHealth(string playerName)
        {
            Player player = Player.Get(playerName);
            if (player == null)
            {
                ServerConsole.AddLog($"❌ Player {playerName} not found!", ConsoleColor.Red);
                return;
            }

            ServerConsole.AddLog($"ℹ️ {player.Nickname}'s HP: {player.Health}", ConsoleColor.Cyan);
        }

        private void TeleportPlayer(string playerName, string targetName)
        {
            Player player = Player.Get(playerName);
            Player target = Player.Get(targetName);

            if (player == null || target == null)
            {
                ServerConsole.AddLog($"❌ Player or target not found!", ConsoleColor.Red);
                return;
            }

            player.Position = target.Position;
            ServerConsole.AddLog($"✅ Teleported {player.Nickname} to {target.Nickname}!", ConsoleColor.Green);
        }

        private void AnnounceMessage(string message)
        {
            ServerConsole.AddLog($"📢 Announcement: {message}", ConsoleColor.Magenta);
        }
    }
}
