using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereDev.Utils.Wu10Man.Interfaces
{
    public interface ITokenEditor
    {
        bool AddPrivilege(string privilege);

        bool RemovePrivilege(string privilege);
    }
}
