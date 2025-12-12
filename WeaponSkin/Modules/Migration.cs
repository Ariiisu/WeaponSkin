using System.Text;
using Microsoft.Extensions.Logging;
using Sharp.Shared.Enums;
using Sharp.Shared.Types;

namespace WeaponSkin.Modules;

internal class Migration : IModule
{
    private readonly InterfaceBridge    _bridge;
    private readonly ILogger<Migration> _logger;

    private const string CommandName = "ws_migrate";

    public Migration(InterfaceBridge bridge, ILogger<Migration> logger)
    {
        _bridge = bridge;
        _logger = logger;
    }

    public bool Init()
    {
        _bridge.ConVarManager.CreateServerCommand(CommandName, OnCommandMigrate);

        return true;
    }

    public void Shutdown()
    {
        _bridge.ConVarManager.ReleaseCommand(CommandName);
    }

    private ECommandAction OnCommandMigrate(StringCommand arg)
    {
        Task.Run(async () =>
        {
            try
            {
                if (_bridge.GetRequestManager() is not { } request)
                {
                    _logger.LogError("Failed to run migration, RequestManager is not found");

                    return;
                }

                var result = await request.RunMigration().ConfigureAwait(false);
                var summary = string.Join('\n', result.Select(row => $"Row: {row.Key} -- {row.Value} records"));

                _logger.LogInformation("Successfully migrated.\n{str}", summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to run migration.");
            }
        });

        return ECommandAction.Handled;
    }
}
