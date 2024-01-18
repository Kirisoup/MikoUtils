using BepInEx;
using HarmonyLib;
using Multiplayer;
using System;
using System.Reflection;


namespace MikoUtils
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Human.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;

        public void Awake()
        {
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
        }

        public void Start()
        {
            _harmony.PatchAll();

            // Register commande
            Shell.RegisterCommand("spamkick", new Action<string>(MikoCmds.OnSpamKick), "spamkick <player#> <times>\r\nRepeatedly kicks player (vanilla soft kick). \r\nUsefull for kicking some modded players in a funny way.");
            NetChat.RegisterCommand(true, false, "invt", new Action(MikoCmds.ToggleInvite), "/invt-切换仅限邀请");
			NetChat.RegisterCommand(true, false, "join", new Action(MikoCmds.ToggleJoin), "/join-切换参加正在进行的游戏");
            Shell.RegisterCommand("s", new Action<string>(MikoCmds.SubObjective), null);
            Shell.RegisterCommand("so", new Action<string>(MikoCmds.SubObjective), "so(s) <subobjective>\r\nsubobjective~次要目标序号 接受的值：整数，+/-号，带+/-号的整数");
        }

        public void OnDestroy()
        {
            _harmony.UnpatchSelf();
        }
    }

    public class MikoFunc {
        public static int LoadNum(string txt, int cur)
        {
            int result;
            if (txt.StartsWith("+") || txt.StartsWith("-"))
            {
                int delta;

                if (!int.TryParse(txt, out delta))
                {
                    if (txt == "+")
                    {
                        delta = 1;
                    }
                    else if (txt == "-")
                    {
                        delta = -1;
                    }
                    else
                    {
                        return -1;
                    }
                }

                result = cur + delta;
            }
            else if (!int.TryParse(txt, out result))
            {
                return -1;
            }

            return Math.Max(0, result);
        }
    }

    // // Prevent kick (remove this when releasing!) - client
    // // > Prevents any codes in App.ServerKicked() to execute.   
    // [HarmonyPatch(typeof(App), "ServerKicked")]
    // public class KickMeNot
    // {
    //     [HarmonyPrefix]
    //     public static bool Prefix()
    //     {
    //     	NetChat.Print("KickMeNot:)");
    //         return false;
    //     }
    // }

    // Power kick - host
    // > Force disconnect player from the host when /kick <#player>
    // > Further attempt from the player to connect to session is already stopped by vanilla game \ / 
    [HarmonyPatch(typeof(NetChat), "OnKick")]
    public class YesKick
    {
        [HarmonyPostfix]
        public static void Postfix(NetChat __instance, string txt)
        {
            MethodInfo methodInfo = AccessTools.Method(typeof(NetChat), "GetClient");
            NetHost client = (NetHost)methodInfo.Invoke(__instance, new object[] { txt });

			NetGame.instance.OnDisconnect(client.connection, false);
        }
    }

    // Level Command utilities - host
    // > level(l) command can be used in multiplayer session now
    // > relative level no.
    [HarmonyPatch(typeof(CheatCodes), "LevelChange")]
    public class LevelChange
    {
        // Hijacks vanilla CheatCodes.LevelChange method
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
        
        // Instead: 
        [HarmonyPostfix]
        public static void Postfix(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("level(l) <level> <checkpoint>\r\nlevel~关卡序号 接受的值：整数，+/-号，带+/-号的整数\r\ncheckpoint~存档点序号 接受的值：整数，留空");
                return;
            }

            // Return if in multiplayer lobby
            if (NetGame.isServer && Game.instance.currentLevelNumber == -1)
            {
                Shell.Print("无法在大厅内切换关卡");
                return;
            }

            string[] array = txt.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (array.Length > 2)
            {
                Shell.Print("参数过长");
                return;
            }

            int L;
            int C = 0;

            // level
            L = MikoFunc.LoadNum(array[0], Game.instance.currentLevelNumber);

            if (L == -1)
            {
                Shell.Print($"不接受当前值 \"{array[0]}\"");
                return;
            }

            L = Math.Min(Game.instance.levels.Length - 1, L);

            // cp
            if (array.Length == 2)
            {
                if (!int.TryParse(array[1], out C))
                {
                    Shell.Print($"不接受当前值 \"{array[1]}\"");
                    return;
                }
            }

            C = Math.Max(0, C);

            try
            {
                App.instance.StartNextLevel((ulong)(long)L, C);
                Shell.Print($"加载关卡 {L} 存档点 {C}");
            }
            catch {}
        }
    }

    // CP command utilities - host
    // > relative cp no.
    [HarmonyPatch(typeof(CheatCodes), "CheckpointChange")]
    public class CheckpointChange
    {
        // Hijacks vanilla CheatCodes.CheckpointChange method
        [HarmonyPrefix]
        public static bool Prefix()
        {
            return false;
        }
        
        // Instead: 
        [HarmonyPostfix]
        public static void Postfix(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("cp(c) <checkpoint>\r\ncheckpoint~存档点序号 接受的值：整数，+/-号，带+/-号的整数");
                return;
            }

            int C;

            C = MikoFunc.LoadNum(txt, Game.instance.currentCheckpointNumber);

            if (C == -1)
            {
                Shell.Print($"不接受当前值 \"{txt}\"");
                return;
            }

            try
            {
                Game.instance.RestartCheckpoint(C, 0);
                Shell.Print(string.Format($"加载存档点 {C}"));
            }
            catch
            {
            }
        }
    }

    public class MikoCmds 
    {
        // Spam kick - host
        // > equivalent to spamming /kick <#player> (vanilla /kick instead of Power kick)
        // > Why? Because it is funny lol
        public static void OnSpamKick(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                CommandRegistry.ShowCurrentHelp();
                return;
            }

            string[] array = txt.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string plr;
            int times;

            if (array.Length == 1)
            {
                plr = txt;
                times = 999;
            }
            else if (array.Length == 2)
            {
                plr = array[0];
                if (!int.TryParse(array[1], out times))
                {
                    Shell.Print($"不接受当前值 \"{array[1]}\" 为重复次数");
                    return;
                }
            }
            else
            {
                Shell.Print("多余参数");
                return;
            }

            NetChat instance = NetChat.instance;

            MethodInfo methodInfo = typeof(NetChat).GetMethod("GetClient", BindingFlags.NonPublic | BindingFlags.Instance);
            NetHost client = (NetHost)methodInfo.Invoke(instance, new object[] { plr });

            if (client == null)
            {
                return;
            }

            if (client == NetGame.instance.local)
            {
                Shell.Print("不会踢走自己");
                return;
            }

            for (int i = 0; i < times; i++)
            {
                NetGame.instance.Kick(client);
            }
        }

        // In game InviteOnly toggle - host
        public static void ToggleInvite()
		{
			if (Options.lobbyInviteOnly == 0)
			{
				new MultiplayerLobbySettingsMenu().InviteOnlyChanged(1);
				NetChat.Print("仅限邀请：启用");
			}
            else 
            {
                new MultiplayerLobbySettingsMenu().InviteOnlyChanged(0);
                NetChat.Print("仅限邀请：禁用");
            }
		}

        // In game JoinInProgress toggle - host
		public static void ToggleJoin()
		{
			if (Options.lobbyJoinInProgress == 0)
			{
				new MultiplayerLobbySettingsMenu().JoinInProgressChanged(1);
				NetChat.Print("参加正在进行的游戏：启用");
			}
            else {
                new MultiplayerLobbySettingsMenu().JoinInProgressChanged(0);
                NetChat.Print("参加正在进行的游戏：禁用");
            }
		}

        // Load SubObjective (similar to level and cp) - host
        public static void SubObjective(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("so(s) <subobjective>\r\nsubobjective~次要目标序号 接受的值：整数，+/-号，带+/-号的整数");
                return;
            }

            int S;

            S = MikoFunc.LoadNum(txt, Game.instance.currentCheckpointSubObjectives);

            if (S == -1)
            {
                Shell.Print($"不接受当前值 \"{txt}\"");
                return;
            }

            try
            {
                Game.instance.RestartCheckpoint(Game.instance.currentCheckpointNumber, S);
                Shell.Print(string.Format($"加载次要目标 {S}"));
            }
            catch
            {
            }
        }
    }
}
