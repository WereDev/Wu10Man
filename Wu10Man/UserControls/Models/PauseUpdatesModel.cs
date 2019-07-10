using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    class PauseUpdatesModel : ModelBase
    {
        private DateTimeOffset? _featureUpdatePauseDate = null;
        public DateTimeOffset? FeatureUpdatePauseDate
        {
            get
            {
                return _featureUpdatePauseDate;
            }
            set
            {
                _featureUpdatePauseDate = value;
                OnPropertyChanged(nameof(FeatureUpdatePauseDate));
            }
        }

        private int _featureUpdateDelayDays = 0;
        public int FeatureUpdateDelayDays
        {
            get
            {
                return _featureUpdateDelayDays;
            }
            set
            {
                _featureUpdateDelayDays = value;
                OnPropertyChanged(nameof(FeatureUpdateDelayDays));
            }
        }

        private DateTimeOffset? _qualityUpdatePauseDate = null;
        public DateTimeOffset? QualityUpdatePauseDate
        {
            get
            {
                return _qualityUpdatePauseDate;
            }
            set
            {
                _qualityUpdatePauseDate = value;
                OnPropertyChanged(nameof(QualityUpdatePauseDate));
            }
        }

        private int _qualityUpdateDelayDays = 0;
        public int QualityUpdateDelayDays
        {
            get
            {
                return _qualityUpdateDelayDays;
            }
            set
            {
                _qualityUpdateDelayDays = value;
                OnPropertyChanged(nameof(QualityUpdateDelayDays));
            }
        }

    }
}
