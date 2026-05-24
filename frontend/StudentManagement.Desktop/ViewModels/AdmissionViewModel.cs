using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.HocSinh;
using StudentManagement.Shared.Dtos.Lop;

namespace StudentManagement.Desktop.ViewModels;

public sealed partial class AdmissionViewModel : ObservableObject
{
    private readonly IStudentApiClient _studentApiClient;
    private readonly IClassApiClient _classApiClient;
    private readonly IRegulationApiClient _regulationApiClient;
    private readonly IConfirmationService _confirmationService;

    // ── Form fields ────────────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _ten = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private DateTime? _ngaySinh = null;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _gioiTinh = "Nam";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _diaChi = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private string _email = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
    private LopResponse? _selectedClass;

    // ── State ──────────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _isSuccess;

    // ── Lookup data ────────────────────────────────────────────────────────
    public ObservableCollection<LopResponse> ClassList { get; } = new();

    // Regulation cache
    private int _tuoiToiThieu = 15;
    private int _tuoiToiDa = 20;
    private int _siSoToiDa = 40;

    [ObservableProperty]
    private string _regulationSummary = "• Đang tải quy định...";

    public AdmissionViewModel(
        IStudentApiClient studentApiClient,
        IClassApiClient classApiClient,
        IRegulationApiClient regulationApiClient,
        IConfirmationService confirmationService)
    {
        _studentApiClient = studentApiClient;
        _classApiClient = classApiClient;
        _regulationApiClient = regulationApiClient;
        _confirmationService = confirmationService;

        _ = LoadLookupDataAsync();
    }

    // ── Computed ───────────────────────────────────────────────────────────
    private bool CanSubmit =>
        !string.IsNullOrWhiteSpace(Ten) &&
        NgaySinh.HasValue &&
        !string.IsNullOrWhiteSpace(DiaChi) &&
        SelectedClass != null &&
        !IsBusy;

    // ── Commands ───────────────────────────────────────────────────────────
    [RelayCommand(CanExecute = nameof(CanSubmit))]
    private async Task Submit()
    {
        StatusMessage = string.Empty;
        IsSuccess = false;

        // ── Client-side age validation ─────────────────────────────────────
        var age = CalculateAge(NgaySinh!.Value);
        if (age < _tuoiToiThieu || age > _tuoiToiDa)
        {
            StatusMessage = $"Tuổi học sinh phải từ {_tuoiToiThieu} đến {_tuoiToiDa} tuổi (hiện tại: {age} tuổi).";
            return;
        }

        // ── Class capacity check ───────────────────────────────────────────
        if (SelectedClass!.SiSo >= _siSoToiDa)
        {
            StatusMessage = $"Lớp {SelectedClass.TenLop} đã đủ sĩ số tối đa ({_siSoToiDa} học sinh).";
            return;
        }

        // ── Safety Confirmation ────────────────────────────────────────────
        var confirmed = await _confirmationService.ConfirmActionAsync(
            "admission",
            $"Bạn có chắc chắn muốn tiếp nhận học sinh {Ten.Trim()} vào lớp {SelectedClass.TenLop} không?"
        );
        if (!confirmed) return;

        IsBusy = true;
        try
        {
            var request = new HocSinhCreateRequest
            {
                IdLop   = SelectedClass.IdLop,
                Ten     = Ten.Trim(),
                GioiTinh = GioiTinh,
                NgaySinh = NgaySinh,
                DiaChi  = DiaChi.Trim(),
                Email   = Email.Trim()
            };

            var created = await _studentApiClient.CreateAsync(request);

            IsSuccess = true;
            StatusMessage = $"✅ Tiếp nhận thành công! Mã học sinh: {created.IdHocSinh}";

            // Reset form
            ResetForm();

            // Reload class list to update SiSo count
            await RefreshClassListAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Lỗi: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void ResetForm()
    {
        Ten = string.Empty;
        NgaySinh = null;
        GioiTinh = "Nam";
        DiaChi = string.Empty;
        Email = string.Empty;
        SelectedClass = null;
        StatusMessage = string.Empty;
        IsSuccess = false;
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private async Task LoadLookupDataAsync()
    {
        IsBusy = true;
        try
        {
            // Load classes
            await RefreshClassListAsync();

            // Load regulations
            var regs = await _regulationApiClient.GetAllAsync();
            foreach (var reg in regs)
            {
                switch (reg.IdThamSo?.Trim().ToUpperInvariant())
                {
                    case "QD1_TUOI_TOI_THIEU":
                        if (int.TryParse(reg.GiaTriThamSo, out var min)) _tuoiToiThieu = min;
                        break;
                    case "QD1_TUOI_TOI_DA":
                        if (int.TryParse(reg.GiaTriThamSo, out var max)) _tuoiToiDa = max;
                        break;
                    case "QD2_SI_SO_TOI_DA":
                        if (int.TryParse(reg.GiaTriThamSo, out var cap)) _siSoToiDa = cap;
                        break;
                }
            }

            RegulationSummary = $"• Độ tuổi tiếp nhận: {_tuoiToiThieu} – {_tuoiToiDa} tuổi\n• Sĩ số tối đa mỗi lớp: {_siSoToiDa} học sinh\n• Hệ thống sẽ báo lỗi nếu lớp đã đủ sĩ số.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"⚠️ Không thể tải dữ liệu: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task RefreshClassListAsync()
    {
        var classes = await _classApiClient.GetAllAsync();
        ClassList.Clear();
        foreach (var c in classes)
            ClassList.Add(c);
    }

    private static int CalculateAge(DateTime dob)
    {
        var today = DateTime.Today;
        var age = today.Year - dob.Year;
        if (dob.Date > today.AddYears(-age)) age--;
        return age;
    }
}
