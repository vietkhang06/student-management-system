using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.ThamSo;

namespace StudentManagement.Desktop.ViewModels;

/// <summary>
/// One editable row in the regulations grid.
/// </summary>
public sealed partial class RegulationRowItem : ObservableObject
{
    // Read-only identity
    public string IdThamSo { get; init; } = string.Empty;
    public string TenThamSo { get; init; } = string.Empty;
    public int KieuThamSo { get; init; }   // 0 = number, 1 = text

    // Editable value
    [ObservableProperty]
    private string _giaTriThamSo = string.Empty;

    // UI state per row
    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _saveStatus = string.Empty;

    /// <summary>Human-readable type label.</summary>
    public string KieuLabel => KieuThamSo == 0 ? "Số" : "Văn bản";
}

/// <summary>
/// ViewModel for the Regulations (Quy định) management screen.
/// Only BANQUANLY can save; both roles can view.
/// </summary>
public sealed partial class RegulationViewModel : ObservableObject
{
    private readonly IRegulationApiClient _regulationApiClient;
    private readonly IConfirmationService _confirmationService;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    public ObservableCollection<RegulationRowItem> Regulations { get; } = new();

    public RegulationViewModel(
        IRegulationApiClient regulationApiClient,
        IConfirmationService confirmationService)
    {
        _regulationApiClient = regulationApiClient;
        _confirmationService = confirmationService;
        _ = LoadAsync();
    }

    [RelayCommand]
    private async Task Refresh() => await LoadAsync();

    [RelayCommand]
    private async Task SaveRow(RegulationRowItem row)
    {
        if (string.IsNullOrWhiteSpace(row.GiaTriThamSo))
        {
            row.SaveStatus = "❌ Giá trị không được để trống";
            return;
        }

        // Validate numeric type
        if (row.KieuThamSo == 0 && !double.TryParse(
                row.GiaTriThamSo.Replace(',', '.'),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out _))
        {
            row.SaveStatus = "❌ Giá trị phải là số";
            return;
        }

        // ── Safety Confirmation ────────────────────────────────────────────
        var confirmed = await _confirmationService.ConfirmActionAsync(
            "change_regulation",
            $"Bạn có chắc chắn muốn thay đổi quy định {row.TenThamSo} thành '{row.GiaTriThamSo.Trim()}' không?"
        );
        if (!confirmed) return;

        row.IsSaving = true;
        row.SaveStatus = string.Empty;
        try
        {
            var updated = await _regulationApiClient.UpdateAsync(
                row.IdThamSo,
                new ThamSoUpdateRequest { GiaTriThamSo = row.GiaTriThamSo.Trim() });

            row.GiaTriThamSo = updated.GiaTriThamSo;
            row.SaveStatus = "✅ Đã lưu";
        }
        catch (Exception ex)
        {
            row.SaveStatus = $"❌ {ex.Message}";
        }
        finally
        {
            row.IsSaving = false;
        }
    }

    private async Task LoadAsync()
    {
        IsLoading = true;
        StatusMessage = string.Empty;
        Regulations.Clear();
        try
        {
            var list = await _regulationApiClient.GetAllAsync();
            foreach (var r in list)
            {
                Regulations.Add(new RegulationRowItem
                {
                    IdThamSo    = r.IdThamSo,
                    TenThamSo   = r.TenThamSo,
                    KieuThamSo  = r.KieuThamSo,
                    GiaTriThamSo = r.GiaTriThamSo,
                });
            }

            if (Regulations.Count == 0)
                StatusMessage = "ℹ️ Chưa có quy định nào trong hệ thống.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Không thể tải quy định: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
