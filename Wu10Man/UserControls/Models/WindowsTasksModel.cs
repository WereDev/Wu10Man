using System.Linq;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class WindowsTasksModel : ModelBase
    {
        private WindowsTasksStatusModel[] _tasks = new WindowsTasksStatusModel[0];

        public bool AllTasksDisabled => !_tasks.Any(x => x.TaskExists && x.IsTaskEnabled);

        public string AllTasksButtonLabel => AllTasksDisabled ? "Enable All Tasks" : "Disable All Tasks";

        public WindowsTasksStatusModel[] Tasks
        {
            get
            {
                return _tasks;
            }

            set
            {
                _tasks = value ?? new WindowsTasksStatusModel[0];
                foreach (var service in _tasks)
                {
                    service.PropertyChanged += (sender, e) =>
                    {
                        TriggerPropertyChanged(nameof(AllTasksButtonLabel), nameof(AllTasksDisabled), nameof(Tasks));
                    };
                }

                TriggerPropertyChanged(nameof(AllTasksButtonLabel), nameof(AllTasksDisabled), nameof(Tasks));
            }
        }
    }
}
