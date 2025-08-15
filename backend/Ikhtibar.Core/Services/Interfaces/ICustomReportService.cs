using System.Threading.Tasks;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface ICustomReportService
    {
        Task<ServiceResult<object>> ExecuteAsync(object request);
    }
}
