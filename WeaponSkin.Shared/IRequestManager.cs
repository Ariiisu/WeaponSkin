using Sharp.Shared.Units;

namespace WeaponSkin.Shared;

public interface IRequestManager
{
    const string Identitiy = "WeaponSkin.IRequestManager";

    Task<WeaponCosmetics[]> GetPlayerWeaponCosmetics(SteamID steamId);

    Task<TeamItem[]> GetPlayerTeamKnives(SteamID steamId);
    Task<TeamItem[]> GetPlayerTeamGloves(SteamID steamId);

    Task<TeamItem[]> GetPlayerTeamAgent(SteamID steamId);
    Task<TeamItem[]> GetPlayerTeamMusicKits(SteamID steamId);
    Task<TeamItem[]> GetPlayerTeamMedals(SteamID steamId);

    Task<Dictionary<string, int>> RunMigration();
}
