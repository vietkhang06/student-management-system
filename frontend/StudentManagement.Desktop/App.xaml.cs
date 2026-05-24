using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using StudentManagement.Desktop.Services;
using StudentManagement.Desktop.ViewModels;
using StudentManagement.Desktop.Views;

namespace StudentManagement.Desktop;

public partial class App : Application
{
    private ServiceProvider? _serviceProvider;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();
        ConfigureServices(services);

        _serviceProvider = services.BuildServiceProvider();
        _serviceProvider.GetRequiredService<INavigationService>().ShowLogin();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _serviceProvider?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        var apiBaseUrl = Environment.GetEnvironmentVariable("STUDENT_MANAGEMENT_API_BASE_URL");
        var options = new ApiClientOptions
        {
            BaseAddress = Uri.TryCreate(apiBaseUrl, UriKind.Absolute, out var configuredUri)
                ? configuredUri
                : new Uri("http://localhost:8080/")
        };

        services.AddSingleton(options);
        services.AddSingleton<IUserSessionService, UserSessionService>();
        services.AddSingleton<INavigationService, WindowNavigationService>();
        services.AddSingleton<IConfirmationService, ConfirmationService>();

        services.AddTransient<AuthHeaderHandler>();

        services.AddHttpClient<IAuthApiClient, AuthApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IStudentApiClient, StudentApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IClassApiClient, ClassApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IRegulationApiClient, RegulationApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IScoreApiClient, ScoreApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ISubjectApiClient, SubjectApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ITermApiClient, TermApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<IReportApiClient, ReportApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        services.AddHttpClient<ISystemAdminApiClient, SystemAdminApiClient>((serviceProvider, client) =>
        {
            client.BaseAddress = serviceProvider.GetRequiredService<ApiClientOptions>().BaseAddress;
            client.Timeout = TimeSpan.FromSeconds(15);
        }).AddHttpMessageHandler<AuthHeaderHandler>();

        // ViewModels
        services.AddTransient<LoginViewModel>();
        services.AddSingleton<ShellViewModel>();
        services.AddTransient<DashboardViewModel>();
        services.AddTransient<StudentListViewModel>();
        services.AddTransient<ClassListViewModel>();
        services.AddTransient<ClassDetailViewModel>();
        services.AddTransient<PlaceholderViewModel>();
        services.AddTransient<AdmissionViewModel>();
        services.AddTransient<ScoreViewModel>();
        services.AddTransient<ReportViewModel>();
        services.AddTransient<RegulationViewModel>();
        services.AddTransient<SystemAdminViewModel>();

        // Views
        services.AddTransient<LoginView>();
        services.AddTransient<ShellWindow>();
        services.AddTransient<Views.DashboardView>();
        services.AddTransient<Views.StudentListView>();
        services.AddTransient<Views.ClassListView>();
        services.AddTransient<Views.ClassDetailView>();
        services.AddTransient<Views.PlaceholderView>();
        services.AddTransient<Views.AdmissionView>();
        services.AddTransient<Views.ScoreView>();
        services.AddTransient<Views.ReportView>();
        services.AddTransient<Views.RegulationView>();
        services.AddTransient<Views.SystemAdminView>();
    }
}
