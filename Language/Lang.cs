
using System;
using System.Reflection;
using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using Multiplayer;
using I2.Loc;

public partial class Lang
{
    public static string _b = Environment.NewLine;
    public static string _t = "\t";

    public static Dictionary<string, string> helps = new()
    {
        { "level", "" },
        { "cp", "" },
        { "subobj", "" },
        { "changecp", "" },
        { "invite", "" },
        { "joingame", "" },
        { "locklvl", "" },
        { "lockcp", "" },
        { "broadcast", "" },
        { "disconnect", "" },
        { "synclvl", "" },
    };

    public static Dictionary<string, string> errors = new()
    {
        { "badVal", "" },
        { "tooLong", "" },
        { "inLobby", "" },
        { "isClient", "" },
    };

    public static string loadLevel;
    public static string loadCP;
    public static string loadSubobj;
    public static string changeCP;
    public static string inviteOnly;
    public static string joinInProgress;
    public static string lockLvl;
    public static string lockCP;
    public static string joinMsg;
    public static string leftMsg;
    public static string broadcast;

    public static void Translate()
    {
        switch(LocalizationManager.CurrentLanguage) {
            case "Chinese Simplified": zh_cn();
            break;

            default: en();
            break;
        }
    }
}
