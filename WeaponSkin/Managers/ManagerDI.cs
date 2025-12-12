using Microsoft.Extensions.DependencyInjection;

namespace WeaponSkin.Managers;

internal static class ManagerDI
{
    public static void AddManagerDi(this IServiceCollection services)
    {
        services.ImplSingleton<IPlayerInfoManager, IManager, PlayerInfoManager>();
    }
}