using Microsoft.Extensions.Logging;
using Sharp.Shared.HookParams;
using WeaponSkin.Managers;

namespace WeaponSkin.Modules;

internal class PlayerMedal : IModule
{
    private readonly InterfaceBridge      _bridge;
    private readonly IPlayerInfoManager   _playerInfo;
    private readonly ILogger<PlayerMedal> _logger;

    public PlayerMedal(InterfaceBridge bridge, IPlayerInfoManager playerInfo, ILogger<PlayerMedal> logger)
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

        var controller = @params.Controller;

        if (controller.GetInventoryService() is not { } inventory)
        {
            return;
        }

        var pawn = @params.Pawn;

        if (_playerInfo.GetPlayerMedal(client, pawn.Team) is not { } medal
            || _bridge.EconItemManager.GetEconItemDefinitionByIndex(medal) is not { DefaultLoadoutSlot: 55 })
        {
            return;
        }

        var ranks = inventory.GetSchemaFixedArray<uint>("m_rank");
        ranks[5] = medal;
    }
}
