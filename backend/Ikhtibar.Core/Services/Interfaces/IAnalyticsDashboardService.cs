using System.Threading.Tasks;
using Ikhtibar.Shared.DTOs;

namespace Ikhtibar.Core.Services.Interfaces
{
    public interface IAnalyticsDashboardService
    {
        Task<ServiceResult<object>> ExecuteAsync(object request);
    }
}
