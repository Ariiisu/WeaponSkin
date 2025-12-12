using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.HookParams;
using WeaponSkin.Managers;

namespace WeaponSkin.Modules;

internal class PlayerGloves : IModule
{
    private readonly InterfaceBridge       _bridge;
    private readonly IPlayerInfoManager    _playerInfo;
    private readonly ILogger<PlayerGloves> _logger;

    public PlayerGloves(InterfaceBridge bridge, IPlayerInfoManager playerInfo, ILogger<PlayerGloves> logger)
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

        if (_playerInfo.GetPlayerGloves(client, pawn.Team) is not { } gloves
            || _playerInfo.GetPlayerWeaponSkin(client, (EconItemId) gloves) is not { } cosmetics)
        {
            return;
        }

        pawn.GiveGloves(gloves, cosmetics.PaintId, cosmetics.Wear, (int) cosmetics.Seed);
    }
}
