using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities;

/// <summary>
///     Database entity representing team-specific medal selections
/// </summary>
[SugarTable("ws_team_medals")]
[SugarIndex($"index_{{table}}_{nameof(SteamId)}", nameof(SteamId), OrderByType.Asc)]
[SugarIndex($"index_{{table}}_{nameof(Team)}",    nameof(Team),    OrderByType.Asc)]
public class TeamMedalEntity
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }

    [SugarColumn(IsNullable = false)]
    public ulong SteamId { get; set; }

    /// <summary>
    /// Team identifier (2 = T, 3 = CT)
    /// </summary>
    [SugarColumn(IsNullable = false)]
    public int Team { get; set; }

    [SugarColumn(IsNullable = false)]
    public int ItemId { get; set; }
}
