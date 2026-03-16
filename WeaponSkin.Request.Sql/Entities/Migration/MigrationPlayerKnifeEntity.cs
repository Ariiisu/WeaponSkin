using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities.Migration;

/// <summary>
///     Migration database entity for wp_player_knife table
///     Used for migrating data from other plugin schemas
/// </summary>
[SugarTable("wp_player_knife")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}_{nameof(WeaponTeam)}", 
    nameof(SteamId), OrderByType.Asc,
    nameof(WeaponTeam), OrderByType.Asc, IsUnique = true)]
public class MigrationPlayerKnifeEntity
{
    [SugarColumn(ColumnName = "steamid", Length = 18, IsNullable = false)]
    public string SteamId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "weapon_team", ColumnDataType = "int(1)", IsNullable = false)]
    public int WeaponTeam { get; set; }

    [SugarColumn(ColumnName = "knife", Length = 64, IsNullable = false)]
    public string Knife { get; set; } = string.Empty;
}
