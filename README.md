# Bulk Insert with NpgSql and ef core

## Notes:
- A regular add works just fine with Entity framework.
- With .NET 8, the NpgSql ef core provider switched from using an internal feature supporting jsonb to now use json capabilities built into entity framework
- Docs - https://www.npgsql.org/efcore/mapping/json.html?tabs=data-annotations%2Cpoco#tojson-owned-entity-mapping

