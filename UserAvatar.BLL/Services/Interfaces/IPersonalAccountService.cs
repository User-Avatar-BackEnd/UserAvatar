using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserAvatar.BLL.Services.Interfaces
{
    public interface IPersonalAccountService
    {
        void ChangePassword(int userId, string oldPassword, string newPassword);
        void ChangeLogin(int userId, string newLogin);
       
    }
}
