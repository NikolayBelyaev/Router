namespace Router.Executable.Admin.Models;

public class TargetViewModel
{
    public string Id { get; }
    public string Target { get; set; }
    public string Address { get; set; }
    public bool Maintenance { get; set; }
    public bool IsNew { get; set; }
    public bool ToBeDeleted { get; set; }

    // target name as received from the server; null for new targets
    public string OriginalTarget { get; }

    private string InitFingerprint { get; }

    public TargetViewModel(string target, string address, bool maintenance, bool isNew = false, bool toBeDeleted = false, string originalTarget = null)
    {
        Id = Guid.NewGuid().ToString();
        Target = target;
        Address = address;
        Maintenance = maintenance;
        IsNew = isNew;
        ToBeDeleted = toBeDeleted;
        OriginalTarget = originalTarget;
        InitFingerprint = GetFingerprint();
    }

    public bool IsModified()
    {
        return InitFingerprint != GetFingerprint();
    }

    private string GetFingerprint()
    {
        return string.Join("|", Id, Target, Address, Maintenance);
    }
}