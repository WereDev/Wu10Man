using System;

namespace WereDev.Utils.Wu10Man.UserControls.Models
{
    class PauseUpdatesModel : ModelBase
    {
        private DateTime? _featureUpdatePauseDate = null;
        public DateTime? FeatureUpdatePauseDate
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

        private DateTime? _qualityUpdatePauseDate = null;
        public DateTime? QualityUpdatePauseDate
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
