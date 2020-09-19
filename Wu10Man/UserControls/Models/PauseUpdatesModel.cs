using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    public class PauseUpdatesModel : ModelBase
    {
        private DateTime? _featureUpdatePauseDate = null;
        private int _featureUpdateDelayDays = 0;
        private DateTime? _qualityUpdatePauseDate = null;
        private int _qualityUpdateDelayDays = 0;

        public DateTime? FeatureUpdatePauseDate
        {
            get
            {
                return _featureUpdatePauseDate;
            }

            set
            {
                _featureUpdatePauseDate = value;
                TriggerPropertyChanged(nameof(FeatureUpdatePauseDate));
            }
        }

        public int FeatureUpdateDelayDays
        {
            get
            {
                return _featureUpdateDelayDays;
            }

            set
            {
                _featureUpdateDelayDays = value;
                TriggerPropertyChanged(nameof(FeatureUpdateDelayDays));
            }
        }

        public DateTime? QualityUpdatePauseDate
        {
            get
            {
                return _qualityUpdatePauseDate;
            }

            set
            {
                _qualityUpdatePauseDate = value;
                TriggerPropertyChanged(nameof(QualityUpdatePauseDate));
            }
        }

        public int QualityUpdateDelayDays
        {
            get
            {
                return _qualityUpdateDelayDays;
            }

            set
            {
                _qualityUpdateDelayDays = value;
                TriggerPropertyChanged(nameof(QualityUpdateDelayDays));
            }
        }
    }
}
