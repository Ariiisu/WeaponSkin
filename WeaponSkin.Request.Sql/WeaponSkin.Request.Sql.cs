using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sharp.Shared;
using Sharp.Shared.Units;
using SqlSugar;
using WeaponSkin.Shared;

namespace WeaponSkin.Request.Sql;

public class WeaponSkinRequest : IModSharpModule, IRequestManager
{
    private readonly ISharedSystem              _sharedSystem;
    private readonly string                     _dllPath;
    private readonly string                     _sharpPath;
    private readonly ILogger<WeaponSkinRequest> _logger;

    private RequestManager? _requestManager;

    public WeaponSkinRequest(
        ISharedSystem  sharedSystem,
        string         dllPath,
        string         sharpPath,
        Version        version,
        IConfiguration coreConfiguration,
        bool           hotReload)
    {
        _sharedSystem = sharedSystem;
        _dllPath      = dllPath;
        _sharpPath    = sharpPath;

        var loggerFactory = _sharedSystem.GetLoggerFactory();

        _logger = loggerFactory.CreateLogger<WeaponSkinRequest>();
    }

    private const string DefaultMySqlPort      = "3306";
    private const string DefaultPostgreSqlPort = "5432";

    public bool Init()
    {
        var econItemManager = _sharedSystem.GetEconItemManager();

        try
        {
            var config     = LoadConfiguration();
            var dbTypeEnum = ParseDatabaseType(config);

            var connectionString = BuildConnectionString(config, dbTypeEnum);

            _requestManager = new (connectionString, dbTypeEnum, econItemManager);

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize: {msg}", ex.Message);

            return false;
        }
    }

    public void PostInit()
    {
        _sharedSystem.GetSharpModuleManager().RegisterSharpModuleInterface<IRequestManager>(this,
                 IRequestManager.Identitiy,
                 this);
    }

    public void Shutdown()
    {
        _requestManager?.Dispose();
    }

    private IConfigurationRoot LoadConfiguration()
    {
        var configPath = Path.Combine(Path.GetFullPath(_sharpPath), "configs");

        return new ConfigurationBuilder()
               .SetBasePath(configPath)
               .AddJsonFile("weaponskin.jsonc", false, false)
               .Build();
    }

    private static DbType ParseDatabaseType(IConfiguration config)
    {
        var dbType = config["Database:Type"] ?? "MySql";

        return Enum.Parse<DbType>(dbType, true);
    }

    private string BuildSqliteConnectionString(IConfiguration config)
    {
        var dbFileName = config["Database:Database"] ?? "weaponskin.db";

        if (string.IsNullOrEmpty(Path.GetExtension(dbFileName)))
        {
            dbFileName += ".db";
        }

        var dataDirectory = Path.Combine(_sharpPath, "data");
        Directory.CreateDirectory(dataDirectory);

        var dbPath = Path.Combine(dataDirectory, dbFileName);
        _logger.LogInformation("Using SQLite database: {path}", dbPath);

        return $"Data Source={dbPath}";
    }

    private string BuildConnectionString(IConfiguration config, DbType dbType)
    {
        if (dbType == DbType.Sqlite)
        {
            return BuildSqliteConnectionString(config);
        }

        var host     = config["Database:Host"];
        var port     = config["Database:Port"];
        var database = config["Database:Database"];
        ValidateRequiredFields(host, database, config, out var user, out var password);

        var connectionString = BuildConnectionStringByType(dbType, host, port, database, user, password);

        var displayServer = port != null ? $"{host}:{port}" : host;
        _logger.LogInformation("Using {dbType} database: {server}/{database}", dbType, displayServer, database);

        return connectionString;
    }

    private static void ValidateRequiredFields(
        string?        host,
        string?        database,
        IConfiguration config,
        out string?    user,
        out string?    password)
    {
        var missingFields = new List<string>();

        if (string.IsNullOrWhiteSpace(host))
        {
            missingFields.Add("Database:Host");
        }

        if (string.IsNullOrWhiteSpace(database))
        {
            missingFields.Add("Database:Database");
        }

        user     = config["Database:User"];
        password = config["Database:Password"];

        if (string.IsNullOrWhiteSpace(user))
        {
            missingFields.Add("Database:User");
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            missingFields.Add("Database:Password");
        }

        if (missingFields.Count > 0)
        {
            var errorMessage = $"Missing required database configuration fields: {string.Join(", ", missingFields)}";

            throw new InvalidOperationException(errorMessage);
        }
    }

    private static string BuildConnectionStringByType(
        DbType  dbType,
        string? host,
        string? port,
        string? database,
        string? user,
        string? password)
    {
        return dbType switch
        {
            DbType.MySql =>
                $"Server={host};Port={port ?? DefaultMySqlPort};Database={database};User={user};Password={password};",

            DbType.PostgreSQL =>
                $"Host={host};Port={port ?? DefaultPostgreSqlPort};Database={database};Username={user};Password={password};",

            _ => throw new
                NotSupportedException($"Database type '{dbType}' is not supported. Currently supported types: MySql, PostgreSQL, SqlServer"),
        };
    }

    string IModSharpModule.DisplayName   => "[WeaponSkin] SQL Request";
    string IModSharpModule.DisplayAuthor => "Nukoooo";

    public Task<WeaponCosmetics[]> GetPlayerWeaponCosmetics(SteamID steamId)
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.GetPlayerWeaponCosmetics(steamId);

    public Task<TeamItem[]> GetPlayerTeamKnives(SteamID steamId)
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.GetPlayerTeamKnives(steamId);

    public Task<TeamItem[]> GetPlayerTeamGloves(SteamID steamId)
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.GetPlayerTeamGloves(steamId);

    public Task<TeamItem[]> GetPlayerTeamAgent(SteamID steamId)
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.GetPlayerTeamAgent(steamId);

    public Task<TeamItem[]> GetPlayerTeamMusicKits(SteamID steamId)
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.GetPlayerTeamMusicKits(steamId);

    public Task<TeamItem[]> GetPlayerTeamMedals(SteamID steamId)
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.GetPlayerTeamMedals(steamId);

    public Task<Dictionary<string, int>> RunMigration()
        => _requestManager == null
            ? throw new InvalidOperationException("RequestManager not initialized")
            : _requestManager.RunMigration();
}
