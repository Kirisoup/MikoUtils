public static partial class Lang
{
    public static void en()
    {
        try {
            helps["level"] =
            "Load level"+ _b+
            _t+ "Accepted values for <level>: integers, +/-, integers starting with +/-;"+ _b+
            _t+ "Accepted values for <checkpoint>: integers, left as empty";

            helps["cp"] =
            "Load checkpoint"+ _b+
            _t+ "Accepted values for <checkpoint>: integers, +/-, integers starting with +/-";

            helps["subobj"] =
            "Load sub-objectives under the current checkpoint"+ _b+
            _t+ "Accepted values for <subobjective>: integers, +/-, integers starting with +/-";

            helps["changecp"] =
            "Change checkpoint without reload"+ _b+
            _t+ "Accepted values for <checkpoint>: integers, +/-, integers starting with +/-";

            helps["invite"] =
            "Toggle invite only option";

            helps["joingame"] =
            "Toggle join game in progress option";

            helps["locklvl"] =
            "Toggle lock level option";

            helps["lockcp"] =
            "Toggle lock checkpoint option";

            helps["broadcast"] =
            "Toggle broadcast mikoutil notifications to netchat";

            helps["disconnect"] =
            "Disconnects a player from lobby";

            helps["synclvl"] =
            "Send current level info to clients to fix level desync";


            errors["badVal"] =
            "Current value cannot be accepted";

            errors["tooLong"] =
            "Current parameter(s) is too long";

            errors["inLobby"] =
            "Cannot load levels while inside a lobby";

            errors["isClient"] =
            "This action can only be performed by lobby host";
        } catch {}

        loadLevel =
        "Loading level";

        loadCP =
        "Loading checkpoint";

        loadSubobj =
        "Loading sub-objective";

        changeCP =
        "Checkpoint changed to";

        inviteOnly =
        "Invite Only";

        joinInProgress =
        "Join Game in Progress";

        lockLvl =
        "Lock Level";

        lockCP =
        "Lock Checkpoint";

        joinMsg =
        "joined";

        leftMsg =
        "left";

        broadcast =
        "Broadcast notifications to netchat";
    }
}