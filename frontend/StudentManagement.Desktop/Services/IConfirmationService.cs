using System.Threading.Tasks;

namespace StudentManagement.Desktop.Services;

public interface IConfirmationService
{
    Task<bool> ConfirmActionAsync(string actionKey, string message);
}
