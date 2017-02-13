// #autoload
// #name = T2 Stats
// #version = 0.0.1
// #date = January 24, 2017
// #category = Stats
// #author = Hayden McAfee
// #warrior = Apex-
// #email = hayden@outlook.com
// #web = http://github.com/haydenmc
// #web = http://hmcafee.com
// #description = Advanced crowdsourced Tribes 2 statistics.
// #status = Alpha
// #include = support/kill_callbacks.cs
// #include = support/mission_callbacks.cs
// #include = support/list.cs
// ---------------------------------------------------------------------------

// Config
$T2Stats::Debug = true;
$T2Stats::Hostname = "localhost";
$T2Stats::Port = "5000";

// Debug logging function
function t2stats_debugLog(%str) {
    if ($T2Stats::Debug) {
        echo("T2Stats: " @ %str);
    }
}

//
// StatsConnection object
//
function StatsConnection::onDNSFailed(%this)
{
    t2stats_debugLog("StatsConnection DNS lookup failed.");
}

function StatsConnection::onConnectFailed(%this)
{
    t2stats_debugLog("StatsConnection connection failed.");
}

function StatsConnection::onDNSResolved(%this)
{
    t2stats_debugLog("StatsConnection DNS resolved.");
}

function StatsConnection::onConnected(%this)
{
    t2stats_debugLog("StatsConnection connected.");
    %this.send(%this.data);
}

function StatsConnection::onDisconnect(%this)
{
    t2stats_debugLog("StatsConnection disconnected.");
    %this.delete();
}

function StatsConnection::onLine(%this, %line)
{
    t2stats_debugLog( "StatsConnection line received: " @ %line );
}

//
// T2Stats object
//
function T2Stats::reportKill(%this) {
    %weaponName = "Spinfusor";
    %victimGuid = 12345;
    %victimName = "Player 1";
    %killerGuid = 54321;
    %killerName = "Player 2";
    %reporterGuid = 11111;
    %reporterName = "Player 3";
    %matchTime = "00:02:03.23";
    %matchStartTime = "2014-05-06T22:24:55Z";
    %matchDuration = "00:12:02.00";
    %matchMapName = MissionCallback.getMissionName();
    %matchServerIpAddress = MissionCallback.getServerAddress();
    %matchServerPort = 28000;

    %postBody =
        "{" @
        "    weaponName: \"" @ %weaponName @ "\"," @
        "    victim: {" @
        "        tribesGuid: " @ %victimGuid @ "," @
        "        name: \"" @ %victimName @ "\"," @
        "    }," @
        "    killer: {" @
        "        tribesGuid: " @ %killerGuid @ "," @
        "        name: \"" @ %killerName @ "\"," @
        "    }," @
        "    reporter: {" @
        "        tribesGuid: " @ %reporterGuid @ "," @
        "        name: \"" @ reporterName @ "\"," @
        "    }," @
        "    matchTime: \"" @ %matchTime @ "\"," @
        "    match: {" @
        "        startTime: \"" @ %matchStartTime @ "\"," @
        "        duration: \"" @ %matchDuration @ "\"," @
        "        map: {" @
        "            name: \"" @ %matchMapName @ "\"," @
        "        }," @
        "        server: {" @
        "            ipAddress: \"" @ %matchServerIpAddress @ "\"," @
        "            port: " @ %matchServerPort @ "," @
        "        }" @
        "    }" @
        "}";

	%data = 
        "POST /Kills HTTP/1.1\r\n" @
        "Host: " @ $T2Stats::Hostname @ ":" @ $T2Stats::Port @ "\r\n" @
        "User-Agent: Tribes2\r\n" @
        "Content-Type: application/json\r\n" @
        "Content-Length: " @ strlen(%postBody) @ "\r\n" @
        "Connection: close\r\n" @
        "\r\n" @ %postBody;

    t2stats_debugLog(%data);
    
    // %connection = new TCPObject(StatsConnection);
    // %connection.data = %data;
    // %connection.connect($T2Stats::Hostname @ ":" @ T2Stats::Port);
    // %connection.schedule(1000,send,%data);
}

