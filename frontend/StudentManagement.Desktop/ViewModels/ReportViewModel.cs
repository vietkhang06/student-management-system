using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StudentManagement.Desktop.Services;
using StudentManagement.Shared.Dtos.BaoCao;
using StudentManagement.Shared.Dtos.HocKy;
using StudentManagement.Shared.Dtos.Lop;
using StudentManagement.Shared.Dtos.MonHoc;

namespace StudentManagement.Desktop.ViewModels;

// ── Tab index enum ────────────────────────────────────────────────────────────
public enum ReportTab { SubjectReport = 0, SemesterReport = 1 }

// ── ViewModel ─────────────────────────────────────────────────────────────────
public sealed partial class ReportViewModel : ObservableObject
{
    private readonly IReportApiClient _reportApiClient;
    private readonly IClassApiClient _classApiClient;
    private readonly ISubjectApiClient _subjectApiClient;
    private readonly ITermApiClient _termApiClient;
    private readonly IUserSessionService _userSessionService;

    // ── Tab selection ─────────────────────────────────────────────────────────
    [ObservableProperty]
    private int _selectedTabIndex = 0;

    // ── Shared filters ────────────────────────────────────────────────────────
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadSubjectReportCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadSemesterReportCommand))]
    private LopResponse? _selectedClass;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadSubjectReportCommand))]
    private MonHocResponse? _selectedSubject;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(LoadSubjectReportCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadSemesterReportCommand))]
    private HocKyResponse? _selectedTerm;

    // ── Lookup data ───────────────────────────────────────────────────────────
    public ObservableCollection<LopResponse> Classes { get; } = new();
    public ObservableCollection<MonHocResponse> Subjects { get; } = new();
    public ObservableCollection<HocKyResponse> Terms { get; } = new();

    // ── State ─────────────────────────────────────────────────────────────────
    [ObservableProperty]
    private bool _isLoadingFilters;

    [ObservableProperty]
    private bool _isLoadingReport;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasStatusMessage))]
    private string _statusMessage = string.Empty;

    public bool HasStatusMessage => !string.IsNullOrEmpty(StatusMessage);

    // ── Subject Report results ────────────────────────────────────────────────
    [ObservableProperty]
    private bool _hasSubjectReport;

    [ObservableProperty]
    private string _subjectReportClass = string.Empty;

    [ObservableProperty]
    private string _subjectReportSubject = string.Empty;

    [ObservableProperty]
    private string _subjectReportTerm = string.Empty;

    [ObservableProperty]
    private int _subjectReportTongHocSinh;

    [ObservableProperty]
    private int _subjectReportSoLuongDat;

    [ObservableProperty]
    private string _subjectReportTyLe = string.Empty;

    // ── Semester Report results ───────────────────────────────────────────────
    [ObservableProperty]
    private bool _hasSemesterReport;

    [ObservableProperty]
    private string _semesterReportClass = string.Empty;

    [ObservableProperty]
    private string _semesterReportTerm = string.Empty;

    [ObservableProperty]
    private int _semesterReportTongHocSinh;

    [ObservableProperty]
    private int _semesterReportSoLuongDat;

    [ObservableProperty]
    private string _semesterReportTyLe = string.Empty;

    // ── Constructor ───────────────────────────────────────────────────────────
    public ReportViewModel(
        IReportApiClient reportApiClient,
        IClassApiClient classApiClient,
        ISubjectApiClient subjectApiClient,
        ITermApiClient termApiClient,
        IUserSessionService userSessionService)
    {
        _reportApiClient = reportApiClient;
        _classApiClient = classApiClient;
        _subjectApiClient = subjectApiClient;
        _termApiClient = termApiClient;
        _userSessionService = userSessionService;

        _ = LoadFiltersAsync();
    }

    // ── CanExecute ────────────────────────────────────────────────────────────
    private bool CanLoadSubjectReport =>
        SelectedClass != null && SelectedSubject != null && SelectedTerm != null && !IsLoadingReport;

    private bool CanLoadSemesterReport =>
        SelectedClass != null && SelectedTerm != null && !IsLoadingReport;

    // ── Commands ──────────────────────────────────────────────────────────────
    [RelayCommand(CanExecute = nameof(CanLoadSubjectReport))]
    private async Task LoadSubjectReport()
    {
        IsLoadingReport = true;
        StatusMessage = string.Empty;
        HasSubjectReport = false;
        try
        {
            var result = await _reportApiClient.GetSubjectReportAsync(
                SelectedClass!.IdLop,
                SelectedSubject!.IdMonHoc,
                SelectedTerm!.IdHocKy);

            SubjectReportClass   = result.TenLop;
            SubjectReportSubject = result.TenMonHoc;
            SubjectReportTerm    = SelectedTerm!.TenHocKy + " — " + SelectedTerm!.NamHoc;
            SubjectReportTongHocSinh  = result.TongHocSinh;
            SubjectReportSoLuongDat   = result.SoLuongDat;
            SubjectReportTyLe         = $"{result.TyLeDat:F1}%";
            HasSubjectReport = true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Lỗi: {ex.Message}";
        }
        finally
        {
            IsLoadingReport = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanLoadSemesterReport))]
    private async Task LoadSemesterReport()
    {
        IsLoadingReport = true;
        StatusMessage = string.Empty;
        HasSemesterReport = false;
        try
        {
            var result = await _reportApiClient.GetSemesterReportAsync(
                SelectedClass!.IdLop,
                SelectedTerm!.IdHocKy);

            SemesterReportClass        = result.TenLop;
            SemesterReportTerm         = SelectedTerm!.TenHocKy + " — " + SelectedTerm!.NamHoc;
            SemesterReportTongHocSinh  = result.TongHocSinh;
            SemesterReportSoLuongDat   = result.SoLuongDat;
            SemesterReportTyLe         = $"{result.TyLeDat:F1}%";
            HasSemesterReport = true;
        }
        catch (Exception ex)
        {
            StatusMessage = $"❌ Lỗi: {ex.Message}";
        }
        finally
        {
            IsLoadingReport = false;
        }
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private async Task LoadFiltersAsync()
    {
        IsLoadingFilters = true;
        try
        {
            var classTask   = _classApiClient.GetAllAsync();
            var subjectTask = _subjectApiClient.GetAllAsync();
            var termTask    = _termApiClient.GetAllAsync();

            await Task.WhenAll(classTask, subjectTask, termTask);

            var allClasses = await classTask;
            var allSubjects = await subjectTask;
            var allTerms = await termTask;

            // Populate Terms
            foreach (var t in allTerms)
            {
                Terms.Add(t);
            }

            // Populate Subjects
            foreach (var s in allSubjects)
            {
                Subjects.Add(s);
            }

            // Populate Classes based on role
            var user = _userSessionService.CurrentUser;
            var profile = _userSessionService.Profile;

            if (user == null || user.IsBanQuanLy)
            {
                foreach (var c in allClasses)
                {
                    Classes.Add(c);
                }
            }
            else
            {
                // Teacher: ONLY see their homeroom class
                if (profile != null && !string.IsNullOrEmpty(profile.ChuNhiemLopId))
                {
                    foreach (var c in allClasses)
                    {
                        if (string.Equals(c.IdLop, profile.ChuNhiemLopId, StringComparison.OrdinalIgnoreCase))
                        {
                            Classes.Add(c);
                            SelectedClass = c; // Auto-select homeroom class
                            break;
                        }
                    }
                }
            }
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
