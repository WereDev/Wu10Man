using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class ProgressBarModel : ModelBase
    {
        private int _minValue = 0;
        private int _maxValue = 100;
        private int _currentValue = 25;

        public int MinValue
        {
            get
            {
                return _minValue;
            }

            set
            {
                _minValue = Math.Max(value, 0);
                TriggerPropertyChanged(nameof(MinValue), nameof(CurrentValue));
            }
        }

        public int MaxValue
        {
            get
            {
                return _maxValue;
            }

            set
            {
                _maxValue = Math.Max(value, _minValue);
                TriggerPropertyChanged(nameof(MaxValue), nameof(CurrentValue));
            }
        }

        public int CurrentValue
        {
            get
            {
                return _currentValue;
            }

            set
            {
                _currentValue = Math.Min(Math.Max(value, _minValue), _maxValue);
                TriggerPropertyChanged(nameof(CurrentValue));
            }
        }
    }
}
