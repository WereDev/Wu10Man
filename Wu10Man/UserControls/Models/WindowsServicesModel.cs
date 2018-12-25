namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class WindowsServicesModel : ModelBase
    {

        private WindowsServiceStatusModel[] _services = new WindowsServiceStatusModel[0];
        public WindowsServiceStatusModel[] Services
        {
            get
            {
                return _services;
            }
            set
            {
                _services = value ?? new WindowsServiceStatusModel[0];
                OnPropertyChanged(nameof(WindowsServiceStatusModel));
            }
        }
    }
}
