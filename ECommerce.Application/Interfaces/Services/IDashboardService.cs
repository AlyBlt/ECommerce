using ECommerce.Application.DTOs.Dashboard;
using ECommerce.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Interfaces.Services
{
    public interface IDashboardService
    {
        Task<DashboardStatsDTO> GetDashboardStatsAsync();
    }
}
