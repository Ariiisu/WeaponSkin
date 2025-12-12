using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.Listeners;
using Sharp.Shared.Objects;
using Sharp.Shared.Units;
using WeaponSkin.Shared;

namespace WeaponSkin.Managers;

internal interface IPlayerInfoManager
{
    EconItemId? GetPlayerKnife(IGameClient client, CStrikeTeam team);

    WeaponCosmetics? GetPlayerWeaponSkin(IGameClient client, EconItemId id);

    ushort? GetPlayerAgent(IGameClient client, CStrikeTeam team);

    ushort? GetPlayerMedal(IGameClient client, CStrikeTeam team);

    ushort? GetPlayerMusicKit(IGameClient client, CStrikeTeam team);

    EconGlovesId? GetPlayerGloves(IGameClient client, CStrikeTeam team);

    void RefreshInventory(IGameClient client);
}

internal class PlayerInfoManager : IPlayerInfoManager, IClientListener, IManager
{
    public int ListenerVersion  => IClientListener.ApiVersion;
    public int ListenerPriority => 0;

    private const int TEAM_CT_INDEX  = 0;
    private const int TEAM_TE_INDEX  = 1;
    private const int TEAM_MAX_COUNT = 2;

    private readonly InterfaceBridge            _bridge;
    private readonly ILogger<PlayerInfoManager> _logger;

    private readonly EconItemId?[,]      _playerKnives = new EconItemId?[PlayerSlot.MaxPlayerCount, TEAM_MAX_COUNT];
    private readonly EconGlovesId?[,]    _playerGloves = new EconGlovesId?[PlayerSlot.MaxPlayerCount, TEAM_MAX_COUNT];
    private readonly WeaponCosmetics[][] _weaponCosmetics;

    private readonly ushort?[,] _playerAgents    = new ushort?[PlayerSlot.MaxPlayerCount, TEAM_MAX_COUNT];
    private readonly ushort?[,] _playerMedals    = new ushort?[PlayerSlot.MaxPlayerCount, TEAM_MAX_COUNT];
    private readonly ushort?[,] _playerMusicKits = new ushort?[PlayerSlot.MaxPlayerCount, TEAM_MAX_COUNT];

    public PlayerInfoManager(InterfaceBridge bridge, ILogger<PlayerInfoManager> logger)
    {
        _bridge = bridge;
        _logger = logger;

        _weaponCosmetics = Enumerable.Repeat<WeaponCosmetics[]>([], PlayerSlot.MaxPlayerCount).ToArray();
    }

    public bool Init()
    {
        _bridge.ClientManager.InstallClientListener(this);

        return true;
    }

    public void Shutdown()
    {
        _bridge.ClientManager.RemoveClientListener(this);
    }

    public void OnClientPutInServer(IGameClient client)
    {
        if (client.IsFakeClient)
        {
            return;
        }

        ClearPlayerData(client.Slot);
        var steamId = client.SteamId;

        Task.Run(async () => await GetPlayerInventory(steamId).ConfigureAwait(false));
    }

    public void OnClientDisconnected(IGameClient client, NetworkDisconnectionReason reason)
    {
        ClearPlayerData(client.Slot);
    }

    public EconItemId? GetPlayerKnife(IGameClient client, CStrikeTeam team)
        => GetPlayerTeamItem(_playerKnives, client, team);

    public WeaponCosmetics? GetPlayerWeaponSkin(IGameClient client, EconItemId id)
    {
        var slot = client.Slot;

        var cosmetics = _weaponCosmetics[slot];

        foreach (var cosmetic in cosmetics)
        {
            if (cosmetic.ItemId == id)
            {
                return cosmetic;
            }
        }

        return null;
    }

    public ushort? GetPlayerAgent(IGameClient client, CStrikeTeam team)
        => GetPlayerTeamItem(_playerAgents, client, team);

    public ushort? GetPlayerMedal(IGameClient client, CStrikeTeam team)
        => GetPlayerTeamItem(_playerMedals, client, team);

    public ushort? GetPlayerMusicKit(IGameClient client, CStrikeTeam team)
        => GetPlayerTeamItem(_playerMusicKits, client, team);

