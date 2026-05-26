using Router.Core.Model.Configuration;
using Router.Executable.Admin.Enums;
using Router.Shared.Router;

namespace Router.Executable.Admin.Models;

public class RoutingRule
{
    public string Id { get; set; }
    public ClientBuildVersion? Version { get; set; }
    public List<ClientPlatform> Platforms { get; set; }
    public string Target { get; set; }
    public UpdateMode UpdateMode { get; set; }
    public bool IsNew { get; set; }
    public bool ToBeDeleted { get; set; }

    // platform → server-assigned GUID; empty for locally created rules
    public Dictionary<ClientPlatform, string> ServerIds { get; set; } = [];

    private string InitFingerprint { get; set; }
    private string InitPropertyFingerprint { get; set; }

    public RoutingRule(
        ClientBuildVersion version,
        List<ClientPlatform> platforms,
        string target,
        UpdateMode updateMode,
        bool isNew = false,
        Dictionary<ClientPlatform, string> serverIds = null
    )
    {
        Id = Guid.NewGuid().ToString();
        Version = version;
        Platforms = platforms ?? [];
        Target = target;
        UpdateMode = updateMode;
        IsNew = isNew;
        ToBeDeleted = false;
        ServerIds = serverIds ?? [];
        InitFingerprint = GetFingerprint();
        InitPropertyFingerprint = GetPropertyFingerprint();
    }

    public RoutingRule(string target)
    {
        Id = Guid.NewGuid().ToString();
        Version = new ClientBuildVersion(0, 0, 0);
        Platforms = [];
        Target = target;
        UpdateMode = UpdateMode.UpToDate;
        IsNew = true;
        ServerIds = [];
        InitFingerprint = GetFingerprint();
        InitPropertyFingerprint = GetPropertyFingerprint();
    }

    public bool IsModified()
    {
        return InitFingerprint != GetFingerprint();
    }

    // True if target or updateMode changed (version is immutable for existing rules)
    public bool HasPropertyChanges()
    {
        return InitPropertyFingerprint != GetPropertyFingerprint();
    }

    private string GetFingerprint()
    {
        var platformsFingerprint = Platforms == null ? string.Empty : string.Join(",", Platforms.Select(p => p.ToString()));
        return string.Join("|", Id, Version?.ToString(), platformsFingerprint, Target, UpdateMode.ToString());
    }

    private string GetPropertyFingerprint()
    {
        return string.Join("|", Target, UpdateMode.ToString());
    }
}