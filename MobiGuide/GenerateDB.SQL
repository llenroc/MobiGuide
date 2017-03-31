EXEC
	sp_addtype	AMOUNT,
				"decimal(13,2)",
				"NULL"
GO


EXEC 
	sp_addtype	BIGNUMBER,
				"bigint",
				"NULL"
GO


EXEC
	sp_addtype	CODE,
				"varchar(20)",
				"NULL"
GO


EXEC
	sp_addtype	FLAG,
				"bit",
				"NULL"
GO


EXEC
	sp_addtype	GUID,
				"uniqueidentifier",
				"NULL"
GO

EXEC
	sp_addtype	LONG_NAME,
				"nvarchar(450)",
				"NULL"
GO


EXEC
	sp_addtype	STATUS_CODE,
				"varchar(1)",
				"NULL"
GO


EXEC
	sp_addtype	USR_NAME,
				"nvarchar(60)",
				"NULL"
GO


CREATE TABLE AircraftConfiguration
	(
	AircraftConfigurationId		GUID	NOT NULL ,
	AirlineCode					CODE  ,
	AircraftTypeCode			CODE  ,
	AircraftConfigurationCode	CODE  ,
	AircraftConfigurationName	USR_NAME  ,
	SeatMapImage				varbinary(MAX)  NULL ,
	AisleX						float  NULL ,
	FrontDoorBoardingFlag		FLAG  ,
	FrontDoorX					float  NULL ,
	FrontDoorY					float  NULL ,
	FrontDoorWidth				float  NULL ,
	RearDoorX					float  NULL ,
	RearDoorY					float  NULL ,
	RearDoorWidth				float NULL ,
	RearDoorBoardingFlag		FLAG  ,
	MiddleRow					integer	NULL,
	StatusCode					STATUS_CODE  ,
	CommitBy					GUID  ,
	CommitDateTime				datetime  NULL 
	)
GO


ALTER TABLE AircraftConfiguration
	ADD CONSTRAINT  XPKAircraftConfiguration 
	PRIMARY KEY   
	NONCLUSTERED	(
					AircraftConfigurationId  ASC
					)
GO


CREATE INDEX XIF1AircraftConfiguration 
ON AircraftConfiguration
(
	AirlineCode  ASC
)
GO


CREATE INDEX XIF2AircraftConfiguration 
ON AircraftConfiguration
(
	AircraftTypeCode  ASC
)
GO


CREATE TABLE AircraftTypeReference
(
	AircraftTypeCode	CODE  NOT NULL ,
	AircraftTypeName	USR_NAME  ,
	StatusCode			STATUS_CODE  ,
	CommitBy			GUID  ,
	CommitDateTime		datetime  NULL 
)
GO


ALTER TABLE AircraftTypeReference
	ADD CONSTRAINT  XPKAircraftTypeReference 
	PRIMARY KEY   
	NONCLUSTERED	(
					AircraftTypeCode  ASC
					)
GO


CREATE TABLE AirlineReference
(
	AirlineCode				CODE  NOT NULL ,
	AirlineName				USR_NAME  ,
	AirlineLogoSmall		varbinary(MAX)  NULL ,
	AirlineLogoLarge		varbinary(MAX)  NULL ,
	FontName				USR_NAME  ,
	FontSize				integer  NULL ,
	FontColor				integer  NULL ,
	BackGroundColor			integer  NULL ,
	BackGroundColor2		integer  NULL ,
	LineColor				integer  NULL ,
	SeatColor				integer  NULL ,
	ShowGuidanceInSeconds	integer  NULL ,
	StatusCode				STATUS_CODE  ,
	CommitBy				GUID  ,
	CommitDateTime			datetime  NULL 
)
GO


ALTER TABLE AirlineReference
	ADD CONSTRAINT  XPKAirlineReference 
	PRIMARY KEY   
	NONCLUSTERED (AirlineCode  ASC)
GO


