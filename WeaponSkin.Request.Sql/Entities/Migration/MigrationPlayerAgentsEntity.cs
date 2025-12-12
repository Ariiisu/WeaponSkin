using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities.Migration;

/// <summary>
///     Migration database entity for wp_player_agents table
///     Used for migrating data from other plugin schemas
/// </summary>
[SugarTable("wp_player_agents")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}", nameof(SteamId), OrderByType.Asc, IsUnique = true)]
public class MigrationPlayerAgentsEntity
{
    [SugarColumn(ColumnName = "steamid", Length = 18, IsNullable = false)]
    public string SteamId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "agent_ct", Length = 64, IsNullable = true)]
    public string? AgentCt { get; set; }

    [SugarColumn(ColumnName = "agent_t", Length = 64, IsNullable = true)]
    public string? AgentT { get; set; }
}
