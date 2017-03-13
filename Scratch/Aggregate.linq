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
		.Select(e => 
			Events.Where(ev =>
				// Same event type
				ev.Discriminator == e.Discriminator &&
				// Same match
				ev.ServerIpAddress == e.ServerIpAddress &&
				ev.ServerPort == e.ServerPort &&
				SqlMethods.DateDiffSecond(ev.MatchStartTime, e.MatchStartTime) < 30 &&
				// Find nearby events
				Math.Abs(ev.MatchTime.TotalSeconds - e.MatchTime.TotalSeconds) < 7 &&
				//(ev.MatchTime > e.MatchTime || ev.EventId == e.EventId) &&
				// That are duplicate
				ev.VictimTribesGuid == e.VictimTribesGuid &&
				ev.KillerTribesGuid == e.KillerTribesGuid &&
				ev.KillType == e.KillType &&
				ev.Weapon == e.Weapon
			).OrderByDescending(ev => ev.MatchTime)
		)