CREATE TABLE AirportReference
(
	AirportCode			CODE  NOT NULL ,
	AirportName			USR_NAME  ,
	StatusCode			STATUS_CODE  ,
	CommitBy			GUID  ,
	CommitDateTime		datetime  NULL 
)
GO


ALTER TABLE AirportReference
	ADD CONSTRAINT  XPKAirportReference 
	PRIMARY KEY   
	NONCLUSTERED	(
					AirportCode  ASC
					)
GO


CREATE TABLE AirportTranslation
(
	AirportTranslationId	GUID  NOT NULL ,
	AirportCode				CODE  ,
	LanguageCode			CODE  ,
	AirportName				nvarchar(MAX)  ,
	CommitBy				GUID  ,
	CommitDateTime			datetime  NULL 
)
GO


ALTER TABLE AirportTranslation
	ADD CONSTRAINT  XPKAirportTranslation 
	PRIMARY KEY   
	NONCLUSTERED	(
					AirportTranslationId  ASC
					)
GO


CREATE INDEX XIF1AirportTranslation 
ON AirportTranslation
(
	AirportCode  ASC
)
GO


CREATE INDEX XIF2AirportTranslation 
ON AirportTranslation
(
	LanguageCode  ASC
)
GO


CREATE TABLE LanguageReference
(
	LanguageCode	CODE  NOT NULL ,
	LanguageName	USR_NAME  ,
	StatusCode		STATUS_CODE  ,
	CommitBy		GUID  ,
	CommitDateTime  datetime  NULL 
)
GO


ALTER TABLE LanguageReference
	ADD CONSTRAINT  XPKLanguageReference 
	PRIMARY KEY   
	NONCLUSTERED	(
					LanguageCode  ASC
					)
GO


CREATE TABLE SeatMap
(
	SeatMapId					GUID  NOT NULL ,
	AircraftConfigurationId		GUID  ,
	SeatRow						integer  NULL ,
	SeatColumn					CODE  ,
	SeatWidth					float  NULL ,
	SeatHeight					float  NULL ,
	PositionX					float  NULL ,
	PositionY					float  NULL ,
	CommitBy					GUID  ,
	CommitDateTime				datetime  NULL 
)
GO


ALTER TABLE SeatMap
	ADD CONSTRAINT  XPKSeatMap 
	PRIMARY KEY   
	NONCLUSTERED	(
					SeatMapId  ASC
					)
GO


CREATE INDEX XIF1SeatMap 
ON SeatMap
(
	AircraftConfigurationId  ASC
)
GO


CREATE TABLE TextTemplate
(
	TextTemplateId			GUID  NOT NULL ,
	AirlineCode				CODE  ,
	TextName				USR_NAME  ,
	TextTemplate			NVARCHAR(MAX)  ,
	RotateInSeconds			integer  NULL ,
	StatusCode				STATUS_CODE  ,
	CommitBy				GUID  ,
	CommitDateTime			datetime  NULL 
)
GO


ALTER TABLE TextTemplate
	ADD CONSTRAINT  XPKTextTemplate 
	PRIMARY KEY   
	NONCLUSTERED	(
					TextTemplateId  ASC
					)
GO


CREATE INDEX XIF1TextTemplate 
ON TextTemplate
(
	AirlineCode  ASC
)
GO


CREATE TABLE TextTranslation
(
	TextTranslationId		GUID  NOT NULL ,
	TextTemplateId			GUID  ,
	LanguageCode			CODE  ,
	TextTemplate			NVARCHAR(MAX)  ,
	CommitBy				GUID  ,
	CommitDateTime			datetime  NULL 
)
GO


ALTER TABLE TextTranslation
	ADD CONSTRAINT  XPKTextTranslation 
	PRIMARY KEY   
	NONCLUSTERED	(
					TextTranslationId  ASC
					)
GO


CREATE INDEX XIF1TextTranslation 
ON TextTranslation
(
	TextTemplateId  ASC
)
GO


