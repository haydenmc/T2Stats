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
// ---------------------------------------------------------------------------

function t2stats_processKillCallback(%type, %killer, %victim, %weapon, %i_die, %i_win, %suicide, %tk) {
    echo("Kill detected (" @ %type @ "): '" @ %killer @ "' killed '" @ %victim @ "', weapon: '" @ %weapon @ "'.");
    echo("VICTIM: " @ %victim.guid @ "/" @ %victim.name);
    echo("KILLER: " @ %killer.guid @ "/" @ %killer.name);
}

function StatsConnection::onDNSFailed(%this)
{
    error("StatsConnection DNS lookup failed.");
}

function StatsConnection::onConnectFailed(%this)
{
    error("StatsConnection connection failed.");
}

function StatsConnection::onDNSResolved(%this)
{
    echo("StatsConnection DNS resolved.");
}

function StatsConnection::onConnected(%this)
{
    echo("StatsConnection connected.");
    //%this.send(%this.data);
}

function StatsConnection::onDisconnect(%this)
{
    echo("StatsConnection disconnected.");
    %this.delete();
}

function StatsConnection::onLine(%this, %line)
{
    echo( "StatsConnection line received: " @ %line );
}

function t2stats_reportKill() {
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
    %matchMapName = "Katabatic";
    %matchServerIpAddress = "192.168.0.12";
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
        "Host: localhost:5000\r\n" @
        "User-Agent: Tribes2\r\n" @
        "Content-Type: application/json\r\n" @
        "Content-Length: " @ strlen(%postBody) @ "\r\n" @
        "Connection: close\r\n" @
        "\r\n" @ %postBody;
    
    %connection = new TCPObject(StatsConnection);
    %connection.data = %data;
    %connection.connect("localhost:5000");
}

Callback.add("KillCallback", t2stats_processKillCallback);
echo("T2 Stats Activated.");