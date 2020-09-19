using System.Windows;
using System.Windows.Controls;

namespace WereDev.Utils.Wu10Man.UserWindows.Models
{
    public class TabItemModel<T> : ITabItemModel<T>
        where T : UserControl, new()
    {
        private T _userControl = null;

        public string Header { get; set; }

        public string BackgroundColor { get; set; }

        public T UserControl
        {
            get
            {
                _userControl = _userControl ?? new T();
                return _userControl;
            }
        }

        public bool IsLegacy { get; set; } = false;
    }
}
