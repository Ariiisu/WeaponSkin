using Microsoft.Extensions.DependencyInjection;

namespace WeaponSkin.Modules;

internal static class ModuleDi
{
    public static void AddModuleDi(this IServiceCollection services)
    {
        services.AddSingleton<IModule, WeaponSkin>();
        services.AddSingleton<IModule, PlayerAgent>();
        services.AddSingleton<IModule, PlayerMedal>();
        services.AddSingleton<IModule, PlayerMusicKit>();
        services.AddSingleton<IModule, PlayerGloves>();
        services.AddSingleton<IModule, Migration>();
    }
}