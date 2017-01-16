using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Properties
{
    public class AirportReference
    {
        private string _airportCode;
        private string _airportName;
        private string _status;
        public AirportReference() : this(String.Empty, String.Empty, String.Empty)
        {
        }
        public AirportReference(string airportCode, string airportName, string statusCode)
        {
            _airportCode = airportCode;
            _airportName = airportName;
            switch (statusCode)
            {
                case "A":
                    _status = "Active";
                    break;
                case "I":
                    _status = "Inactive";
                    break;
            }
        }
        public string AirportCode
        {
            get { return _airportCode; }
            set { _airportCode = value; }
        }
        public string AirportName
        {
            get { return _airportName; }
            set { _airportName = value; }
        }

        public string SeletedLanguageCode { get; internal set; }

        public string Status
        {
            get { return _status; }
            set
            {
                switch (value)
                {
                    case "A":
                        _status = "Active";
                        break;
                    case "I":
                        _status = "Inactive";
                        break;
                }
            }
        }

    }
    public class AirportTranslation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public List<string> LanguageList { get; set; }
        private string _airportTranslationId;
        private string _languageName;
        private string _airportName;
        private string _selectedLanguageCode;
        private bool _isEnabled;


        public string AirportTranslationId
        {
            get { return _airportTranslationId; }
            set
            {
                _airportTranslationId = value;
                OnPropertyChanged("AirportTranslationId");
            }
        }
        public string AirportName
        {
            get { return _airportName; }
            set
            {
                _airportName = value;
                OnPropertyChanged("AirportName");
            }
        }
        public string LanguageName
        {
            get { return _languageName; }
            set
            {
                _languageName = value;
                OnPropertyChanged("LanguageName");
            }
        }
        public string SelectedLanguageCode
        {
            get { return _selectedLanguageCode; }
            set
            {
                _selectedLanguageCode = value;
                OnPropertyChanged("SelectedLanguageCode");
            }
        }
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled == value) return;
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }
        public AirportTranslation()
        {
            this._airportTranslationId = String.Empty;
            this._languageName = String.Empty;
            this._airportName = String.Empty;
            this.LanguageList = new List<string>();
            this._isEnabled = true;
        }
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
    public enum STATUS
    {
        NEW,
        EDIT
    }
    public class CustomComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }
        public override string ToString()
        {
            return Text;
        }
    }
}
