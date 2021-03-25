using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Bll.Gamification.Models;
using UserAvatar.Bll.Infrastructure;

namespace UserAvatar.Bll.Gamification.Services.Interfaces
{
    public interface IRateService
    {
        Task<Result<FullRateModel>> GetTopRate(int userId);
    }
}