CREATE INDEX XIF2TextTranslation 
ON TextTranslation
(
	LanguageCode  ASC
)
GO


CREATE TABLE UserAccount
(
	UserAccountId		GUID  NOT NULL ,
	AirlineCode			CODE  ,
	UserLogon			USR_NAME  ,
	UserPassword		USR_NAME  ,
	LastName			USR_NAME  ,
	FirstName			USR_NAME  ,
	StatusCode			STATUS_CODE  ,
	CommitBy			GUID  ,
	CommitDateTime		datetime  NULL 
)
GO


ALTER TABLE UserAccount
	ADD CONSTRAINT  XPKUserAccount 
	PRIMARY KEY   
	NONCLUSTERED	(
					UserAccountId  ASC
					)
GO


CREATE INDEX XIF1UserAccount 
ON UserAccount
(
	AirlineCode  ASC
)
GO

------------------------------------------
-- Insert Example Airline
------------------------------------------

INSERT INTO 
	AirlineReference 
	(
	AirlineCode,
	AirlineName,
	FontName,
	FontSize,
	FontColor,
	BackGroundColor,
	LineColor,
	SeatColor,
	ShowGuidanceInSeconds,
	StatusCode,
	CommitBy,
	CommitDateTime
	) 
VALUES
	(
	'PG',
	'Bangkok Airways',
	'SeGOe WP',
	65,
	16777215,
	10407935,
	3978097,
	16711680,
	5,
	'A',
	NULL,
	GETDATE()
	)
GO

------------------------------------------
-- Insert Default Airline Logos
------------------------------------------

UPDATE AirlineReference
SET AirlineLogoLarge = 
    (
	SELECT  BulkColumn 
	FROM OPENROWSET
		(
		BULK  'C:\Users\thawa\OneDrive\Documents\Visual Studio 2015\Projects\MobiGuide\MobiGuide\SampleData\PGBigLogo.png',
		SINGLE_BLOB
		) 
		AS x
	)
WHERE AirlineCode = 'PG'
GO

UPDATE AirlineReference
SET AirlineLogoSmall = 
    (
	SELECT  BulkColumn 
	FROM OPENROWSET
		(
		BULK  'C:\Users\thawa\OneDrive\Documents\Visual Studio 2015\Projects\MobiGuide\MobiGuide\SampleData\PGSmallLogo.png',
		SINGLE_BLOB
		) 
		AS x
	)
WHERE AirlineCode = 'PG'
GO

-------------------------------------------
-- Insert 'System' User
-------------------------------------------

DECLARE @sysId			uniqueidentifier  
SET		@sysId			= NEWID() 

INSERT INTO 
	UserAccount 
	(
	UserAccountId,
	AirlineCode,
	UserLogon,
	UserPassword,
	LastName,
	FirstName,
	StatusCode,
	CommitBy,
	CommitDateTime
	)
VALUES
	(
	@sysId,
	NULL,
	'system',
	'test',
	NULL,
	'SYSTEM',
	NULL,
	NULL,
	GETDATE()
	)

-------------------------------------------
-- Insert Example Language
-------------------------------------------

INSERT INTO 
	LanguageReference 
VALUES 
	(
	'THA',
	'Thai',
	'A' ,
	@sysId,
	GETDATE()
	)
GO

-------------------------------------------
-- Insert Example User
-------------------------------------------

DECLARE @adminId			uniqueidentifier  
SET		@adminId			= NEWID()

INSERT INTO 
	UserAccount 
	(
	UserAccountId,
	AirlineCode,
	UserLogon,
	UserPassword,
	LastName,
	FirstName,
	StatusCode,
	CommitBy,
	CommitDateTime
	)
VALUES
	(
	@adminId,
	'PG',
	'dome',
	'test',
	'Klinkliang',
	'Thawatchai',
	NULL,
	NULL,
	GETDATE()
	)
GO