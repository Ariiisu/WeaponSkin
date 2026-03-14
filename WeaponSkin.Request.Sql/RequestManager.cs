using Sharp.Shared.Enums;
using Sharp.Shared.Managers;
using Sharp.Shared.Units;
using SqlSugar;
using WeaponSkin.Request.Sql.Entities;
using WeaponSkin.Shared;

namespace WeaponSkin.Request.Sql;

internal class RequestManager : IRequestManager
{
    private readonly SqlSugarScope            _db;
    private readonly IEconItemManager?        _econItemManager;

    public RequestManager(string           connectionString,
                          DbType           dbType,
                          IEconItemManager econItemManager)
    {
        _econItemManager = econItemManager;

        _db = new (new ConnectionConfig
        {
            ConnectionString      = connectionString,
            DbType                = dbType,
            IsAutoCloseConnection = true,
            InitKeyType           = InitKeyType.Attribute,
            MoreSettings = new ()
            {
                DisableNvarchar = true,
            },
            LanguageType = LanguageType.English,
        });

        InitializeTables();
    }

    private void InitializeTables()
    {
        _db.CodeFirst.InitTables<WeaponCosmeticsEntity>();
        _db.CodeFirst.InitTables<TeamKnifeEntity>();
        _db.CodeFirst.InitTables<TeamGloveEntity>();
        _db.CodeFirst.InitTables<TeamAgentEntity>();
        _db.CodeFirst.InitTables<TeamMusicKitEntity>();
        _db.CodeFirst.InitTables<TeamMedalEntity>();
    }

    public void Dispose()
        => _db.Dispose();

    public async Task<Dictionary<string, int>> RunMigration()
    {
        if (_econItemManager == null)
        {
            throw new InvalidOperationException("IEconItemManager is required for migration but was not provided.");
        }

        if (_db.CurrentConnectionConfig.DbType != DbType.MySql)
        {
            throw new
                InvalidOperationException($"Migration only supports MySql. Current connection type: {_db.CurrentConnectionConfig.DbType}");
        }

        var migrationService = new MigrationService(_db, _econItemManager);

        return await migrationService.MigrateAll();
    }

    public async Task<WeaponCosmetics[]> GetPlayerWeaponCosmetics(SteamID steamId)
    {
        var steamIdValue = (ulong)steamId;

        var entities = await _db.Queryable<WeaponCosmeticsEntity>()
                                .Where(x => x.SteamId == steamIdValue)
                                .ToArrayAsync();

        return entities.Select(MapToWeaponCosmetics).ToArray();
    }

    public async Task<TeamItem[]> GetPlayerTeamKnives(SteamID steamId)
    {
        var steamIdValue = (ulong)steamId;

        var entities = await _db.Queryable<TeamKnifeEntity>()
                                .Where(x => x.SteamId == steamIdValue)
                                .Select(x => new
                                {
                                    x.Team,
                                    x.ItemId,
                                })
                                .ToArrayAsync();

        return entities.Select(e => new TeamItem { Team = (CStrikeTeam)e.Team, ItemId = (EconItemId)e.ItemId }).ToArray();
    }

    public async Task<TeamItem[]> GetPlayerTeamGloves(SteamID steamId)
    {
        var steamIdValue = (ulong)steamId;

        var entities = await _db.Queryable<TeamGloveEntity>()
                                .Where(x => x.SteamId == steamIdValue)
                                .Select(x => new
                                {
                                    x.Team,
                                    x.ItemId,
                                })
                                .ToArrayAsync();

        return entities.Select(e => new TeamItem { Team = (CStrikeTeam)e.Team, ItemId = (EconItemId)e.ItemId }).ToArray();
    }

    public async Task<TeamItem[]> GetPlayerTeamAgent(SteamID steamId)
    {
        var steamIdValue = (ulong)steamId;

        var entities = await _db.Queryable<TeamAgentEntity>()
                                .Where(x => x.SteamId == steamIdValue)
                                .Select(x => new
                                {
                                    x.Team,
                                    x.ItemId,
                                })
                                .ToArrayAsync();

        return entities.Select(e => new TeamItem { Team = (CStrikeTeam)e.Team, ItemId = (EconItemId)e.ItemId }).ToArray();
    }

    public async Task<TeamItem[]> GetPlayerTeamMusicKits(SteamID steamId)
    {
        var steamIdValue = (ulong)steamId;

        var entities = await _db.Queryable<TeamMusicKitEntity>()
                                .Where(x => x.SteamId == steamIdValue)
                                .Select(x => new
                                {
                                    x.Team,
                                    x.ItemId,
                                })
                                .ToArrayAsync();

        return entities.Select(e => new TeamItem { Team = (CStrikeTeam)e.Team, ItemId = (EconItemId)e.ItemId }).ToArray();
    }

    public async Task<TeamItem[]> GetPlayerTeamMedals(SteamID steamId)
    {
        var steamIdValue = (ulong)steamId;

        var entities = await _db.Queryable<TeamMedalEntity>()
                                .Where(x => x.SteamId == steamIdValue)
                                .Select(x => new
                                {
                                    x.Team,
                                    x.ItemId,
                                })
                                .ToArrayAsync();

        return entities.Select(e => new TeamItem { Team = (CStrikeTeam)e.Team, ItemId = (EconItemId)e.ItemId }).ToArray();
    }

    private static WeaponCosmetics MapToWeaponCosmetics(WeaponCosmeticsEntity entity)
    {
        var stickers = new Sticker?[5];
        stickers[0] = ParseSticker(entity.WeaponSticker0);
        stickers[1] = ParseSticker(entity.WeaponSticker1);
        stickers[2] = ParseSticker(entity.WeaponSticker2);
        stickers[3] = ParseSticker(entity.WeaponSticker3);
        stickers[4] = ParseSticker(entity.WeaponSticker4);

        var keychain = ParseKeychain(entity.WeaponKeychain);

        return new()
        {
            ItemId   = (EconItemId) entity.ItemId,
            PaintId  = entity.PaintId,
            Wear     = entity.Wear,
            Seed     = entity.Seed,
            StatTrak = entity.StatTrak,
            NameTag  = entity.NameTag ?? string.Empty,
            Stickers = stickers,
            Keychain = keychain,
        };
    }

    private static Sticker? ParseSticker(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var parts = value.Split(';');
        if (parts.Length != 7)
            return null;

        if (!int.TryParse(parts[0], out var stickerId) || stickerId == 0)
            return null;

        return new ()
        {
            StickerId = stickerId,
            Schema    = int.TryParse(parts[1], out var schema) ? schema : 0,
            OffsetX   = float.TryParse(parts[2], out var offsetX) ? offsetX : 0f,
            OffsetY   = float.TryParse(parts[3], out var offsetY) ? offsetY : 0f,
            Wear      = float.TryParse(parts[4], out var wear) ? wear : 0f,
            Scale     = float.TryParse(parts[5], out var scale) ? scale : 1f,
            Rotation  = float.TryParse(parts[6], out var rotation) ? rotation : 0f,
        };
    }

    private static Keychain? ParseKeychain(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var parts = value.Split(';');
        if (parts.Length != 5)
            return null;

        if (!int.TryParse(parts[0], out var keychainId) || keychainId == 0)
            return null;

        return new()
        {
            KeychainId = keychainId,
            X          = float.TryParse(parts[1], out var x) ? x : 0f,
            Y          = float.TryParse(parts[2], out var y) ? y : 0f,
            Z          = float.TryParse(parts[3], out var z) ? z : 0f,
            Seed       = float.TryParse(parts[4], out var seed) ? seed : 0f,
        };
    }
}
