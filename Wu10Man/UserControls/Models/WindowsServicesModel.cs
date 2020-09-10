using System.Linq;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class WindowsServicesModel : ModelBase
    {
        private WindowsServiceStatusModel[] _services = new WindowsServiceStatusModel[0];

        public bool AllServicesDisabled => !_services.Any(x => x.ServiceExists && x.IsServiceEnabled);

        public string AllServicesButtonLabel => AllServicesDisabled ? "Enable All Services" : "Disable All Services";

        public WindowsServiceStatusModel[] Services
        {
            get
            {
                return _services;
            }

            set
            {
                _services = value ?? new WindowsServiceStatusModel[0];
                foreach (var service in _services)
                {
                    service.PropertyChanged += WindowsServiceStatusModel_PropertyChanged;
                }

                OnPropertyChanged(nameof(WindowsServiceStatusModel));
                OnPropertyChanged(nameof(AllServicesButtonLabel));
                OnPropertyChanged(nameof(AllServicesDisabled));
            }
        }

        private void WindowsServiceStatusModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged(nameof(AllServicesButtonLabel));
            OnPropertyChanged(nameof(AllServicesDisabled));
        }
    }
}
