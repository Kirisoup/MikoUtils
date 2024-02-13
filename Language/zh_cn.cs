public static partial class Lang
{
    public static void zh_cn()
    {
        try {
            helps["level"] =
            "加载存档点"+ _b+
            _t+ "关卡序号 <level> 接受的值：整数，+/-，带+/-的整数；"+ _b+
            _t+ "存档点序号 <checkpoint> 接受的值：整数，留空";

            helps["cp"] =
            "加载存档点"+ _b+
            _t+ "存档点序号 <checkpoint> 接受的值：整数，+/-，带+/-的整数";

            helps["subobj"] =
            "加载当前存档点下的 sub-objective"+ _b+
            _t+ "Sub-objective 序号 <subobjective> 接受的值：整数，+/-，带+/-的整数；";

            helps["changecp"] =
            "Change checkpoint without reload"+ _b+
            _t+ "存档点序号 <checkpoint> 接受的值：整数，+/-，带+/-的整数；";

            helps["invite"] =
            "切换“仅限邀请”选项";

            helps["joingame"] =
            "切换“参加正在进行的游戏”选项";

            helps["locklvl"] =
            "切换“锁定关卡”选项";

            helps["lockcp"] =
            "切换“锁定存档点”选项";

            helps["broadcast"] =
            "切换是否广播本插件通知到公屏";

            helps["disconnect"] =
            "断开玩家与大厅的连接";

            helps["synclvl"] =
            "向客机发送当前关卡信息，以修复关卡不同步问题";


            errors["badVal"] =
            "不接受当前输入的值";

            errors["tooLong"] =
            "当前参数过长";

            errors["inLobby"] =
            "无法在大厅加载关卡";

            errors["isClient"] =
            "无法在客机执行";
        } catch {}

        loadLevel =
        "加载关卡";

        loadCP =
        "加载存档点";

        loadSubobj = 
        "加载次要目标";

        changeCP = 
        "存档点更改至";

        inviteOnly = 
        "仅限邀请";

        joinInProgress = 
        "参加正在进行的游戏";

        lockLvl = 
        "锁定关卡";

        lockCP = 
        "锁定存档点";

        joinMsg =
        "加入了游戏";

        leftMsg =
        "离开了游戏";

        broadcast =
        "广播通知到公屏";
    }
}