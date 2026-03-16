using SqlSugar;

namespace WeaponSkin.Request.Sql.Entities.Migration;

/// <summary>
///     Migration database entity for wp_player_skins table
///     Used for migrating data from other plugin schemas
/// </summary>
[SugarTable("wp_player_skins")]
[SugarIndex($"unique_{{table}}_{nameof(SteamId)}_{nameof(WeaponTeam)}_{nameof(WeaponDefindex)}", 
    nameof(SteamId), OrderByType.Asc,
    nameof(WeaponTeam), OrderByType.Asc,
    nameof(WeaponDefindex), OrderByType.Asc, IsUnique = true)]
public class MigrationPlayerSkinsEntity
{
    [SugarColumn(ColumnName = "steamid", Length = 18, IsNullable = false)]
    public string SteamId { get; set; } = string.Empty;

    [SugarColumn(ColumnName = "weapon_team", ColumnDataType = "int(1)", IsNullable = false)]
    public int WeaponTeam { get; set; }

    [SugarColumn(ColumnName = "weapon_defindex", ColumnDataType = "int(6)", IsNullable = false)]
    public int WeaponDefindex { get; set; }

    [SugarColumn(ColumnName = "weapon_paint_id", ColumnDataType = "int(6)", IsNullable = false)]
    public int WeaponPaintId { get; set; }

    [SugarColumn(ColumnName = "weapon_wear", ColumnDataType = "float", IsNullable = false)]
    public float WeaponWear { get; set; } = 0.000001f;

    [SugarColumn(ColumnName = "weapon_seed", ColumnDataType = "int(16)", IsNullable = false)]
    public int WeaponSeed { get; set; } = 0;

    [SugarColumn(ColumnName = "weapon_nametag", Length = 128, IsNullable = true)]
    public string? WeaponNametag { get; set; }

    [SugarColumn(ColumnName = "weapon_stattrak", ColumnDataType = "tinyint(1)", IsNullable = false)]
    public bool WeaponStattrak { get; set; } = false;

    [SugarColumn(ColumnName = "weapon_stattrak_count", ColumnDataType = "int(10)", IsNullable = false)]
    public int WeaponStattrakCount { get; set; } = 0;

    [SugarColumn(ColumnName = "weapon_sticker_0", Length = 128, IsNullable = false)]
    public string WeaponSticker0 { get; set; } = "0;0;0;0;0;0;0";

    [SugarColumn(ColumnName = "weapon_sticker_1", Length = 128, IsNullable = false)]
    public string WeaponSticker1 { get; set; } = "0;0;0;0;0;0;0";

    [SugarColumn(ColumnName = "weapon_sticker_2", Length = 128, IsNullable = false)]
    public string WeaponSticker2 { get; set; } = "0;0;0;0;0;0;0";

    [SugarColumn(ColumnName = "weapon_sticker_3", Length = 128, IsNullable = false)]
    public string WeaponSticker3 { get; set; } = "0;0;0;0;0;0;0";

    [SugarColumn(ColumnName = "weapon_sticker_4", Length = 128, IsNullable = false)]
    public string WeaponSticker4 { get; set; } = "0;0;0;0;0;0;0";

    [SugarColumn(ColumnName = "weapon_keychain", Length = 128, IsNullable = false)]
    public string WeaponKeychain { get; set; } = "0;0;0;0;0";
}
