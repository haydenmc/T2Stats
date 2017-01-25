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

function processKillCallback(%type, %killer, %victim, %weapon, %i_die, %i_win, %suicide, %tk) {
    echo("Kill detected (" @ %type @ "): '" @ %killer @ "' killed '" @ %victim @ "', weapon: '" @ %weapon @ "'.");
    echo("VICTIM: " @ %victim.guid @ "/" @ %victim.name);
    echo("KILLER: " @ %killer.guid @ "/" @ %killer.name);
}

Callback.add("KillCallback", processKillCallback);
echo("T2 Stats Activated.");