<Query Kind="Expression">
  <Connection>
    <ID>6f18b3ef-c788-4c6e-989a-e89a3cf435d3</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>T2Stats</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

Events
	.Select(e => new
		{
			e.MatchDuration,
			e.MatchGameType,
			e.MatchMapName,
			e.ServerIpAddress,
			e.ServerName,
			e.ServerPort,
			e.KillType,
			e.KillerName,
			e.KillerTribesGuid,
			e.VictimName,
			e.VictimTribesGuid,
			e.Weapon,
			count = Events.Count(ec => 
				ec.MatchDuration == e.MatchDuration &&
				ec.MatchGameType == e.MatchGameType &&
				ec.MatchMapName == e.MatchMapName &&
				ec.ServerIpAddress == e.ServerIpAddress &&
				ec.KillType == e.KillType &&
				ec.KillerName == e.KillerName &&
				ec.KillerTribesGuid == e.KillerTribesGuid &&
				ec.VictimName == e.VictimName &&
				ec.VictimTribesGuid == e.VictimTribesGuid &&
				ec.Weapon == e.Weapon &&
				Math.Abs(ec.MatchTime.TotalSeconds - e.MatchTime.TotalSeconds) < 7 &&
				ec.MatchTime < e.MatchTime
			)
		})
// Order by count, lop off the top result, kill others