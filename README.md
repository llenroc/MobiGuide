MOBIGUIDE SCOPE

1. User interface showing the seatmap configuration of an aircraft with highlight for the passenger where the seat is and how to walk to the seat. The passenger enters the seat or scans the boarding pass. The system will then display the location of the seat and the way to walk.
2. Show boarding information to passengers in different languages (e.g. time to boarding, boarding instructions, flight delays)
3. System User Interface to create and save seat map configuration for multiple aircrafts
4. System User Interface to maintain boarding information text templates
5. Boarding Gate user interface to display seatmap and to select boarding information text to display

Entities:

AIRCRAFT CONFIGURATION

1. Configuration Code (PK)
2. Seat Map Image
3. Vertical Position of Aisle
4. X/Y Coordinates for Front Door
5. X/Y Coordinates for Back Door

SEATMAP

1. SeatId (FK)
2. Aircraft Configuration Code (FK)
3. X/Y Coordinates for Seat Location
4. Seat Row (Numeric)
5. Seat Column (A to Z)

TEXT TEMPLATE

1. TextTemplateId (PK)
2. Template Name
3. Rotate Language in nn Seconds (numeric)

TEMPLATE LANGUAGE

1. LanguageTemplateId (PK)
2. Text TemplateId (FK)
3. Language Code (FK) (TH, JP, DE, EM, FR, IT, CN, PH, ID, IN, ES, KR, etc.)
4. Text (Unicode)

SETTINGS

1. Airline Code [FK]
2. Airline Name
3. Airline Logo
4. Colors (Style Sheet) (Background)
5. Font Name
6. Font Size
7. Font Color
8. Guidance Line and Seat Highlight Color
9. Number of Seconds to Show Guidance 
10. (Maybe) Screen Definition Settings (zoom factor for X/Y coordinates)

AIRPORT

1. Airport Code
2. Airport Name

AIRPORT LANGUAGE

1. LanguageAirportId (PK)
2. Airport Code (FK)
3. Language Code (FK) (TH, JP, DE, EM, FR, IT, CN, PH, ID, IN, ES, KR, etc.)
4. Airport Name (Unicode)