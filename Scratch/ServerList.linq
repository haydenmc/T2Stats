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
	.Select(e => new { e.ServerIpAddress, e.ServerPort }).Distinct()
	.Select(e => new
	{
		ServerName = Events
			.Where(ev => ev.ServerIpAddress == e.ServerIpAddress && ev.ServerPort == e.ServerPort)
			.OrderByDescending(ev => ev.MatchStartTime).OrderByDescending(ev => ev.MatchTime)
			.FirstOrDefault().ServerName,
		e.ServerIpAddress,
		e.ServerPort
	})