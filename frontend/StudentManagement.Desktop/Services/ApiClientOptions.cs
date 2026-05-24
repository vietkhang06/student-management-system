namespace StudentManagement.Desktop.Services;

public sealed class ApiClientOptions
{
    public Uri BaseAddress { get; init; } = new("http://localhost:8080/");
}
