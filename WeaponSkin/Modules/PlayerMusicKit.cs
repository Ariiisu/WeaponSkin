using Microsoft.Extensions.Logging;
using Sharp.Shared.HookParams;
using WeaponSkin.Managers;

namespace WeaponSkin.Modules;

internal class PlayerMusicKit : IModule
{
    private readonly InterfaceBridge         _bridge;
    private readonly IPlayerInfoManager      _playerInfo;
    private readonly ILogger<PlayerMusicKit> _logger;

    public PlayerMusicKit(InterfaceBridge bridge, IPlayerInfoManager playerInfo, ILogger<PlayerMusicKit> logger)
    {
        _bridge     = bridge;
        _playerInfo = playerInfo;
        _logger     = logger;
    }

    public bool Init()
    {
        _bridge.HookManager.PlayerSpawnPost.InstallForward(OnPlayerSpawnPost);

        return true;
    }

    public void Shutdown()
    {
        _bridge.HookManager.PlayerSpawnPost.RemoveForward(OnPlayerSpawnPost);
    }

    private void OnPlayerSpawnPost(IPlayerSpawnForwardParams @params)
    {
        var client = @params.Client;

        if (client.IsFakeClient)
        {
            return;
        }

        var pawn = @params.Pawn;

        if (_playerInfo.GetPlayerMusicKit(client, pawn.Team) is not { } musicKit
            || _bridge.EconItemManager.GetEconItemDefinitionByIndex(musicKit) is not { DefaultLoadoutSlot: 55 })
        {
            return;
        }

        var controller = @params.Controller;

        if (controller.GetInventoryService() is { } inventory)
        {
            inventory.MusicId = musicKit;
        }
    }
}
