using System;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using Multiplayer;

namespace MikoUtils
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("Human.exe")]
    public class Plugin : BaseUnityPlugin
    {
        private Harmony _harmony;

        public void Awake() { _harmony = new Harmony(PluginInfo.PLUGIN_GUID); }

        public void Start() { _harmony.PatchAll();

            // Register commands
            // Shell.RegisterCommand("spamkick", new Action<string>(MikoCmds.OnSpamKick), "spamkick <player#> <times> \r\nRepeatedly kicks player (vanilla soft kick) \r\nUsefull for kicking some modded players in a funny way");
            NetChat.RegisterCommand(true, false, "invt", new Action(MikoCmds.ToggleInvite), "/invt-切换仅限邀请");
            NetChat.RegisterCommand(true, false, "join", new Action(MikoCmds.ToggleJoin), "/join-切换参加正在进行的游戏");
            Shell.RegisterCommand("s", new Action<string>(MikoCmds.SubObjective), null);
            Shell.RegisterCommand("subobj", new Action<string>(MikoCmds.SubObjective), "subobj(s) <subobjective> \r\n加载当前存档点下的次要目标 \r\nsubobjective~次要目标序号 接受的值：整数，+/-号，带+/-号的整数");
            Shell.RegisterCommand("cc", new Action<string>(MikoCmds.ChangeCP), null);
            Shell.RegisterCommand("changecp", new Action<string>(MikoCmds.ChangeCP), "cc <checkpoint> \r\n改变存档点，但不加载 \r\ncheckpoint~存档点序号 接受的值：整数，+/-号，带+/-号的整数");
            NetChat.RegisterCommand(true, false, "lockcp", new Action(MikoCmds.LockCP), null);
            NetChat.RegisterCommand(true, false, "locklvl", new Action(MikoCmds.LockLvl), null);
        }

        public void OnDestroy() { _harmony.UnpatchSelf(); }
    }

    public class MikoVar
    {
        public static bool CPLocked;
    }

    public class MikoFunc
    {
        // Used in level/checkpoint/subobject loading commands,
        // making it possibel to load them with either absolute id, or steps relative from the id
        public static int LoadNum(string txt, int cur)
        {
            int result;
            if (txt.StartsWith("+") || txt.StartsWith("-"))
            {
                int delta;

                if (!int.TryParse(txt, out delta))
                {
                    if (txt == "+") { delta = 1; }
                    else if (txt == "-") { delta = -1; }
                    else { return -1; }
                }

                result = cur + delta;
            }
            else if (!int.TryParse(txt, out result)) { return -1; }

            return Math.Max(0, result);
        }

        // Used in level command,
        // making it possibel to load extra levels
        public static Tuple<ulong, WorkshopItemSource> EXLvl(ulong level, WorkshopItemSource levelType)
        {
            ulong builtInLength = (ulong)(long)Game.instance.levels.Length;
            if (level >= builtInLength && levelType == WorkshopItemSource.BuiltIn)
            {
                level -= builtInLength;
                levelType = WorkshopItemSource.EditorPick;
            }
            return Tuple.Create(level, levelType);;
        }
    }

    public class MikoCmds 
    {
        // // Spam kick - host
        // // + equivalent to spamming /kick <#player> (vanilla /kick instead of Power kick)
        // // + Why? Because it is funny lol
        // public static void OnSpamKick(string txt)
        // {
        //     if (string.IsNullOrEmpty(txt))
        //     {
        //         CommandRegistry.ShowCurrentHelp();
        //         return;
        //     }

        //     string[] array = txt.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //     string plr;
        //     int times;

        //     if (array.Length == 1)
        //     {
        //         plr = txt;
        //         times = 999;
        //     }
        //     else if (array.Length == 2)
        //     {
        //         plr = array[0];
        //         if (!int.TryParse(array[1], out times))
        //         {
        //             Shell.Print($"不接受当前值 \"{array[1]}\" 为重复次数");
        //             return;
        //         }
        //     }
        //     else
        //     {
        //         Shell.Print("多余参数");
        //         return;
        //     }

        //     NetChat instance = NetChat.instance;

        //     MethodInfo methodInfo = typeof(NetChat).GetMethod("GetClient", BindingFlags.NonPublic | BindingFlags.Instance);
        //     NetHost client = (NetHost)methodInfo.Invoke(instance, new object[] { plr });

        //     if (client == null)
        //     {
        //         return;
        //     }

        //     if (client == NetGame.instance.local)
        //     {
        //         Shell.Print("不会踢走自己");
        //         return;
        //     }

        //     for (int i = 0; i < times; i++)
        //     {
        //         NetGame.instance.Kick(client);
        //     }
        // }

        // In game InviteOnly toggle - host
        public static void ToggleInvite()
        {
            Options.lobbyInviteOnly ^= 1;

			NetGame.instance.transport.UpdateLobbyType();
			NetGame.instance.transport.UpdateOptionsLobbyData();

            NetChat.Print($"仅限邀请：{Convert.ToBoolean(Options.lobbyInviteOnly)}");
        }

        // In game JoinInProgress toggle - host
        public static void ToggleJoin()
        {
            Options.lobbyJoinInProgress ^= 1;

			NetGame.instance.transport.UpdateJoinInProgress();
			NetGame.instance.transport.UpdateOptionsLobbyData();

            NetChat.Print($"参加正在进行的游戏：{Convert.ToBoolean(Options.lobbyJoinInProgress)}");
        }

        // Load SubObjective (similar to level and cp) - host
        public static void SubObjective(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("subobj(s) <subobjective> \r\n加载当前存档点下的次要目标 \r\nsubobjective~次要目标序号 接受的值：整数，+/-号，带+/-号的整数");
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

        public static void ChangeCP(string txt) {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("cc");
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
                Game.instance.currentCheckpointNumber = Math.Min(C, Game.currentLevel.checkpoints.Length - 1);
                Game.instance.currentCheckpointSubObjectives = 0;
                Shell.Print(string.Format($"更改至存档点 {C}"));
            }
            catch
            {
            }
        }

        public static void LockCP() 
        {
            MikoVar.CPLocked = !MikoVar.CPLocked;
            NetChat.Print($"锁定存档点：{MikoVar.CPLocked}");
        }

        public static void LockLvl()
        {
            Options.lobbyLockLevel ^= 1;
			NetGame.instance.transport.UpdateOptionsLobbyData();
            NetChat.Print($"锁定关卡：{Convert.ToBoolean(Options.lobbyLockLevel)}");
        }
    }

    // Power kick - host
    // + Force disconnect players from the host after /kick <#player>
    // + Further attempt from the players to connect to the session is already stopped by vanilla game \ / 
    [HarmonyPatch(typeof(NetChat), "OnKick")]
    public class YesKick
    {
        public static void Postfix(NetChat __instance, string txt)
        {
            MethodInfo methodInfo = AccessTools.Method(typeof(NetChat), "GetClient");
            NetHost client = (NetHost)methodInfo.Invoke(__instance, new object[] { txt });

            NetGame.instance.OnDisconnect(client.connection, false);
        }
    }

    // Level Command utilities - host
    // + level(l) command can be used in multiplayer session now
    // + relative level loading.
    [HarmonyPatch(typeof(CheatCodes), "LevelChange")]
    public class LevelChange
    {
        public static bool Prefix(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("level(l) <level> <checkpoint> \r\n加载关卡 \r\nlevel~关卡序号 接受的值：整数，+/-号，带+/-号的整数 \r\ncheckpoint~存档点序号 接受的值：整数，留空");
                return false;
            }

            if (NetGame.isServer && Game.instance.currentLevelNumber == -1)
            {
                Shell.Print("无法在大厅内切换关卡");
                return false;
            }

            string[] array = txt.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (array.Length > 2)
            {
                Shell.Print("参数过长");
                return false;
            }

            int L;
            int C = 0;

            // level
            L = MikoFunc.LoadNum(array[0], Game.instance.currentLevelNumber);

            if (L == -1)
            {
                Shell.Print($"不接受当前值 \"{array[0]}\"");
                return false;
            }

            L = Math.Min(Game.instance.levels.Length + Game.instance.editorPickLevels.Length - 1, L);

            // cp
            if (array.Length == 2)
            {
                if (!int.TryParse(array[1], out C))
                {
                    Shell.Print($"不接受当前值 \"{array[1]}\"");
                    return false;
                }
            }

            C = Math.Max(0, C);

            try
            {
                App.instance.StartNextLevel((ulong)(long)L, C);
                Shell.Print($"加载关卡 {L} 存档点 {C}");
            }
            catch {}
            
            return false;
        }
    }

    [HarmonyPatch(typeof(NetGame), "ServerLoadLevel")]
    public class ServerLoadEX
    {
        public static bool Prefix(ref ulong number, ref WorkshopItemSource levelType)
        {
            Tuple<ulong, WorkshopItemSource> r = MikoFunc.EXLvl(number, levelType);
            number = r.Item1; levelType = r.Item2;

            return true;
        }
    }
    [HarmonyPatch(typeof(App), "LaunchGame", new Type[] { typeof(ulong), typeof(WorkshopItemSource), typeof(int), typeof(int), typeof(Action) })]
    public class LaunchEX
    {
        public static bool Prefix(ref ulong level, ref WorkshopItemSource levelType)
        {
            Tuple<ulong, WorkshopItemSource> r = MikoFunc.EXLvl(level, levelType);
            level = r.Item1; levelType = r.Item2;

            return true;
        }
    }

    [HarmonyPatch(typeof(App), "LevelLoadedServer")]
    public class EXLoadedServer
    {
        public static bool Prefix(ref ulong level, ref WorkshopItemSource levelType)
        {
            Tuple<ulong, WorkshopItemSource> r = MikoFunc.EXLvl(level, levelType);
            level = r.Item1; levelType = r.Item2;

            return true;
        }
    }


    // CP command utilities - host
    // + relative cp loading.
    [HarmonyPatch(typeof(CheatCodes), "CheckpointChange")]
    public class CheckpointChange
    {
        public static bool Prefix(string txt)
        {
            if (string.IsNullOrEmpty(txt))
            {
                Shell.Print("cp(c) <checkpoint> \r\n加载存档点 \r\ncheckpoint~存档点序号 接受的值：整数，+/-号，带+/-号的整数");
                return false;
            }

            int C;

            C = MikoFunc.LoadNum(txt, Game.instance.currentCheckpointNumber);

            if (C == -1)
            {
                Shell.Print($"不接受当前值 \"{txt}\"");
                return false;
            }

            try
            {
                Game.instance.RestartCheckpoint(C, 0);
                Shell.Print(string.Format($"加载存档点 {C}"));
                return false;
            }
            catch
            {
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Game), "EnterCheckpoint")]
    public class NoCP
    {
        public static bool Prefix()
        {
            if (MikoVar.CPLocked == true) { return false; }

            return true;
        }
    }
}
