using System.ComponentModel;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public abstract class ModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void TriggerPropertyChanged(params string[] properties)
        {
            foreach (var property in properties)
            {
                TriggerPropertyChanged(property);
            }
        }

        private void TriggerPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
