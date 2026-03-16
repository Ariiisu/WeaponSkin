using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities.Migration;

/// <summary>
///     Migration database entity for wp_player_music table
///     Used for migrating data from other plugin schemas
/// </summary>
[SugarTable("wp_player_music")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}_{nameof(WeaponTeam)}", 
    nameof(SteamId), OrderByType.Asc,
    nameof(WeaponTeam), OrderByType.Asc, IsUnique = true)]
public class MigrationPlayerMusicEntity
{
    [SugarColumn(ColumnName = "steamid", Length = 64, IsNullable = false)]
    public string SteamId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "weapon_team", ColumnDataType = "int(1)", IsNullable = false)]
    public int WeaponTeam { get; set; }

    [SugarColumn(ColumnName = "music_id", ColumnDataType = "int(11)", IsNullable = false)]
    public int MusicId { get; set; }
}
