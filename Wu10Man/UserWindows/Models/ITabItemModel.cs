using System.Windows.Controls;

namespace WereDev.Utils.Wu10Man.UserWindows.Models
{
    public interface ITabItemModel<out T>
            where T : UserControl, new()
    {
        string Header { get; }

        string BackgroundColor { get; }

        T UserControl { get; }

        bool IsLegacy { get; }
    }
}
