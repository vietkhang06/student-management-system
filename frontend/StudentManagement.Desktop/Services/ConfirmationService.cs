using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using StudentManagement.Desktop.Views;

namespace StudentManagement.Desktop.Services;

public sealed class ConfirmationService : IConfirmationService
{
    private readonly IUserSessionService _userSessionService;
    private readonly string _settingsFilePath;
    private Dictionary<string, bool> _doNotAskSettings = new();

    public ConfirmationService(IUserSessionService userSessionService)
    {
        _userSessionService = userSessionService;
        
        var appDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudentManagement"
        );

        if (!Directory.Exists(appDataFolder))
        {
            Directory.CreateDirectory(appDataFolder);
        }

        _settingsFilePath = Path.Combine(appDataFolder, "confirmations.json");
        LoadSettings();
    }

    public Task<bool> ConfirmActionAsync(string actionKey, string message)
    {
        var username = _userSessionService.CurrentUser?.TenDangNhap ?? "default";
        var userActionKey = $"{username}_{actionKey}";

        // If the user already checked "Do not ask again" and confirmed previously
        if (_doNotAskSettings.TryGetValue(userActionKey, out var doNotAsk) && doNotAsk)
        {
            return Task.FromResult(true);
        }

        var tcs = new TaskCompletionSource<bool>();

        // Ensure we run on UI Thread
        Application.Current.Dispatcher.Invoke(() =>
        {
            try
            {
                var dialog = new ConfirmationWindow(message)
                {
                    Owner = Application.Current.MainWindow
                };

                var result = dialog.ShowDialog();
                if (result == true && dialog.UserConfirmed)
                {
                    if (dialog.DoNotAskAgain)
                    {
                        _doNotAskSettings[userActionKey] = true;
                        SaveSettings();
                    }
                    tcs.SetResult(true);
                }
                else
                {
                    tcs.SetResult(false);
                }
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });

        return tcs.Task;
    }

    private void LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                var json = File.ReadAllText(_settingsFilePath);
                _doNotAskSettings = JsonSerializer.Deserialize<Dictionary<string, bool>>(json) 
                                    ?? new Dictionary<string, bool>();
            }
        }
        catch
        {
            _doNotAskSettings = new Dictionary<string, bool>();
        }
    }

    private void SaveSettings()
    {
        try
        {
            var json = JsonSerializer.Serialize(_doNotAskSettings, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_settingsFilePath, json);
        }
        catch
        {
            // Ignored
        }
    }
}
