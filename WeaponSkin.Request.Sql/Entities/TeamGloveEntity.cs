using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities;

/// <summary>
///     Database entity representing team-specific glove selections
/// </summary>
[SugarTable("ws_team_gloves")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}_{nameof(Team)}", 
    $"{nameof(SteamId)},{nameof(Team)}", OrderByType.Asc, IsUnique = true)]
public class TeamGloveEntity
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
