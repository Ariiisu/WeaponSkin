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

    private readonly unsafe delegate* unmanaged<nint, byte, void> CCSPlayerPawn_SetGlovesBodyGroup;

    public PlayerGloves(InterfaceBridge bridge, IPlayerInfoManager playerInfo, ILogger<PlayerGloves> logger)
    {
        _bridge     = bridge;
        _playerInfo = playerInfo;
        _logger     = logger;

        unsafe
        {
            CCSPlayerPawn_SetGlovesBodyGroup = (delegate* unmanaged<IntPtr, byte, void>) GetSetGlovesBodyGroupAddress();

            _logger.LogInformation("CCSPlayerPawn_SetGlovesBodyGroup 0x{addr:X}", (nint) CCSPlayerPawn_SetGlovesBodyGroup);
        }
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

        unsafe
        {
            if (CCSPlayerPawn_SetGlovesBodyGroup is null)
            {
                _logger.LogWarning("CCSPlayerPawn_SetGlovesBodyGroup is null, gloves changer may not work properly");

                return;
            }

            CCSPlayerPawn_SetGlovesBodyGroup(pawn.GetAbsPtr(), 0);
        }
    }

    private nint GetSetGlovesBodyGroupAddress()
    {
        var server  = _bridge.ModuleManager.Server;

        var first_or_third_personToken = _bridge.ModSharp.MakeStringToken("first_or_third_person");

        var tokenBytes   = BitConverter.GetBytes(first_or_third_personToken);
        var tokenAddress = server.FindData(tokenBytes, false);

        if (tokenAddress == nint.Zero)
        {
            _logger.LogWarning("Cannot find address with first_or_third_person_token(0x{token:X})", first_or_third_personToken);

            return nint.Zero;
        }

        var                                  tokenRefs = server.GetReferencesFromPointer(tokenAddress);
        HashSet<(nint @ref, nint funcStart)> sets      = [];

        foreach (var @ref in tokenRefs)
        {
            if (server.GetFunctionRange(@ref, out var start, out _))
            {
                var (isRead, isWritten) = MemoryUtilities.AnalyzeInstructionAccess(@ref);

                // we only care about read only access
                if (isRead && !isWritten)
                {
                    sets.Add((@ref, start));
                }
            }
        }

        if (sets.Count != 1)
        {
            _logger.LogWarning("Expected one function that references first_or_third_person_token but got {size}", sets.Count);

            return nint.Zero;
        }

        return sets.First().@ref;
    }
}
