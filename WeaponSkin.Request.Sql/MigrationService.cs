using System.Collections.Frozen;
using Sharp.Shared.Enums;
using Sharp.Shared.Managers;
using SqlSugar;
using WeaponSkin.Request.Sql.Entities;
using WeaponSkin.Request.Sql.Entities.Migration;

namespace WeaponSkin.Request.Sql;

internal class MigrationService
{
    private readonly ISqlSugarClient _db;

    private readonly FrozenDictionary<string, ushort> _agents;
    private readonly FrozenDictionary<string, ushort> _knives;

    public MigrationService(ISqlSugarClient db, IEconItemManager econItemManager)
    {
        _db = db;

        var items = econItemManager.GetEconItems();

        var agentsBySuffix = new Dictionary<string, ushort>();
        var knives         = new Dictionary<string, ushort>();

        foreach (var (index, definition) in items)
        {
            // agents
            if (definition.DefaultLoadoutSlot == 38)
            {
                var model = definition.BaseDisplayModel;
                agentsBySuffix[model.Split('/').Last()] = index;
            }
            else if (definition.ItemTypeName == "#CSGO_Type_Knife")
            {
                knives[definition.DefinitionName] = index;
            }
        }

        _agents = agentsBySuffix.ToFrozenDictionary();
        _knives = knives.ToFrozenDictionary();
    }

    private async Task<int> MigratePlayerSkins()
    {
        var legacyData  = await _db.Queryable<MigrationPlayerSkinsEntity>().ToListAsync();
        var newEntities = new List<WeaponCosmeticsEntity>();

        foreach (var legacy in legacyData)
        {
            if (!ulong.TryParse(legacy.SteamId, out var steamId))
            {
                continue;
            }

            newEntities.Add(new ()
            {
                SteamId        = steamId,
                ItemId         = legacy.WeaponDefindex,
                PaintId        = (ushort) legacy.WeaponPaintId,
                Wear           = legacy.WeaponWear,
                Seed           = legacy.WeaponSeed,
                StatTrak       = legacy.WeaponStattrak ? legacy.WeaponStattrakCount : null,
                NameTag        = legacy.WeaponNametag,
                WeaponSticker0 = legacy.WeaponSticker0,
                WeaponSticker1 = legacy.WeaponSticker1,
                WeaponSticker2 = legacy.WeaponSticker2,
                WeaponSticker3 = legacy.WeaponSticker3,
                WeaponSticker4 = legacy.WeaponSticker4,
                WeaponKeychain = legacy.WeaponKeychain,
            });
        }

        return await _db.Storageable(newEntities)
                        .ExecuteCommandAsync();
    }

    private async Task<int> MigratePlayerKnives()
    {
        var legacyData  = await _db.Queryable<MigrationPlayerKnifeEntity>().ToListAsync();
        var newEntities = new List<TeamKnifeEntity>();

        foreach (var legacy in legacyData)
        {
            if (!ulong.TryParse(legacy.SteamId, out var steamId))
            {
                continue;
            }

            if (!_knives.TryGetValue(legacy.Knife, out var indx))
            {
                continue;
            }

            newEntities.Add(new ()
            {
                SteamId = steamId,
                Team    = legacy.WeaponTeam,
                ItemId  = indx,
            });
        }

        return await _db.Storageable(newEntities)
                        .ExecuteCommandAsync();
    }

    private async Task<int> MigratePlayerGloves()
    {
        var legacyData  = await _db.Queryable<MigrationPlayerGlovesEntity>().ToListAsync();
        var newEntities = new List<TeamGloveEntity>();

        foreach (var legacy in legacyData)
        {
            if (!ulong.TryParse(legacy.SteamId, out var steamId))
            {
                continue;
            }

            newEntities.Add(new ()
            {
                SteamId = steamId,
                Team    = legacy.WeaponTeam,
                ItemId  = legacy.WeaponDefindex,
            });
        }

        return await _db.Storageable(newEntities)
                        .ExecuteCommandAsync();
    }

    private async Task<int> MigratePlayerAgents()
    {
        var legacyData  = await _db.Queryable<MigrationPlayerAgentsEntity>().ToListAsync();
        var newEntities = new List<TeamAgentEntity>();

        foreach (var legacy in legacyData)
        {
            if (!ulong.TryParse(legacy.SteamId, out var steamId))
            {
                continue;
            }

            // CT agent
            if (!string.IsNullOrEmpty(legacy.AgentCt)
                && _agents.TryGetValue(legacy.AgentCt, out var ctIndex))
            {
                newEntities.Add(new ()
                {
                    SteamId = steamId,
                    Team    = (int) CStrikeTeam.CT,
                    ItemId  = ctIndex,
                });
            }

            // T agent
            if (!string.IsNullOrEmpty(legacy.AgentT)
                && _agents.TryGetValue(legacy.AgentT, out var tIndex))
            {
                newEntities.Add(new ()
                {
                    SteamId = steamId,
                    Team    = (int) CStrikeTeam.TE,
                    ItemId  = tIndex,
                });
            }
        }

        return await _db.Storageable(newEntities)
                        .ExecuteCommandAsync();
    }

    private async Task<int> MigratePlayerMusic()
    {
        var legacyData  = await _db.Queryable<MigrationPlayerMusicEntity>().ToListAsync();
        var newEntities = new List<TeamMusicKitEntity>();

        foreach (var legacy in legacyData)
        {
            if (!ulong.TryParse(legacy.SteamId, out var steamId))
            {
                continue;
            }

            newEntities.Add(new ()
            {
                SteamId = steamId,
                Team    = legacy.WeaponTeam,
                ItemId  = legacy.MusicId,
            });
        }

        return await _db.Storageable(newEntities)
                        .ExecuteCommandAsync();
    }

    private async Task<int> MigratePlayerPins()
    {
        var legacyData  = await _db.Queryable<MigrationPlayerPinsEntity>().ToListAsync();
        var newEntities = new List<TeamMedalEntity>();

        foreach (var legacy in legacyData)
        {
            if (!ulong.TryParse(legacy.SteamId, out var steamId))
            {
                continue;
            }

            newEntities.Add(new ()
            {
                SteamId = steamId,
                Team    = legacy.WeaponTeam,
                ItemId  = legacy.Id,
            });
        }

        return await _db.Storageable(newEntities)
                        .ExecuteCommandAsync();
    }

    public async Task<Dictionary<string, int>> MigrateAll()
    {
        var results = new Dictionary<string, int>
        {
            ["Skins"]  = await MigratePlayerSkins(),
            ["Knives"] = await MigratePlayerKnives(),
            ["Gloves"] = await MigratePlayerGloves(),
            ["Agents"] = await MigratePlayerAgents(),
            ["Music"]  = await MigratePlayerMusic(),
            ["Pins"]   = await MigratePlayerPins(),
        };

        return results;
    }
}
