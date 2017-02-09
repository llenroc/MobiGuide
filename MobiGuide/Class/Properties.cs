using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomExtensions;
using System.Reflection;
using System.Diagnostics;

namespace Properties
{
    //abstract properties
    public abstract class Reference : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private string _code;
        private string _name;
        private string _status;
        private string _statusCode;
        protected virtual string Code
        {
            get { return _code; }
            set
            {
                _code = value;
                OnPropertyChanged("Code");
            }
        }
        protected virtual string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        protected virtual string Status
        {
            get { return _status; }
        }
        protected virtual string StatusCode
        {
            get { return _statusCode; }
            set
            {
                _statusCode = value;
                switch (value)
                {
                    case "A":
                        _status = "Active";
                        break;
                    case "I":
                        _status = "Inactive";
                        break;
                }
                OnPropertyChanged("StatusCode");
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

    public class AirportReference : Reference
    {
        public AirportReference() : this(String.Empty, String.Empty, String.Empty)
        {
        }
        public AirportReference(string airportCode, string airportName, string statusCode)
        {
            base.Code = airportCode;
            base.Name = airportName;
            base.StatusCode = statusCode;
        }
        public string AirportCode
        {
            get { return base.Code; }
            set { base.Code = value; }
        }
        public string AirportName
        {
            get { return base.Name; }
            set { base.Name = value; }
        }
        public new string Status
        {
            get { return base.Status; }
        }

        public string SeletedLanguageCode { get; internal set; }

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
    public class AircraftType : Reference
    {
        public string AircraftTypeCode
        {
            get { return base.Code;}
            set
            {
                base.Code = value;
                OnPropertyChanged("AircraftTypeCode");
            }
        }
        public string AircraftTypeName
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                OnPropertyChanged("AircraftTypeName");
            }
        }
        public new string Status
        {
            get { return base.Status; }
        }
        public new string StatusCode
        {
            get { return base.StatusCode; }
            set
            {
                base.StatusCode = value;
                OnPropertyChanged("StatusCode");
            }
        }
    }
    public class AircraftConfiguration : Reference
    {
        private Guid _id;
        private string _airlineCode;
        private AircraftType _aircraftType;
        private double _aisleX;
        private bool _frontDoorBoardingFlag;
        private bool _rearDoorBoardingFlag;
        private double _frontDoorX;
        private double _frontDoorY;
        private double _frontDoorWidth;
        private double _rearDoorX;
        private double _rearDoorY;
        private double _rearDoorWidth;
        private string _seatMapImagePath;
        public Guid AircraftConfigurationId
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged("AircraftConfigurationId");
            }
        }
        public AircraftType AircraftType
        {
            get { return _aircraftType; }
            set
            {
                _aircraftType = value;
                OnPropertyChanged("AircraftType");
            }
        }
        public string AirlineCode
        {
            get { return _airlineCode; }
            set
            {
                _airlineCode = value;
            }
        }
        public string AircraftConfigurationCode
        {
            get { return base.Code; }
            set
            {
                base.Code = value;
                OnPropertyChanged("AircraftConfigurationCode");
            }
        }
        public string AircraftConfigurationName
        {
            get { return base.Name; }
            set
            {
                base.Name = value;
                OnPropertyChanged("AircraftConfigurationName");
            }
        }
        public double AisleX
        {
            get { return _aisleX; }
            set
            {
                _aisleX = value;
                OnPropertyChanged("AisleX");
            }
        }
        public bool FrontDoorBoardingFlag
        {
            get { return _frontDoorBoardingFlag; }
            set
            {
                _frontDoorBoardingFlag = value;
                OnPropertyChanged("FrontDoorBoardingFlag");
            }
        }
        public bool RearDoorBoardingFlag
        {
            get { return _rearDoorBoardingFlag; }
            set
            {
                _rearDoorBoardingFlag = value;
                OnPropertyChanged("RearDoorBoardingFlag");
            }
        }
        public double FrontDoorX
        {
            get { return _frontDoorX; }
            set
            {
                _frontDoorX = value;
                OnPropertyChanged("FrontDoorX");
            }
        }
        public double FrontDoorY
        {
            get { return _frontDoorY; }
            set
            {
                _frontDoorY = value;
                OnPropertyChanged("FrontDoorY");
            }
        }
        public double FrontDoorWidth
        {
            get { return _frontDoorWidth; }
            set
            {
                _frontDoorWidth = value;
                OnPropertyChanged("FrontDoorWidth");
            }
        }
        public double RearDoorX
        {
            get { return _rearDoorX; }
            set
            {
                _rearDoorX = value;
                OnPropertyChanged("RearDoorX");
            }
        }
        public double RearDoorY
        {
            get { return _rearDoorY; }
            set
            {
                _rearDoorY = value;
                OnPropertyChanged("RearDoorY");
            }
        }
        public double RearDoorWidth
        {
            get { return _rearDoorWidth; }
            set
            {
                _rearDoorWidth = value;
                OnPropertyChanged("RearDoorWidth");
            }
        }
        public string SeatMapImagePath
        {
            get { return _seatMapImagePath; }
            set
            {
                _seatMapImagePath = value;
                OnPropertyChanged("SeatMapImagePath");
            }
        }

