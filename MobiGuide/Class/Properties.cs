using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExtensions;

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
    public abstract class LanguageTranslation : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Guid _translationId;
        private string _languageCode;
        private List<string> _languageList;
        private string _languageName;
        private string _display;
        private bool _isEnabled;

        public Guid TranslationId
        {
            get { return _translationId; }
            set
            {
                _translationId = value;
                OnPropertyChanged("TranslationId");
            }
        }
        public string Display
        {
            get { return _display; }
            set
            {
                _display = value;
                OnPropertyChanged("Display");
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
        public string LanguageCode
        {
            get { return _languageCode; }
            set
            {
                _languageCode = value;
                OnPropertyChanged("LanguageCode");
            }
        }
        public List<string> LanguageList
        {
            get { return _languageList; }
            set
            {
                _languageList = value;
                OnPropertyChanged("LanguageList");
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
        public LanguageTranslation()
        {
            this._translationId = Guid.Empty;
            this._languageName = String.Empty;
            this._display = String.Empty;
            this._languageList = new List<string>();
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
    public class AirportTranslation : LanguageTranslation
    {
        public AirportTranslation() : base() { }
        public Guid AirportTranslationId
        {
            get { return base.TranslationId; }
            set
            {
                base.TranslationId = value;
            }
        }
        public string AirportName
        {
            get { return base.Display; }
            set
            {
                base.Display = value;
            }
        }
    }
    public class TextTranslation : LanguageTranslation
    {
        private Guid _textTemplateId;
        public Guid TextTemplateId
        {
            get { return this._textTemplateId; }
            set
            {
                this._textTemplateId = value;
                OnPropertyChanged("TextTemplateId");
            }
        }
        public Guid TextTranslationId
        {
            get { return base.TranslationId; }
            set
            {
                base.TranslationId = value;
            }
        }
        public string TextTemplate
        {
            get { return base.Display; }
            set
            {
                base.Display = value;
                OnPropertyChanged("DisplayText");
            }
        }
        public string DisplayText
        {
            get { return TextTemplate.Shorten(60); }
        }

    }
    public class TextTemplate : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public TextTemplate(){ }
        private Guid _textTemplateId;
        public Guid TextTemplateId
        {
            get { return _textTemplateId; }
            set
            {
                _textTemplateId = value;
                OnPropertyChanged("TextTemplateId");
            }
        }
        private string _airportCode;

        public string AirportCode
        {
            get { return _airportCode; }
            set
            {
                _airportCode = value;
                OnPropertyChanged("AirportCode");
            }
        }

        private string _textName;

        public string TextName
        {
            get { return _textName; }
            set {
                _textName = value;
                OnPropertyChanged("TextName");
            }
        }

        private string _textDisplay;

        public string TextDisplay
        {
            get { return _textDisplay; }
            set
            {
                _textDisplay = value;
                OnPropertyChanged("TextDisplay");
            }
        }

        private int _rotateInSeconds;

        public int RotateInSeconds
        {
            get { return _rotateInSeconds; }
            set
            {
                _rotateInSeconds = value;
                OnPropertyChanged("RotateInSeconds");
            }
        }

        private string _status;

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
                    default:
                        _status = String.Empty;
                        break;
                }
                OnPropertyChanged("Status");
            }
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
