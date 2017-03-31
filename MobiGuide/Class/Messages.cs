using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MobiGuide.Class
{
    static class Messages
    {
        internal const string ERROR_ADD_AIRCRAFT_CONFIG = "Failed to Add Aircraft Configuration";
        internal const string ERROR_ADD_AIRCRAFT_TYPE = "Failed to create Aircraft Type ";
        internal const string ERROR_ADD_AIRPORT_REF = "Cannot Save Airport Reference";
        internal const string ERROR_ADD_AIRPORT_TRANSLATION = "Error Occurred While Add Airport Translation";
        internal const string ERROR_ADD_LANGUAGE_REF = "Cannot Save Language Reference";
        internal const string ERROR_ADD_TEXT_TEMPLATE = "Failed to create Text Template ";
        internal const string ERROR_ADD_USER = "Cannot add new user at this time ";
        internal const string ERROR_AIRCRAFT_TYPE_NOTFOUND = "No Aircraft Type Found Do you want to go to create it ";
        internal const string ERROR_AIRPORT_REF_NOT_FOUND = "Airport Reference Not Found";
        internal const string ERROR_EMPTY_UPASS = "Please fill both Username and Password";
        internal const string ERROR_ENTER_AIRLINE_NAME = "Please Enter Airline Name";
        internal const string ERROR_EXISTING_AIRLINE_CODE = "Airport Code is Existing";
        internal const string ERROR_GET_AIRCRAFT_CONFIG = "Cannot get Aircraft Configuration Data";
        internal const string ERROR_GET_AIRCRAFT_TYPE = "Failed to download Aircraft Type Data";
        internal const string ERROR_GET_AIRPORT_CODE_LIST = "Cannot Get Airport Code List";
        internal const string ERROR_GET_AIRPORT_REF = "Error Occurred While Fetching Airport Reference";
        internal const string ERROR_GET_AIRPORT_TRANSLATION = "Error occurred while fetching Airport Translation Info";
        internal const string ERROR_GET_BG_COLOR = "Can not Get Background Color";
        internal const string ERROR_GET_FONT_COLOR = "Can not Get Font Color";
        internal const string ERROR_GET_FONT_NAME = "Can not Get Font Name";
        internal const string ERROR_GET_FONT_SIZE = "Can not Get Font Size";
        internal const string ERROR_GET_LANGUAGE_CODE_LIST = "Cannot Get Language Code List";
        internal const string ERROR_GET_LANGUAGE_REF = "Error Occurred While Fetching Language Reference";
        internal const string ERROR_GET_TEXT_TEMPLATE = "Failed to download Text Template Data";
        internal const string ERROR_LANGUAGE_REF_NOT_FOUND = "Language Reference Not Found ";
        internal const string ERROR_LOGIN_FAILED = "Failed to login please contact administrator";
        internal const string ERROR_REMOVE_AIRLINE_LARGE_LOGO = "Failed to remove Airline Large Logo";
        internal const string ERROR_REMOVE_AIRLINE_SMALL_LOGO = "Failed to remove Airline Small Logo";
        internal const string ERROR_SHOW_AIRLINE_LOGO = "Can not display Airline Logo";
        internal const string ERROR_UNKNOWN = "Unexpected error occurred Please contact administrator ";
        internal const string ERROR_UPASS_NOMATCH = "Username or Password do not match";
        internal const string ERROR_UPDATE_AIRCRAFT_CONFIG = "Failed to Update Aircraft Configuration";
        internal const string ERROR_UPDATE_AIRCRAFT_TYPE = "Failed to update Aircraft Type ";
        internal const string ERROR_UPDATE_AIRLINE_LARGE_LOGO = "Failed to update Airline Large Logo";
        internal const string ERROR_UPDATE_AIRLINE_REF = "Failed to update Airline Reference";
        internal const string ERROR_UPDATE_AIRLINE_SMALL_LOGO = "Failed to update Airline Small Logo";
        internal const string ERROR_UPDATE_AIRPORT_TRANSLATION = "Error Occurred While Update Airport Translation";
        internal const string ERROR_UPDATE_TEXT_TEMPLATE = "Failed to update Text Template ";
        internal const string ERROR_UPDATE_UPROFILE = "Failed to update profile ";

        internal const string INFO_CONFIRM_LOGOUT = "Do you want to logout ";

        internal const string SUCCESS_ADD_AIRCRAFT_CONFIG = "Add Aircraft Configuration successfully";
        internal const string SUCCESS_ADD_AIRCRAFT_TYPE = "Create Aircraft Type Successfully ";
        internal const string SUCCESS_ADD_AIRPORT_REF = "Add Airport Reference Successfully";
        internal const string SUCCESS_ADD_AIRPORT_TRANSLATION = "Add Airport Translation Successfully";
        internal const string SUCCESS_ADD_LANGUAGE_REF = "Add Language Reference Successfully";
        internal const string SUCCESS_ADD_TEXT_TEMPLATE = "Create Text Template Successfully ";
        internal const string SUCCESS_UPDATE_AIRCRAFT_CONFIG = "Update Aircraft Configuration successfully";
        internal const string SUCCESS_UPDATE_AIRCRAFT_TYPE = "Update Aircraft Type Successfully ";
        internal const string SUCCESS_UPDATE_AIRLINE_REF = "Update Airline Reference Successfully";
        internal const string SUCCESS_UPDATE_AIRPORT_TRANSLATION = "Update Airport Translation Successfully";
        internal const string SUCCESS_UPDATE_TEXT_TEMPLATE = "Update Text Template Successfully ";
        internal const string SUCCESS_UPDATE_UPROFILE = "Update Profile Succussfully ";

        internal const string TITLE_EDIT_AIRCRAFT_TYPE = "Edit Aircraft Type";
        internal const string TITLE_EDIT_AIRPORT_REF = "Edit Airport Reference";
        internal const string TITLE_EDIT_TEXT_TEMPLATE = "Edit Text Template";
        internal const string TITLE_EDIT_TEXT_TRANSLATION = "Edit Text Translation";
        internal const string TITLE_NEW_AIRCRAFT_CONFIG = "New Aircraft Configuration";
        internal const string TITLE_NEW_AIRCRAFT_TYPE = "New Aircraft Type";
        internal const string TITLE_NEW_AIRPORT_REF = "New Airport Reference";
        internal const string TITLE_NEW_TEXT_TEMPLATE = "New Text Template";
        internal const string TITLE_NEW_TEXT_TRANSLATION = "New Text Translation";
        internal const string TITLE_EDIT_AIRCRAFT_CONFIG = "Edit Aircraft Configuration";

        internal const string WARNING_AIRPORT_REF_NOT_FOUND = "Airport Reference Not Found";
        internal const string WARNING_ENTER_AIRCRAFT_CONFIG_CODE = "Please enter \"Aircraft Configuration Code\"";
        internal const string WARNING_ENTER_AIRCRAFT_CONFIG_NAME = "Please enter \"Aircraft Configuration Name\"";
        internal const string WARNING_ENTER_AIRPORT_NAME = "Please Enter Airport Name";
        internal const string WARNING_ENTER_LANGUAGE_NAME = "Please Enter Language Name";
        internal const string WARNING_EXISTING_AIRPORT_CODE = "Airport Code is Existing";
        internal const string WARNING_EXISTING_LANGUAGE_CODE = "Language Code is Existing";
        internal const string WARNING_EXISTING_UNAME = "This Username already exist Please use another one ";
        internal const string WARNING_NO_MORE_LANGUAGE_REF_ADD = "No more Language Reference to add to this Airport";
        internal const string WARNING_NOT_FILLED_FIELDS = "Please fill every fields before save ";
        internal const string WARNING_NOT_FILLED_LANGUAGES = "Please fill all display name in different languages";
        internal const string WARNING_PASS_NOT_FILLED = "Please fill both Password and Confirm Password fields";
        internal const string WARNING_PASS_NOT_MATCH = "Please match your password";
        internal const string WARNING_WRONG_AIRLINE_CODE = "Airline Code Must Contains 3 Characters";
        internal const string WARNING_WRONG_AIRPORT_CODE = "Airport Code Must Contains 3 Characters";
        internal const string WARNING_WRONG_LANGUAGE_CODE = "Language Code Must Contains 2-3 Characters";

        internal static string SUCCESS_ADD_USER(string uname)
        {
            return String.Format("Add user [{0}] successfully", uname);
        }

        internal static string SUCCESS_UPDATE_AIRPORT_REF(string airportCode)
        {
            return String.Format("Update [{0}] Airport Reference Successfully", airportCode);
        }
        internal static string ERROR_UPDATE_AIRPORT_REF(string airportCode)
        {
            return string.Format("Failed to Update [{0}] Airport Reference", airportCode);
        }

        internal static string SUCCESS_UPDATE_LANGUAGE_REF(string langCode)
        {
            return string.Format("Update [{0}] Language Reference Successfully", langCode);
        }

        internal static string ERROR_UPDATE_LANGUAGE_REF(string langCode)
        {
            return string.Format("Failed to Update [{0}] Language Reference", langCode);
        }
    }
}