        public new string Status
        {
            get { return base.Status; }
        }
        public new string StatusCode
        {
            get { return base.StatusCode; }
            set
            {
                base.StatusCode = value;
                OnPropertyChanged("Status");
                OnPropertyChanged("StatusCode");
            }
        }
    }
    public enum STATUS
    {
        NEW,
        EDIT
    }
    public enum DISPLAY_TYPE
    {
        LOGO,
        SEATMAPS,
        TEXT
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
    public abstract class Shape
    {
        private double _x;
        private double _y;
        private double _width;
        private double _height;

        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
            }
        }

        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
            }
        }

        public double Width
        {
            get { return _width; }
            set
            {
                _width = value;
            }
        }

        public double Height
        {
            get { return _height; }
            set
            {
                _height = value;
            }
        }
    }

    public class Seat : Shape
    {
        public int Row { get; set; }
        public string Column { get; set; }
    }
    public class AisleY : Shape
    {
        private int _afterRow;
        public int AfterRow
        {
            get { return _afterRow; }
            set
            {
                _afterRow = value;
            }
        }
    }
    public class ShowText : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Guid _textTemplateId;
        private string _flightNo;
        private string _origin;
        private string _dest;
        private string _originCode;
        private string _destCode;
        private DateTime _depDate;
        private TimeSpan _depTime;
        private string _textTemplate;
        private int _rotateSeconds;
        
        public Guid TextTemplateId
        {
            get { return _textTemplateId; }
            set
            {
                _textTemplateId = value;
                OnPropertyChanged("TextTemplateId");
            }
        }
        public string FlightNo
        {
            get { return _flightNo; }
            set
            {
                _flightNo = value;
                OnPropertyChanged("FlightNo");
            }
        }
        public string OriginName
        {
            get { return _origin; }
            set
            {
                _origin = value;
                OnPropertyChanged("OriginName");
            }
        }
        public string DestinationName
        {
            get { return _dest; }
            set
            {
                _dest = value;
                OnPropertyChanged("DestinationName");
            }
        }
        public string OriginCode
        {
            get { return _originCode; }
            set
            {
                _originCode = value;
                OnPropertyChanged("OriginCode");
            }
        }
        public string DestinationCode
        {
            get { return _destCode; }
            set
            {
                _destCode = value;
                OnPropertyChanged("DestinationCode");
            }
        }
        public DateTime DepartureDate
        {
            get { return _depDate.Date; }
            set
            {
                _depDate = value;
                OnPropertyChanged("DepartureDate");
            }
        }
        public TimeSpan DepartureTime
        {
            get { return _depTime; }
            set
            {
                _depTime = value;
                OnPropertyChanged("DepartureTime");
            }
        }
        public string TextTemplate
        {
            get { return _textTemplate; }
            set
            {
                _textTemplate = value;
                OnPropertyChanged("TextTemplate");
            }
        }
        public int RotateInSeconds
        {
            get { return _rotateSeconds; }
            set
            {
                _rotateSeconds = value;
                OnPropertyChanged("RotateInSeconds");
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
}
