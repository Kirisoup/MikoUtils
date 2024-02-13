using System; 
using System.Reflection; 
using System.Collections.Generic; 
using BepInEx; 
using HarmonyLib; 
using Multiplayer; 
 
namespace MikoUtils 
{ 
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)] 
    [BepInProcess("Human.exe")] 
    public class Plugin : BaseUnityPlugin 
    { 
        public Harmony harmony; 
 
        public void Awake() 
        { 
            harmony = new Harmony(PluginInfo.PLUGIN_GUID); 
        } 
 
        public void Start() 
        { 
            harmony.PatchAll(); 
 
            Lang.Translate(); 
            MikoCmds.Reg.RegCmds(); 
        } 
 
        public void OnDestroy() 
        { 
            harmony.UnpatchSelf(); 
        } 
 
        public class MikoCmds 
        { 
            public class Reg 
            { 
                public static void RegCmds() 
                { 
                    /* load cheats */ 
                    RegShell("subobj", "s", 
                        new Action<string>(Cmds.LoadSubObj), 
                        Lang.helps["subobj"], "<subobj>" 
                    ); 
 
                    RegShell("changecp", "cc", 
                        new Action<string>(Cmds.ChangeCP),  
                        Lang.helps["changecp"], "<checkpoint>" 
                    ); 
 
                    ModHelp("level", "l", 
                        Lang.helps["level"], "<level> <checkpoint?>" 
                    ); 
 
                    ModHelp("cp", "c", 
                        Lang.helps["cp"], "<checkpoint>" 
                    ); 
 
                    /* lobby settings */ 
                    RegChat("invonly", "io", 
                        new Action(Cmds.ToggleInvite),  
                        Lang.helps["invite"] 
                    ); 
 
                    RegChat("joingame", "join", 
                        new Action(Cmds.ToggleJoin),  
                        Lang.helps["joingame"] 
                    ); 
 
                    RegChat("locklvl", "lockl", 
                        new Action(Cmds.LockLvl.ChatEntry),  
                        Lang.helps["locklvl"] 
                    ); 
 
                    RegShell("locklvl", "lockl", 
                        new Action(Cmds.LockLvl.ShellEntry),  
                        Lang.helps["locklvl"] 
                    ); 
 
                    RegChat("lockcp", "lockc", 
                        new Action(Cmds.LockCP.ChatEntry),  
                        Lang.helps["lockcp"] 
                    ); 
 
                    RegShell("lockcp", "lockc", 
                        new Action(Cmds.LockCP.ShellEntry),  
                        Lang.helps["lockcp"] 
                    ); 
 
                    /* mp utils */ 
                    RegChat("broadcast", "", 
                        new Action(Cmds.ToggleBroadcast),  
                        Lang.helps["broadcast"] 
                    ); 
 
                    RegChat("disconnect", "dc", 
                        new Action<string>(Cmds.Disconnect),  
                        Lang.helps["disconnect"], "<player#>" 
                    ); 
 
                    RegChat("synclvl", "sync", 
                        new Action(Cmds.SyncLvl),  
                        Lang.helps["synclvl"] 
                    ); 
                } 
 
                private static void RegShell(string cmd, string abbr, Action action, string help = null) 
                { 
                    help = HelpUtil(cmd, abbr, help); 
 
                    Shell.RegisterCommand(cmd, action, help); 
 
                    if (!string.IsNullOrEmpty(abbr)) Shell.RegisterCommand(abbr, action, null); 
                } 
                private static void RegShell(string cmd, string abbr, Action<string> action, string help = null, string val = null) 
                { 
                    help = HelpUtil(cmd, abbr, help, val); 
 
                    Shell.RegisterCommand(cmd, action, help); 
 
                    if (!string.IsNullOrEmpty(abbr)) Shell.RegisterCommand(abbr, action, null); 
                } 
 
                private static void RegChat(string cmd, string abbr, Action action, string help = null, string val = null) 
                { 
                    help = HelpUtil_(cmd, abbr, help); 
 
                    NetChat.RegisterCommand(true, false, cmd, action, help); 
 
                    if (!string.IsNullOrEmpty(abbr)) NetChat.RegisterCommand(true, false, abbr, action, null); 
                } 
                private static void RegChat(string cmd, string abbr, Action<string> action, string help = null, string val = null) 
                { 
                    help = HelpUtil_(cmd, abbr, help, val); 
 
                    NetChat.RegisterCommand(true, false, cmd, action, help); 
 
                    if (!string.IsNullOrEmpty(abbr)) NetChat.RegisterCommand(true, false, abbr, action, null); 
                } 
 
                private static void ModHelp(string cmd, string abbr, string help, string val = null) 
                {  
                    try { description_[cmd] = HelpUtil(cmd, abbr, help, val); } catch {} 
                } 
 
                private static string HelpUtil(string cmd, string abbr, string help, string val = null) 
                { 
                    // unless help is null, prefix help with "cmd - " or "cmd(abbr) - " 
                    return string.IsNullOrEmpty(help) ? 
                        null : 
                        $"{cmd}{( 
                            string.IsNullOrEmpty(abbr) ? 
                                null : 
                                $"({abbr})" 
                        )}{( 
                            string.IsNullOrEmpty(abbr) ? 
                                null : 
                                " " + val 
                        )} - {help}";  
                } 
 
                private static string HelpUtil_(string cmd, string abbr, string help, string val = null) 
                { 
                    return "/" + HelpUtil(cmd, abbr, help, val); // unless help is null, prefix help with "/cmd - " or "/cmd(abbr) - " 
                } 
 
                private static FieldInfo commandsField = typeof(Shell).GetField("commands", BindingFlags.NonPublic | BindingFlags.Static); 
                private static CommandRegistry CommandRegistry_ = (CommandRegistry)commandsField.GetValue(null); 
 
                private static FieldInfo descriptionfield = typeof(CommandRegistry).GetField("description", BindingFlags.NonPublic | BindingFlags.Instance); 
                private static Dictionary<string, string> description_ = (Dictionary<string, string>)descriptionfield.GetValue(CommandRegistry_); 
            } 
 
            public class Cmds 
            { 
                public static void LoadSubObj(string txt) 
                { 
                    if (Empty(txt, Lang.helps["subobj"])) return; 
 
                    if (IsClient) return; 
 
                    int S = NumberHack(txt, Game.instance.currentCheckpointSubObjectives); 
 
                    if (BadVal(S == -1)) return; 
 
                    try { 
                        Game.instance.RestartCheckpoint(Game.instance.currentCheckpointNumber, S); 
                        Shell.Print($"{Lang.loadSubobj}: {S}"); 
                    } catch {} 
                } 
 
                public static void ChangeCP(string txt) 
                { 
                    if (Empty(txt, Lang.helps["changecp"])) return; 
 
                    if (IsClient) return; 
 
                    int C = NumberHack(txt, Game.instance.currentCheckpointNumber); 
 
                    if (BadVal(C == -1)) return; 
 
                    try { 
                        Game.instance.currentCheckpointNumber = Math.Min(C, Game.currentLevel.checkpoints.Length - 1); 
                        Game.instance.currentCheckpointSubObjectives = 0; 
                        Shell.Print($"{Lang.changeCP}: {C}"); 
                    } catch {} 
                } 
 
                public static void Disconnect(string txt) 
                { 
                    if (IsClient) return; 
 
                    MethodInfo methodInfo = AccessTools.Method(typeof(NetChat), "GetClient"); 
                    NetHost client = (NetHost)methodInfo.Invoke(NetChat.instance, new object[] { txt }); 
 
                    NetGame.instance.OnDisconnect(client.connection, false); 
                } 
 
                public static void ToggleBroadcast() 
                { 
                    if (IsClient) return; 
 
                    DoBroadcast = !DoBroadcast; 
 
                    string Bool = $"{DoBroadcast}"; 
                    Chat.Msg($"<color=#00DDDD>= {Lang.broadcast}: {Chat.FormatKW(Bool)}</color>"); 
                } 
 
                public static void ToggleInvite() 
                { 
                    if (IsClient) return; 
 
                    Options.lobbyInviteOnly ^= 1; 
 
                    NetGame.instance.transport.UpdateLobbyType(); 
                    NetGame.instance.transport.UpdateOptionsLobbyData(); 
 
                    string Bool = $"{Convert.ToBoolean(Options.lobbyInviteOnly)}"; 
                    Chat.MsgFormat($"{Lang.inviteOnly}: {Chat.FormatKW(Bool)}"); 
                } 
 
                public static void ToggleJoin() 
                { 
                    if (IsClient) return; 
 
                    Options.lobbyJoinInProgress ^= 1; 
 
                    NetGame.instance.transport.UpdateJoinInProgress(); 
                    NetGame.instance.transport.UpdateOptionsLobbyData(); 
 
                    string Bool = $"{Convert.ToBoolean(Options.lobbyJoinInProgress)}"; 
                    Chat.MsgFormat($"{Lang.joinInProgress}: {Chat.FormatKW(Bool)}"); 
                } 
 
                public class LockCP 
                { 
                    public static void ChatEntry()  
                    { 
                        if (IsClient) return; 
 
                        Toggle(); 
 
                        Chat.MsgFormat($"{Lang.lockCP}: {Chat.FormatKW($"{CPLocked}")}"); 
                    } 
 
                    public static void ShellEntry()  
                    { 
                        if (IsClient) return; 
 
                        Toggle(); 
 
                        Shell.Print($"{Lang.lockCP}: {CPLocked}"); 
                    } 
 
                    private static void Toggle() 
                    { 
                        CPLocked = !CPLocked; 
                    } 
                } 
 
                public class LockLvl 
                { 
                    public static void ChatEntry() 
                    { 
                        if (IsClient) return; 
 
                        Toggle(); 
 
                        Chat.MsgFormat($"{Lang.lockLvl}: {Chat.FormatKW($"{LvlLocked}")}"); 
                    } 
 
                    public static void ShellEntry() 
                    { 
                        if (IsClient) return; 
 
                        Toggle(); 
 
                        Shell.Print($"{Lang.lockLvl}: {LvlLocked}"); 
                    } 
 
                    private static void Toggle() 
                    { 
                        LvlLocked = !LvlLocked; 
 
                        if (NetGame.isServer) ToggleSvr(); 
                    } 
 
                    private static void ToggleSvr() 
                    { 
                        Options.lobbyLockLevel = LvlLocked ? 1 : 0; 
                        NetGame.instance.transport.UpdateOptionsLobbyData(); 
                    } 
                } 
 
                public static void SyncLvl() 
                { 
                    if (App.state == AppSate.ServerPlayLevel || App.state == AppSate.ServerLobby) 
                    NetGame.instance.ServerLoadLevel((ulong)(long)Game.instance.currentLevelNumber, Game.instance.currentLevelType, true, 0U); 
                } 
            } 
 
            public static bool CPLocked; 
 
            public static bool LvlLocked = Options.lobbyLockLevel != 0; 
 
            public static bool DoBroadcast; 
 
        } 
 
        public class MikoPatch 
        { 
            public class LevelCmd 
            { 
                [HarmonyPatch(typeof(CheatCodes), "LevelChange")] 
                public class LevelChange 
                { 
                    public static bool Prefix(string txt) 
                    { 
                        LoadLevel(txt); 
                        return false; 
                    } 
                } 
 
                [HarmonyPatch(typeof(NetGame), "ServerLoadLevel")] 
                [HarmonyWrapSafe] 
                public class ExServerLoadLevel 
                { 
                    public static bool Prefix(ref ulong number, ref WorkshopItemSource levelType) 
                    { 
                        object[] result = IfEx(number, levelType); 
                        number = (ulong)result[0]; levelType = (WorkshopItemSource)result[1]; 
 
                        return true; 
                    } 
                } 
 
                [HarmonyPatch(typeof(App), "LaunchGame", new Type[] { typeof(ulong), typeof(WorkshopItemSource), typeof(int), typeof(int), typeof(Action) })] 
                [HarmonyWrapSafe] 
                public class ExLaunchGame 
                { 
                    public static bool Prefix(ref ulong level, ref WorkshopItemSource levelType) 
                    { 
                        object[] result = IfEx(level, levelType); 
                        level = (ulong)result[0]; levelType = (WorkshopItemSource)result[1]; 
 
                        return true; 
                    } 
                } 
 
                [HarmonyPatch(typeof(App), "LevelLoadedServer")] 
                [HarmonyWrapSafe] 
                public class ExLevelLoadedServer 
                { 
                    public static bool Prefix(ref ulong level, ref WorkshopItemSource levelType) 
                    { 
                        object[] result = IfEx(level, levelType); 
                        level = (ulong)result[0]; levelType = (WorkshopItemSource)result[1]; 
 
                        return true; 
                    } 
                } 
 
                private static object[] IfEx(ulong level, WorkshopItemSource levelType) 
                { 
                    ulong builtInLength = (ulong)(long)Game.instance.levels.Length; 
                    if (level >= builtInLength && levelType == WorkshopItemSource.BuiltIn) 
                    { 
                        level -= builtInLength; 
                        levelType = WorkshopItemSource.EditorPick; 
                    } 
                    return new object[] {level, levelType}; 
                } 
 
                private static void LoadLevel(string txt) 
                { 
                    if (Empty(txt, Lang.helps["level"])) return; 
 
                    if (IsClient) return; 
 
                    if (NetGame.isServer && Game.instance.currentLevelNumber == -1) 
                    { 
                        Shell.Print(Lang.errors["inLobby"]); 
                        return; 
                    } 
 
                    string[] array = txt.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); 
 
                    if (TooLong(array, 2)) return; 
 
                    // level 
                    int CurL = Game.instance.currentLevelNumber; 
                    int NumL = Game.instance.levels.Length; 
                    int NumL_ = Game.instance.editorPickLevels.Length; 
                    string TypeL = Game.instance.currentLevelType.ToString(); 
 
                    if (TypeL != "BuiltIn") 
                    { 
                        if (TypeL == "EditorPick") CurL += NumL; 
 
                        else CurL = 0; 
                    } 
 
                    int L = NumberHack(array[0], CurL); 
 
                    if (BadVal(L == -1)) return; 
 
                    L = Math.Min(NumL + NumL_ - 1, L); 
 
                    // cp 
                    int C = 0; 
 
                    if (array.Length == 2) 
                    if (BadVal(!int.TryParse(array[1], out C))) return; 
 
                    C = Math.Max(0, C); 
 
                    try { 
                        App.instance.StartNextLevel((ulong)(long)L, C); 
                        Shell.Print($"{Lang.loadLevel}: {L}; {Lang.loadCP}: {C}"); 
                    } catch { } 
                } 
            } 
 
            public class CPCmd 
            { 
                [HarmonyPatch(typeof(CheatCodes), "CheckpointChange")] 
                public class CheckpointChange 
                { 
                    public static bool Prefix(string txt) 
                    { 
                        LoadCP(txt); 
                        return false; 
                    } 
                } 
 
                private static void LoadCP(string txt) 
                { 
                    if (Empty(txt, Lang.helps["cp"])) return; 
 
                    if (IsClient) return; 
 
                    int C; 
 
                    C = NumberHack(txt, Game.instance.currentCheckpointNumber); 
 
                    if (BadVal(C == -1)) return; 
 
                    try { 
                        Game.instance.RestartCheckpoint(C, 0); 
                        Shell.Print($"{Lang.loadCP}: {C}"); 
                        return; 
                    } catch {} 
                } 
            } 
 
            [HarmonyPatch(typeof(NetChat), "OnKick")] 
            public class AntiAntiKick 
            { 
                public static void Postfix(string txt) 
                { 
                    if (int.TryParse(txt, out int num) && num - 2 < NetGame.instance.readyclients.Count && num > 1) 
                    { 
                        NetHost client = NetGame.instance.readyclients[num - 2]; 
                        NetGame.instance.OnDisconnect(client.connection, false); 
                    } 
                } 
            } 
 
            [HarmonyPatch(typeof(Game), "EnterCheckpoint")] 
            public class LockCP 
            { 
                public static bool Prefix() 
                { 
                    if (MikoCmds.CPLocked == true) return false; 
                    return true; 
                } 
            } 
 
            [HarmonyPatch(typeof(Game), "EnterPassZone")] 
            public class LockLvl 
            { 
                public static bool Prefix() 
                { 
                    if (MikoCmds.LvlLocked && !NetGame.isServer) return false; 
                    return true; 
                } 
            } 
 
            [HarmonyPatch(typeof(NetGame), "OnClientHelo")] 
            public class JoinMsg 
            { 
                public static void Postfix(ref NetHost client) 
                { 
                    Chat.MsgFormat($"<size=18>{Chat.FormatKW($"{client.name}")} {Lang.joinMsg}</size>"); 
                } 
            } 
 
            [HarmonyPatch(typeof(NetGame), "OnServerDisconnect")] 
            public class LeftMsg 
            { 
                public static void Postfix(ref NetHost client) 
                { 
                    Chat.MsgFormat($"<size=18>{Chat.FormatKW($"{client.name}")} {Lang.leftMsg}</size>"); 
                } 
            } 
 
            [HarmonyPatch(typeof(LanguageMenu), "SetLanguage")] 
            public class UpdateLanguage 
            { 
                public static void Postfix() 
                { 
                    Lang.Translate(); 
                } 
            } 
        } 
 
        public class Chat 
        { 
            public static void MsgFormat(string notif) 
            { 
                if (!MikoCmds.DoBroadcast) Msg($"<color=#00DDDD>= {notif}</color>", 0); 
 
                else Msg($"<color=#00DDDD># {notif}</color>"); 
            } 
 
            public static string FormatKW(string kw) 
            { 
                kw = $"<color=#FFFFFF><b>{kw}</b></color>"; 
                return kw; 
            } 
 
            public static void Msg(string msg, int clientId = -1) 
            { 
                if (NetGame.isClient) return; 
 
                NetStream netStream = NetGame.BeginMessage(NetMsgId.Chat); 
 
                netStream.WriteNetId(NetGame.instance.local.hostId); 
                netStream.Write(msg); 
                  
                if (clientId == -1) 
                { 
                    NetChat.Print(msg); 
 
                    for (int i = 0; i < NetGame.instance.readyclients.Count; i++) 
                    NetGame.instance.SendReliable(NetGame.instance.readyclients[i], netStream); 
                } 
 
                else if (clientId == 0) NetChat.Print(msg); 
 
                else 
                { 
                    NetHost sendto = NetGame.instance.FindReadyHost((uint)clientId); 
                    NetGame.instance.SendReliable(sendto, netStream); 
                } 
 
                netStream?.Release(); 
            } 
        } 
 
        public static int NumberHack(string txt, int cur) 
        { 
            int result; 
 
            if (txt.StartsWith("+") || txt.StartsWith("-")) 
            { 
                if (!int.TryParse(txt, out int delta)) return -1; 
 
                result = cur + delta; 
            } 
            else if (!int.TryParse(txt, out result)) return -1; 
 
            return Math.Max(0, result); 
        } 
 
        public static bool IsClient 
        {  
            get { 
                bool flag = NetGame.isClient; 
                if (flag) Shell.Print(Lang.errors["isClient"]); 
                return flag; 
            } 
        } 
 
        public static bool BadVal(bool flag) 
        { 
            if (flag) Shell.Print(Lang.errors["badVal"]); 
            return flag; 
        } 
 
        public static bool Empty(string txt, string help, bool ChatInstead = false) 
        { 
            bool flag = string.IsNullOrEmpty(txt); 
            if (flag) {  
                if (ChatInstead) NetChat.Print(help);  
                else Shell.Print(help); 
            } 
            return flag; 
        } 
 
        public static bool TooLong(Array array, int expected) 
        { 
            bool flag = array.Length > expected; 
            if (flag) Shell.Print(Lang.errors["tooLong"]); 
            return flag; 
        } 
    } 
} 
