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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
    public abstract class LanguageTranslation : INotifyPropertyChanged
    {
        private string _display;
        private bool _isEnabled;
        private string _languageCode;
        private List<string> _languageList;
        private string _languageName;
        private Guid _translationId;

        public LanguageTranslation()
        {
            _translationId = Guid.Empty;
            _languageName = string.Empty;
            _display = string.Empty;
            _languageList = new List<string>();
            _isEnabled = true;
        }

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public class AirportReference : Reference
    {
        public AirportReference() : this(string.Empty, string.Empty, string.Empty)
        {
        }

        public AirportReference(string airportCode, string airportName, string statusCode)
        {
            Code = airportCode;
            Name = airportName;
            StatusCode = statusCode;
        }

        public string AirportCode
        {
            get { return Code; }
            set { Code = value; }
        }

        public string AirportName
        {
            get { return Name; }
            set { Name = value; }
        }

        public new string Status
        {
            get { return base.Status; }
        }

        public string SeletedLanguageCode { get; internal set; }
    }
    public class AirportTranslation : LanguageTranslation
    {
        public Guid AirportTranslationId
        {
            get { return TranslationId; }
            set
            {
                TranslationId = value;
            }
        }

        public string AirportName
        {
            get { return Display; }
            set
            {
                Display = value;
            }
        }
    }
    public class TextTranslation : LanguageTranslation
    {
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

        public Guid TextTranslationId
        {
            get { return TranslationId; }
            set
            {
                TranslationId = value;
            }
        }

        public string TextTemplate
        {
            get { return Display; }
            set
            {
                Display = value;
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
        private string _airportCode;

        private int _rotateInSeconds;

        private string _status;

        private string _textDisplay;

        private string _textName;
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

        public string AirportCode
        {
            get { return _airportCode; }
            set
            {
                _airportCode = value;
                OnPropertyChanged("AirportCode");
            }
        }

        public string TextName
        {
            get { return _textName; }
            set {
                _textName = value;
                OnPropertyChanged("TextName");
            }
        }

        public string TextDisplay
        {
            get { return _textDisplay; }
            set
            {
                _textDisplay = value;
                OnPropertyChanged("TextDisplay");
            }
        }

        public int RotateInSeconds
        {
            get { return _rotateInSeconds; }
            set
            {
                _rotateInSeconds = value;
                OnPropertyChanged("RotateInSeconds");
            }
        }

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
                        _status = string.Empty;
                        break;
                }
                OnPropertyChanged("Status");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
    public class AircraftType : Reference
    {
        public string AircraftTypeCode
        {
            get { return Code;}
            set
            {
                Code = value;
                OnPropertyChanged("AircraftTypeCode");
            }
        }

        public string AircraftTypeName
        {
            get { return Name; }
            set
            {
                Name = value;
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
        private AircraftType _aircraftType;
        private double _aisleX;
        private bool _frontDoorBoardingFlag;
        private double _frontDoorWidth;
        private double _frontDoorX;
        private double _frontDoorY;
        private Guid _id;
        private int _middleRow;
        private bool _rearDoorBoardingFlag;
        private double _rearDoorWidth;
        private double _rearDoorX;
        private double _rearDoorY;
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

        public string AirlineCode { get; set; }

        public string AircraftConfigurationCode
        {
            get { return Code; }
            set
            {
                Code = value;
                OnPropertyChanged("AircraftConfigurationCode");
            }
        }

        public string AircraftConfigurationName
        {
            get { return Name; }
            set
            {
                Name = value;
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

        public int MiddleRow
        {
            get
            {
                return _middleRow;
            }
            set
            {
                _middleRow = value;
                OnPropertyChanged("MiddleRow");
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
        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }
    }

    public class Seat : Shape
    {
        public int Row { get; set; }
        public string Column { get; set; }
    }
    public class AisleY : Shape
    {
        public int AfterRow { get; set; }
    }
    public class ShowText : INotifyPropertyChanged
    {
        private DateTime _depDate;
        private TimeSpan _depTime;
        private string _dest;
        private string _destCode;
        private string _flightNo;
        private string _origin;
        private string _originCode;
        private int _rotateSeconds;
        private string _textTemplate;
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}
