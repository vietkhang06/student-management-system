using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.Diem;
using StudentManagement.Shared.Dtos.HocKy;
using StudentManagement.Shared.Dtos.Lop;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.ViewModels;

/// <summary>
/// One editable row in the score board (Biểu mẫu 4).
/// </summary>
public sealed partial class ScoreRowItem : ObservableObject
{
    // Identity (read-only)
    public string IdHocSinh { get; init; } = string.Empty;
    public string TenHocSinh { get; init; } = string.Empty;

    // Editable score fields
    [ObservableProperty]
    private string _diem15 = string.Empty;

    [ObservableProperty]
    private string _diem45 = string.Empty;

    // Computed average (read-only, updated by VM after save)
    [ObservableProperty]
    private string _diemTb = string.Empty;

    [ObservableProperty]
    private bool _isSaving;

    [ObservableProperty]
    private string _saveStatus = string.Empty;

    /// <summary>Returns parsed Diem15, or null if invalid.</summary>
    public decimal? ParsedDiem15 =>
        decimal.TryParse(Diem15.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;

    /// <summary>Returns parsed Diem45, or null if invalid.</summary>
    public decimal? ParsedDiem45 =>
        decimal.TryParse(Diem45.Replace(',', '.'),
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : null;
}

/// <summary>
/// ViewModel for the Score Management screen (Biểu mẫu 4).
/// </summary>
public sealed partial class ScoreViewModel : ObservableObject
{
    private readonly IScoreApiClient _scoreApiClient;
    private readonly IClassApiClient _classApiClient;
    private readonly ISubjectApiClient _subjectApiClient;
    private readonly ITermApiClient _termApiClient;
    private readonly IConfirmationService _confirmationService;
    private readonly IUserSessionService _userSessionService;

    // ── Filter selections ──────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadScoresCommand))]
    private LopResponse? _selectedClass;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadScoresCommand))]
    private MonHocResponse? _selectedSubject;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadScoresCommand))]
    private HocKyResponse? _selectedTerm;

    // ── State ──────────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool _isLoadingFilters;

    [ObservableProperty]
    private bool _isLoadingScores;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private bool _hasScores;

    [ObservableProperty]
    private bool _isReadOnlyScores = true;

    [ObservableProperty]
    private bool _canEditScores;

    // ── Lookup collections ─────────────────────────────────────────────────
    public ObservableCollection<LopResponse> Classes { get; } = new();
    public ObservableCollection<MonHocResponse> Subjects { get; } = new();
    public ObservableCollection<HocKyResponse> Terms { get; } = new();

    // ── Score rows ─────────────────────────────────────────────────────────
    public ObservableCollection<ScoreRowItem> ScoreRows { get; } = new();

    public ScoreViewModel(
        IScoreApiClient scoreApiClient,
        IClassApiClient classApiClient,
        ISubjectApiClient subjectApiClient,
        ITermApiClient termApiClient,
        IConfirmationService confirmationService,
        IUserSessionService userSessionService)
    {
        _scoreApiClient = scoreApiClient;
        _classApiClient = classApiClient;
        _subjectApiClient = subjectApiClient;
        _termApiClient = termApiClient;
        _confirmationService = confirmationService;
        _userSessionService = userSessionService;

        _ = LoadFiltersAsync();
    }

    // ── CanExecute ─────────────────────────────────────────────────────────
    private bool CanLoadScores =>
        SelectedClass != null && SelectedSubject != null && SelectedTerm != null && !IsLoadingScores;

    // ── Commands ───────────────────────────────────────────────────────────
    [RelayCommand(CanExecute = nameof(CanLoadScores))]
    private async Task LoadScores()
    {
        IsLoadingScores = true;
        StatusMessage = string.Empty;
        ScoreRows.Clear();
        HasScores = false;
        IsReadOnlyScores = true;

        try
        {
            var scores = await _scoreApiClient.GetByClassSubjectTermAsync(
                SelectedClass!.IdLop,
                SelectedSubject!.IdMonHoc,
                SelectedTerm!.IdHocKy);

            CanEditScores = CheckCanEditScores();
            IsReadOnlyScores = !CanEditScores;

            foreach (var s in scores)
            {
                ScoreRows.Add(new ScoreRowItem
                {
                    IdHocSinh  = s.IdHocSinh,
                    TenHocSinh = s.TenHocSinh,
                    Diem15     = s.Diem15.HasValue ? s.Diem15.Value.ToString("0.##") : string.Empty,
                    Diem45     = s.Diem45.HasValue ? s.Diem45.Value.ToString("0.##") : string.Empty,
                    DiemTb     = s.DiemTb.HasValue ? s.DiemTb.Value.ToString("0.##") : string.Empty,
                });
            }

            HasScores = ScoreRows.Count > 0;
            if (!HasScores)
                StatusMessage = "ℹ️ Không có dữ liệu điểm cho bộ lọc đã chọn.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Lỗi tải điểm: {ex.Message}";
        }
        finally
        {
            IsLoadingScores = false;
        }
    }

    private bool CheckCanEditScores()
    {
        var user = _userSessionService.CurrentUser;
        if (user == null) return false;

        if (user.IsBanQuanLy) return true;

        var profile = _userSessionService.Profile;
        if (profile == null) return false;

        if (SelectedClass == null || SelectedSubject == null || SelectedTerm == null) return false;

        var key = $"{SelectedClass.IdLop}_{SelectedSubject.IdMonHoc}_{SelectedTerm.IdHocKy}";
        return profile.PhanCongKeys != null && profile.PhanCongKeys.Contains(key);
    }

    [RelayCommand]
    private async Task SaveRow(ScoreRowItem row)
    {
        if (SelectedSubject == null || SelectedTerm == null) return;

        // Validate score range 0 – 10
        if (row.ParsedDiem15.HasValue && (row.ParsedDiem15 < 0 || row.ParsedDiem15 > 10))
        {
            row.SaveStatus = "❌ Điểm 15' phải từ 0 đến 10";
            return;
        }
        if (row.ParsedDiem45.HasValue && (row.ParsedDiem45 < 0 || row.ParsedDiem45 > 10))
        {
            row.SaveStatus = "❌ Điểm 1 tiết phải từ 0 đến 10";
            return;
        }

        // ── Safety Confirmation ────────────────────────────────────────────
        var confirmed = await _confirmationService.ConfirmActionAsync(
            "save_score",
            $"Bạn có chắc chắn muốn lưu điểm môn {SelectedSubject.TenMonHoc} của học sinh {row.TenHocSinh} không?"
        );
        if (!confirmed) return;

        row.IsSaving = true;
        row.SaveStatus = string.Empty;
        try
        {
            var request = new DiemCreateRequest
            {
                IdHocSinh = row.IdHocSinh,
                IdMonHoc  = SelectedSubject!.IdMonHoc,
                IdHocKy   = SelectedTerm!.IdHocKy,
                Diem15    = row.ParsedDiem15,
                Diem45    = row.ParsedDiem45,
            };

            var saved = await _scoreApiClient.SaveAsync(request);
            row.DiemTb    = saved.DiemTb.HasValue ? saved.DiemTb.Value.ToString("0.##") : string.Empty;
            row.Diem15    = saved.Diem15.HasValue ? saved.Diem15.Value.ToString("0.##") : string.Empty;
            row.Diem45    = saved.Diem45.HasValue ? saved.Diem45.Value.ToString("0.##") : string.Empty;
            row.SaveStatus = "✅";
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

    // ── Helpers ────────────────────────────────────────────────────────────
    private async Task LoadFiltersAsync()
    {
        IsLoadingFilters = true;
        try
        {
            var classTask   = _classApiClient.GetAllAsync();
            var subjectTask = _subjectApiClient.GetAllAsync();
            var termTask    = _termApiClient.GetAllAsync();

            await Task.WhenAll(classTask, subjectTask, termTask);

            foreach (var c in await classTask)   Classes.Add(c);
            foreach (var s in await subjectTask) Subjects.Add(s);
            foreach (var t in await termTask)    Terms.Add(t);
        }
        catch (Exception ex)
        {
            StatusMessage = $"⚠️ Không thể tải bộ lọc: {ex.Message}";
        }
        finally
        {
            IsLoadingFilters = false;
        }
    }
}