function T2Stats::recordKill(%this, %type, %weaponName, %killerGuid, %killerName, %victimGuid, %victimName) {
    // Fetch some required info
    %matchTimeMs = getSimTime() - %this.matchStartTimeSimMs;
    %matchStartTime = %this.matchStartTime;
    %matchTimeLimitMinutes = %this.matchTimeLimitMinutes;
    %serverAddress = MissionCallback.getServerAddress();
    %serverIp = getSubStr(%serverAddress, 0, strstr(%serverAddress, ":"));
    %serverPort = getSubStr(%serverAddress, strlen(%serverIp) + 1, strlen(%serverAddress) - (strlen(%serverIp) + 1));
    // Instantiate record object
    %killRecord = new ScriptObject()
    {
        type = %type;
        weaponName = %weaponName;
        killerGuid = %killerGuid;
        killerName = %killerName;
        victimGuid = %victimGuid;
        victimName = %victimName;
        matchTimeMs = %matchTimeMs;
        matchStartTime = %matchStartTime;
        matchTimeLimitMinutes = %matchTimeLimitMinutes;
        matchMapName = MissionCallback.getMissionName();
        matchServerIpAddress = %serverIp;
        matchServerPort = %serverPort;
        matchServerName = MissionCallback.getServerName();
    };
    // Push to end of queue
    %this.queuedKillReports.pushBack(%killRecord);
    t2stats_debugLog("New kill recorded: " @ %this.queuedKillReports.size() @ " queued.");
    if ($T2Stats::Debug) {
        %killRecord.dump();
    }
}

//
// Instantiate T2Stats object
//
if(!isObject(T2Stats)) {
    new ScriptObject(T2Stats)
    {
        class = T2Stats;
        clientName = "";
        clientGuid = "";
        matchStartTimeSimMs = 0;
        matchStartTime = "";
        matchTimeLimitMinutes = 0;
        matchTimeLeftMs = 0;
        queuedKillReports = Container::newList();
    };
}

//
// Bind callbacks
//
// Handle kill messages from the server
function t2stats_handleKillCallback(%type, %killer, %victim, %weapon, %i_die, %i_win, %suicide, %tk) {
    t2stats_debugLog("Kill detected (" @ %type @ "): '" @ detag(%killer.name) @ "' killed '" @ detag(%victim) @ "', weapon: '" @ %weapon @ "'.");
    T2Stats.recordKill(%type, %weapon, %killer.guid, detag(%killer.name), %victim.guid, detag(%victim.name));
}
Callback.add("KillCallback", t2stats_handleKillCallback);

// Handle system clock updates from the server
function t2stats_handleSystemClockCallback(%msgType, %msgString, %timelimit, %curTimeLeftMS) {
    t2stats_debugLog("Match clock update: " @ %curTimeLeftMS @ "ms / " @ %timelimit @ "min limit.");
    // Calculate the match start time
    T2Stats.matchTimeLimitMinutes = %timelimit;
    // If there's a time limit set, we can calculate the match start time
    if (%timelimit > 0) {
        %matchTimeElapsedMs = (%timelimit * 60 * 1000) - %curTimeLeftMS;
        T2Stats.matchStartTimeSimMs = getSimTime() - %matchTimeElapsedMs;
        rubyEval("tsEval 'T2Stats.matchStartTime=\"' + (Time.new - " @ (%matchTimeElapsedMs / 1000) @ ").getutc.to_s + '\";'");
        t2stats_debugLog("Match start time: " @ T2Stats.matchStartTime);
    } else {
        %matchTimeElapsedMs = 0;
        T2Stats.matchStartTimeSimMs = 0;
        T2Stats.matchStartTime = "";
    }
}
addMessageCallback( 'MsgSystemClock', t2stats_handleSystemClockCallback );

// We're ready :)
echo("T2Stats Activated.");