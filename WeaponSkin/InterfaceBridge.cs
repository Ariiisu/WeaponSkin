using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Managers;
using Sharp.Shared.Objects;
using WeaponSkin.Shared;

namespace WeaponSkin;

internal interface IModule
{
    bool Init();

    void OnPostInit(ServiceProvider provider)
    {
    }

    void Shutdown()
    {
    }
}

internal interface IManager
{
    bool Init();

    void OnPostInit(ServiceProvider provider)
    {
    }

    void Shutdown();
}

internal class InterfaceBridge
{
    private readonly ISharedSystem _sharedSystem;

    public InterfaceBridge(string        dllPath,
                           string        sharpPath,
                           Version       version,
                           ISharedSystem sharedSystem,
                           bool          hotReload,
                           bool          debug)
    {
        DllPath       = dllPath;
        SharpPath     = sharpPath;
        Version       = version;
        _sharedSystem = sharedSystem;
        HotReload     = hotReload;
        Debug         = debug;

        ModSharp        = sharedSystem.GetModSharp();
        ConVarManager   = sharedSystem.GetConVarManager();
        EventManager    = sharedSystem.GetEventManager();
        ClientManager   = sharedSystem.GetClientManager();
        EntityManager   = sharedSystem.GetEntityManager();
        FileManager     = sharedSystem.GetFileManager();
        HookManager     = sharedSystem.GetHookManager();
        SchemaManager   = sharedSystem.GetSchemaManager();
        TransmitManager = sharedSystem.GetTransmitManager();
        SteamAPi        = ModSharp.GetSteamGameServer();

        ModuleManager   = sharedSystem.GetLibraryModuleManager();
        EconItemManager = sharedSystem.GetEconItemManager();

        PhysicsQueryManager = sharedSystem.GetPhysicsQueryManager();
        SoundManager        = sharedSystem.GetSoundManager();

        SharpModule = sharedSystem.GetSharpModuleManager();
    }

    public string DllPath { get; }

    public string SharpPath { get; }

    public Version Version { get; }

    public bool HotReload { get; }

    public bool Debug { get; }

    public IModSharp        ModSharp        { get; }
    public IConVarManager   ConVarManager   { get; }
    public IEventManager    EventManager    { get; }
    public IClientManager   ClientManager   { get; }
    public IEntityManager   EntityManager   { get; }
    public IEconItemManager EconItemManager { get; }
    public IFileManager     FileManager     { get; }
    public IHookManager     HookManager     { get; }
    public ISchemaManager   SchemaManager   { get; }
    public ITransmitManager TransmitManager { get; }
    public ISteamApi        SteamAPi        { get; }

    public IPhysicsQueryManager PhysicsQueryManager { get; }

    public ISoundManager         SoundManager  { get; }
    public ILibraryModuleManager ModuleManager { get; }

    private ISharpModuleManager SharpModule { get; }

    public IGameRules     GameRules     => ModSharp.GetGameRules();
    public IGlobalVars    GlobalVars    => ModSharp.GetGlobals();
    public INetworkServer Server        => ModSharp.GetIServer();
    public ILoggerFactory LoggerFactory => _sharedSystem.GetLoggerFactory();

    private IModSharpModuleInterface<IRequestManager>? _requsetInterface;

    public IRequestManager? GetRequestManager()
    {
        _requsetInterface ??= SharpModule.GetRequiredSharpModuleInterface<IRequestManager>(IRequestManager.Identitiy);

        if (_requsetInterface?.Instance is { } instance)
        {
            return instance;
        }

        return null;
    }
}
