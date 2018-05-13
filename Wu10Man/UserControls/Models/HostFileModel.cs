using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class HostFileModel : ModelBase
    {
        public HostFileModel()
        {
            Urls = new List<HostFileItem>();
        }

        public List<HostFileItem> Urls { get; }
    }
}