    public EconGlovesId? GetPlayerGloves(IGameClient client, CStrikeTeam team)
        => GetPlayerTeamItem(_playerGloves, client, team);

    public void RefreshInventory(IGameClient client)
    {
        if (client.IsFakeClient)
        {
            return;
        }

        Task.Run(async () => await GetPlayerInventory(client.SteamId).ConfigureAwait(false));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T? GetPlayerTeamItem<T>(T?[,] itemArray, IGameClient client, CStrikeTeam team)
        where T : struct
    {
        var slot = client.Slot;

        return team switch
        {
            CStrikeTeam.CT => itemArray[slot, TEAM_CT_INDEX],
            CStrikeTeam.TE => itemArray[slot, TEAM_TE_INDEX],
            _              => null,
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssignItems(TeamItem[] source, EconItemId?[,] target, PlayerSlot slot)
    {
        foreach (var item in source)
        {
            if (item.Team == CStrikeTeam.CT)
            {
                target[slot, TEAM_CT_INDEX] = item.ItemId;
            }
            else if (item.Team == CStrikeTeam.TE)
            {
                target[slot, TEAM_TE_INDEX] = item.ItemId;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssignItems(TeamItem[] source, EconGlovesId?[,] target, PlayerSlot slot)
    {
        foreach (var item in source)
        {
            var val = (EconGlovesId) item.ItemId;

            if (item.Team == CStrikeTeam.CT)
            {
                target[slot, TEAM_CT_INDEX] = val;
            }
            else if (item.Team == CStrikeTeam.TE)
            {
                target[slot, TEAM_TE_INDEX] = val;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void AssignItems(TeamItem[] source, ushort?[,] target, PlayerSlot slot)
    {
        foreach (var item in source)
        {
            var val = (ushort) item.ItemId;

            if (item.Team == CStrikeTeam.CT)
            {
                target[slot, TEAM_CT_INDEX] = val;
            }
            else if (item.Team == CStrikeTeam.TE)
            {
                target[slot, TEAM_TE_INDEX] = val;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ClearPlayerData(PlayerSlot slot)
    {
        _weaponCosmetics[slot] = [];

        for (var i = 0; i < TEAM_MAX_COUNT; i++)
        {
            _playerKnives[slot, i]    = null;
            _playerGloves[slot, i]    = null;
            _playerAgents[slot, i]    = null;
            _playerMedals[slot, i]    = null;
            _playerMusicKits[slot, i] = null;
        }
    }

    private async Task GetPlayerInventory(SteamID steamId)
    {
        try
        {
            if (_bridge.GetRequestManager() is not { } request)
            {
                _logger.LogError("Failed to get IRequestManager, did you have WeaponSkin.Request module installed?");

                return;
            }

            var cosmetics = request.GetPlayerWeaponCosmetics(steamId);
            var knives    = request.GetPlayerTeamKnives(steamId);
            var gloves    = request.GetPlayerTeamGloves(steamId);
            var medals    = request.GetPlayerTeamMedals(steamId);
            var musicKits = request.GetPlayerTeamMusicKits(steamId);
            var agents    = request.GetPlayerTeamAgent(steamId);

            await Task.WhenAll(cosmetics, knives, gloves, medals, musicKits, agents).ConfigureAwait(false);

            await _bridge.ModSharp.InvokeFrameActionAsync(() =>
            {
                if (_bridge.ClientManager.GetGameClient(steamId) is { } target)
                {
                    _weaponCosmetics[target.Slot] = cosmetics.Result;
                    AssignItems(knives.Result,    _playerKnives,    target.Slot);
                    AssignItems(gloves.Result,    _playerGloves,    target.Slot);
                    AssignItems(medals.Result,    _playerMedals,    target.Slot);
                    AssignItems(musicKits.Result, _playerMusicKits, target.Slot);
                    AssignItems(agents.Result,    _playerAgents,    target.Slot);
                }
            }).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when requesting cosmetics for steamid {steamid}", steamId);
        }
    }
}