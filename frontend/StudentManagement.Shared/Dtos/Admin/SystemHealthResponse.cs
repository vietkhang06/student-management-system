namespace StudentManagement.Shared.Dtos.Admin;

public sealed class SystemHealthResponse
{
    public string DatabaseStatus { get; set; } = string.Empty;
    public string JvmVersion { get; set; } = string.Empty;
    public long TotalMemoryBytes { get; set; }
    public long FreeMemoryBytes { get; set; }
    public long UsedMemoryBytes { get; set; }
    public string OsName { get; set; } = string.Empty;
}
