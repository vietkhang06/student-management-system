using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudentManagement.Shared.Dtos.Admin;

namespace StudentManagement.Desktop.Services;

public interface IHistoryApiClient
{
    Task<List<HistoryDto>> SearchLogsAsync(
        string? keyword,
        string? hanhDong,
        DateTime? tuNgay,
        DateTime? denNgay,
        CancellationToken cancellationToken = default);
}
