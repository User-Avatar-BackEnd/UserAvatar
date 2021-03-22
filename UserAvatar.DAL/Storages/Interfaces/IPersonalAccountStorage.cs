using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserAvatar.Dal.Entities;

namespace UserAvatar.DAL.Storages.Interfaces
{
    public interface IPersonalAccountStorage
    {
        void Update(User user);
    }
}
