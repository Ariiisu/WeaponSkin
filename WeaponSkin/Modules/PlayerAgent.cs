using Microsoft.Extensions.Logging;
using Sharp.Shared.HookParams;
using Sharp.Shared.Types.Tier;
using WeaponSkin.Managers;

namespace WeaponSkin.Modules;

internal class PlayerAgent : IModule
{
    private readonly InterfaceBridge    _bridge;
    private readonly IPlayerInfoManager _playerInfo;

    private readonly ILogger<PlayerAgent> _logger;

    public PlayerAgent(InterfaceBridge bridge, IPlayerInfoManager playerInfo, ILogger<PlayerAgent> logger)
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

    private unsafe void OnPlayerSpawnPost(IPlayerSpawnForwardParams @params)
    {
        var client = @params.Client;

        if (client.IsFakeClient)
        {
            return;
        }

        var pawn = @params.Pawn;

        if (_playerInfo.GetPlayerAgent(client, pawn.Team) is not { } index
            || _bridge.EconItemManager.GetEconItemDefinitionByIndex(index) is not { DefaultLoadoutSlot: 38 } def)
        {
            return;
        }

        pawn.SetNetVar("m_nCharacterDefIndex", index);
        pawn.SetModel(def.BaseDisplayModel);

        var voPrefix = new CUtlString(*(byte**) (def.GetAbsPtr() + 0x3A8));

        var span = voPrefix.AsSpan();

        var hasFemaleVoice = !span.IsEmpty
                             && (span.IndexOf("_fem"u8) >= 0
                                 || span.SequenceEqual("fbihrt_epic"u8)
                                 || span.SequenceEqual("swat_epic"u8));

        pawn.SetNetVar("m_bHasFemaleVoice", hasFemaleVoice);
        pawn.SetNetVarUtlString("m_strVOPrefix", voPrefix.Get());
    }
}