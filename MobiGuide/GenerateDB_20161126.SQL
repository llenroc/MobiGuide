exec sp_addtype AMOUNT, "decimal(13,2)", "NULL"
go


exec sp_addtype BIGNUMBER, "bigint", "NULL"
go


exec sp_addtype CODE, "varchar(20)", "NULL"
go


exec sp_addtype FLAG, "bit", "NULL"
go


exec sp_addtype GUID, "uniqueidentifier", "NULL"
go


exec sp_addtype LARGE_TEXT, "nvarchar(MAX)", "NULL"
go


exec sp_addtype LONG_NAME, "nvarchar(450)", "NULL"
go


exec sp_addtype STATUS_CODE, "varchar(1)", "NULL"
go


exec sp_addtype USR_NAME, "nvarchar(60)", "NULL"
go


CREATE TABLE AircraftConfiguration
(
	AircraftConfigurationId  GUID  NOT NULL ,
	AirlineCode  CODE  ,
	AircraftTypeCode  CODE  ,
	AircraftConfigurationCode  CODE  ,
	AircraftConfigurationName  USR_NAME  ,
	SeatMapImage  varbinary(MAX)  NULL ,
	AisleX  integer  NULL ,
	FrontDoorBoardingFlag  FLAG  ,
	FrontDoorX  integer  NULL ,
	FrontDoorY  integer  NULL ,
	RearDoorX  integer  NULL ,
	RearDoorY  integer  NULL ,
	RearDoorBoardingFlag  FLAG  ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE AircraftConfiguration
	ADD CONSTRAINT  XPKAircraftConfiguration PRIMARY KEY   NONCLUSTERED (AircraftConfigurationId  ASC)
go


CREATE INDEX XIF1AircraftConfiguration ON AircraftConfiguration
(
	AirlineCode  ASC
)
go


CREATE INDEX XIF2AircraftConfiguration ON AircraftConfiguration
(
	AircraftTypeCode  ASC
)
go


CREATE TABLE AircraftTypeReference
(
	AircraftTypeCode  CODE  NOT NULL ,
	AircraftTypeName  USR_NAME  ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE AircraftTypeReference
	ADD CONSTRAINT  XPKAircraftTypeReference PRIMARY KEY   NONCLUSTERED (AircraftTypeCode  ASC)
go


CREATE TABLE AirlineReference
(
	AirlineCode  CODE  NOT NULL ,
	AirlineName  USR_NAME  ,
	AirlineLogoSmall  varbinary(MAX)  NULL ,
	AirlineLogoLarge  varbinary(MAX)  NULL ,
	FontName  USR_NAME  ,
	FontSize  integer  NULL ,
	FontColor  integer  NULL ,
	BackGroundColor  integer  NULL ,
	LineColor  integer  NULL ,
	SeatColor  integer  NULL ,
	ShowGuidanceInSeconds  integer  NULL ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE AirlineReference
	ADD CONSTRAINT  XPKAirlineReference PRIMARY KEY   NONCLUSTERED (AirlineCode  ASC)
go


CREATE TABLE AirportReference
(
	AirportCode  CODE  NOT NULL ,
	AirportName  USR_NAME  ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE AirportReference
	ADD CONSTRAINT  XPKAirportReference PRIMARY KEY   NONCLUSTERED (AirportCode  ASC)
go


CREATE TABLE AirportTranslation
(
	AirportTranslationId  GUID  NOT NULL ,
	AirportCode  CODE  ,
	LanguageCode  CODE  ,
	AirportName  USR_NAME  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE AirportTranslation
	ADD CONSTRAINT  XPKAirportTranslation PRIMARY KEY   NONCLUSTERED (AirportTranslationId  ASC)
go


CREATE INDEX XIF1AirportTranslation ON AirportTranslation
(
	AirportCode  ASC
)
go


CREATE INDEX XIF2AirportTranslation ON AirportTranslation
(
	LanguageCode  ASC
)
go


CREATE TABLE LanguageReference
(
	LanguageCode  CODE  NOT NULL ,
	LanguageName  USR_NAME  ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE LanguageReference
	ADD CONSTRAINT  XPKLanguageReference PRIMARY KEY   NONCLUSTERED (LanguageCode  ASC)
go


CREATE TABLE SeatMap
(
	SeatMapId  GUID  NOT NULL ,
	AircraftConfigurationId  GUID  ,
	SeatRow  integer  NULL ,
	SeatColumn  CODE  ,
	PositionX  integer  NULL ,
	PositionY  integer  NULL ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE SeatMap
	ADD CONSTRAINT  XPKSeatMap PRIMARY KEY   NONCLUSTERED (SeatMapId  ASC)
go


CREATE INDEX XIF1SeatMap ON SeatMap
(
	AircraftConfigurationId  ASC
)
go


CREATE TABLE TextTemplate
(
	TextTemplateId  GUID  NOT NULL ,
	AirlineCode  CODE  ,
	TextName  USR_NAME  ,
	TextTemplate  LARGE_TEXT  ,
	RotateInSeconds  integer  NULL ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE TextTemplate
	ADD CONSTRAINT  XPKTextTemplate PRIMARY KEY   NONCLUSTERED (TextTemplateId  ASC)
go


CREATE INDEX XIF1TextTemplate ON TextTemplate
(
	AirlineCode  ASC
)
go


CREATE TABLE TextTranslation
(
	TextTranslationId  GUID  NOT NULL ,
	TextTemplateId  GUID  ,
	LanguageCode  CODE  ,
	TextTemplate  LARGE_TEXT  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE TextTranslation
	ADD CONSTRAINT  XPKTextTranslation PRIMARY KEY   NONCLUSTERED (TextTranslationId  ASC)
go


CREATE INDEX XIF1TextTranslation ON TextTranslation
(
	TextTemplateId  ASC
)
go


CREATE INDEX XIF2TextTranslation ON TextTranslation
(
	LanguageCode  ASC
)
go


CREATE TABLE UserAccount
(
	UserAccountId  GUID  NOT NULL ,
	AirlineCode  CODE  ,
	UserLogon  USR_NAME  ,
	UserPassword  USR_NAME  ,
	LastName  USR_NAME  ,
	FirstName  USR_NAME  ,
	StatusCode  STATUS_CODE  ,
	CommitBy  GUID  ,
	CommitDateTime  datetime  NULL 
)
go


ALTER TABLE UserAccount
	ADD CONSTRAINT  XPKUserAccount PRIMARY KEY   NONCLUSTERED (UserAccountId  ASC)
go


CREATE INDEX XIF1UserAccount ON UserAccount
(
	AirlineCode  ASC
)
go

INSERT INTO AirlineReference (
	AirlineCode,
	AirlineName,
	FontName,
	FontColor,
	BackGroundColor,
	LineColor,
	SeatColor,
	ShowGuidanceInSeconds,
	StatusCode,
	CommitBy,
	CommitDateTime
) VALUES
(
	'PG',
	'Bangkok Airways',
	'Tahoma',
	21156,
	21156,
	21156,
	21156,
	5,
	NULL,
	NULL,
	GETDATE()
);
go

UPDATE AirlineReference
SET AirlineLogoLarge = 
    (SELECT  BulkColumn FROM OPENROWSET(BULK  'C:\Users\thawa\OneDrive\Documents\Visual Studio 2015\Projects\MobiGuide\MobiGuide\SampleData\PGBigLogo.png', SINGLE_BLOB) AS x)
WHERE AirlineCode = 'PG'
go

UPDATE AirlineReference
SET AirlineLogoSmall = 
    (SELECT  BulkColumn FROM OPENROWSET(BULK  'C:\Users\thawa\OneDrive\Documents\Visual Studio 2015\Projects\MobiGuide\MobiGuide\SampleData\PGSmallLogo.png', SINGLE_BLOB) AS x)
WHERE AirlineCode = 'PG'
go

DECLARE @sysId uniqueidentifier  
SET @sysId = NEWID() 
INSERT INTO UserAccount 
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
);
go

DECLARE @sysId uniqueidentifier  
SET @sysId = NEWID() 
INSERT INTO UserAccount 
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
	'PG',
	'dome',
	'test',
	'Klinkliang',
	'Thawatchai',
	NULL,
	NULL,
	GETDATE()
);
go