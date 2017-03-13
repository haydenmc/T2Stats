<Query Kind="Statements">
  <Connection>
    <ID>6f18b3ef-c788-4c6e-989a-e89a3cf435d3</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>T2Stats</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

// Unique fields that denote matches
var t1 = Events
	.Select(e => new { e.MatchGameType, e.MatchMapName, e.MatchStartTime, e.ServerIpAddress, e.ServerPort} )
	.Distinct()
	.Where(e => e.ServerIpAddress == "208.100.45.135" && e.ServerPort == 28000);
	
// For each row...
t1
	.Select(te => new
	{
		// Find all within x # seconds
		matches = t1.Where(tee =>
			SqlMethods.DateDiffSecond(te.MatchStartTime, tee.MatchStartTime) < 15 &&
			SqlMethods.DateDiffSecond(te.MatchStartTime, tee.MatchStartTime) > -15
		).OrderBy(tee => tee.MatchStartTime),
		te.MatchStartTime,
	}
	)
	.Where(te =>
		te.matches.FirstOrDefault().MatchStartTime >= te.MatchStartTime
	).Select(te => new
	{
		te.matches.FirstOrDefault().MatchGameType,
		te.matches.FirstOrDefault().MatchMapName,
		te.MatchStartTime,
		te.matches.FirstOrDefault().ServerIpAddress,
		te.matches.FirstOrDefault().ServerPort
	})
	.Dump();

