
CREATE TABLE HUB_COVERAGE
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	COVERAGE_HK          BINARY(16) NOT NULL,
	COVERAGE_CODE        VARCHAR(20) NULL,
	LIMIT_CODE           VARCHAR(20) NULL,
	DEDUCTIBLE_CODE      VARCHAR(20) NULL,
	STATE_CODE           VARCHAR(20) NULL
);

ALTER TABLE HUB_COVERAGE
ADD PRIMARY KEY (COVERAGE_HK);

CREATE TABLE HUB_POLICY
(
	POLICY_HK            BINARY() NOT NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_NUMBER        VARCHAR(50) NULL
);

ALTER TABLE HUB_POLICY
ADD PRIMARY KEY (POLICY_HK);

CREATE TABLE HUB_POLICY_TRANSACTION
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_TRANSACTION_HK BINARY(16) NOT NULL,
	POLICY_TRANSACTION_NK VARCHAR(16777216) NULL
);

ALTER TABLE HUB_POLICY_TRANSACTION
ADD PRIMARY KEY (POLICY_TRANSACTION_HK);

CREATE TABLE HUB_RISK
(
	RISK_HK              BINARY(16) NOT NULL,
	RISK_NK              VARCHAR(100) NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL
);

ALTER TABLE HUB_RISK
ADD PRIMARY KEY (RISK_HK);

CREATE TABLE HUB_VEHICLE
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	VEHICLE_HK           BINARY(16) NOT NULL,
	VEHICLE_NK           VARCHAR(16777216) NULL
);

ALTER TABLE HUB_VEHICLE
ADD PRIMARY KEY (VEHICLE_HK);

CREATE TABLE LNK_POLICY_HAS_POLICY_TRANSACTION
(
	POLICY_HK            BINARY() NOT NULL,
	POLICY_TRANSACTION_HK BINARY(16) NOT NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE LNK_POLICY_HAS_POLICY_TRANSACTION
ADD PRIMARY KEY (POLICY_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE LNK_POLICY_TRANSACTION_HAS_POLICY_TRANSACTION
(
	POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL,
	TRANSACTION_HK       BINARY(16) NOT NULL,
	RELATED_TRANSACTION_HK BINARY(16) NOT NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL
);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_POLICY_TRANSACTION
ADD PRIMARY KEY (POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE
(
	VEHICLE_HK           BINARY(16) NOT NULL,
	COVERAGE_HK          BINARY(16) NOT NULL,
	POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK BINARY(16) NOT NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	RISK_HK              BINARY(16) NOT NULL,
	POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE
ADD PRIMARY KEY (POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK);

CREATE TABLE LNK_POLICY_TRANSACTION_INSURES_VEHICLE
(
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	VEHICLE_HK           BINARY(16) NOT NULL,
	POLICY_TRANSACTION_HK BINARY(16) NOT NULL,
	RISK_HK              BINARY(16) NOT NULL
);

ALTER TABLE LNK_POLICY_TRANSACTION_INSURES_VEHICLE
ADD PRIMARY KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK);

CREATE TABLE SAT_BR_COVERAGE_REF_COV_DED_LIT
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	DEDUCTIBLE_DESCRIPTION VARCHAR(200) NOT NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	COVERAGE_HK          BINARY(16) NOT NULL
);

ALTER TABLE SAT_BR_COVERAGE_REF_COV_DED_LIT
ADD PRIMARY KEY (COVERAGE_HK,LOAD_TIMESTAMP);

CREATE TABLE SAT_BR_COVERAGE_REF_COV_LIMIT_LIT
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	LIMIT_DESCRIPTION    VARCHAR(200) NOT NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	COVERAGE_HK          BINARY(16) NOT NULL
);

ALTER TABLE SAT_BR_COVERAGE_REF_COV_LIMIT_LIT
ADD PRIMARY KEY (COVERAGE_HK,LOAD_TIMESTAMP);

CREATE TABLE SAT_BR_COVERAGE_REF_COV_MNEMONICS
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	COVERAGE_DESCRIPTION VARCHAR(200) NOT NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	COVERAGE_HK          BINARY(16) NOT NULL
);

ALTER TABLE SAT_BR_COVERAGE_REF_COV_MNEMONICS
ADD PRIMARY KEY (COVERAGE_HK,LOAD_TIMESTAMP);

CREATE TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_RATING_STRUCTURE
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL,
	RATING_STRUCTURE     VARCHAR(32) NOT NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	HASHDIFF             BINARY(16) NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_RATING_STRUCTURE
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_REF_PRODCT_RSK_TYP_TBL
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL,
	RISK_TYPE            VARCHAR(200) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	HASHDIFF             BINARY(16) NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_REF_PRODCT_RSK_TYP_TBL
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_RISK_SEGMENT
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL,
	RISK_SEGMENT         VARCHAR(32) NOT NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	HASHDIFF             BINARY(16) NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_RISK_SEGMENT
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE SAT_PEAK_POLICY
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	POLICY_NUMBER        VARCHAR(16777216) NOT NULL,
	POLICYREADONLYHISTORYID VARCHAR(16777216) NOT NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICY_ID            VARCHAR(16777216) NULL,
	ACCOUNTID            VARCHAR(16777216) NULL,
	AGENTID              VARCHAR(16777216) NULL,
	AIPVOLUNTARYRATEINDICATOR VARCHAR(16777216) NULL,
	APPLICATIONORIGINALWRITTENDATE DATE NULL,
	AVAILABLEVEHICLENUMBERID VARCHAR(16777216) NULL,
	CANCELLATIONDATE     DATE NULL,
	CAPPINGPREVIOUSLOBCODE VARCHAR(16777216) NULL,
	COMPANYINCEPTIONDATE DATE NULL,
	CONTRACTOVERRIDE     VARCHAR(16777216) NULL,
	CONTRACTREVIEWDATE   DATE NULL,
	CONTRACTTYPE         VARCHAR(16777216) NULL,
	CONTRACTTYPEASSIGNDATE DATE NULL,
	CORPORATEINCEPTIONDATE DATE NULL,
	CTRAUTHORIZATIONSTATUS VARCHAR(16777216) NULL,
	CTRRECEIVEDDATE      DATE NULL,
	CTRSENTDATE          DATE NULL,
	DECLINEOVERRIDE      BOOLEAN NULL,
	DECLINEOVERRIDEUSER  VARCHAR(16777216) NULL,
	DECPACKAGETYPECODE   VARCHAR(16777216) NULL,
	DEPENDENTS           NUMBER(38,0) NULL,
	EFFECTIVEDATE        DATE NULL,
	EFFECTIVETYPECODE    VARCHAR(16777216) NULL,
	EXPIRATIONDATE       DATE NULL,
	FASTTRACKRENEWALINDICATOR VARCHAR(16777216) NULL,
	GDRNAME              VARCHAR(16777216) NULL,
	INCEPTIONDATE        DATE NULL,
	INSURANCETYPE        VARCHAR(16777216) NULL,
	ISFULLCOVERAGE       VARCHAR(16777216) NULL,
	ISINTERNETSALE       VARCHAR(16777216) NULL,
	ISRENEWALQUESTIONNAIREREQUIRED VARCHAR(16777216) NULL,
	ISSHORTTERMPOLICY    VARCHAR(16777216) NULL,
	LASTCHANGEDINSURANCETYPE VARCHAR(16777216) NULL,
	LASTCHANGEDLIFESEGMENT VARCHAR(16777216) NULL,
	LASTCHANGEDLOBCODE   VARCHAR(16777216) NULL,
	LASTCHANGEDPRIMARYRATINGSTATE VARCHAR(16777216) NULL,
	LASTCHANGEDRISKSEGMENT VARCHAR(16777216) NULL,
	LASTCHANGEDWRITINGCOMPANY VARCHAR(16777216) NULL,
	LEGACYSYSTEMACTIVITYDATE TIMESTAMP_NTZ(9) NULL,
	LIFESEGMENT          VARCHAR(16777216) NULL,
	LIFESEGMENTINCEPTIONDATE DATE NULL,
	LOBCODE              VARCHAR(16777216) NULL,
	MINIMUMPREMIUM       FLOAT NULL,
	MINIMUMPREMIUMWRITTEN FLOAT NULL,
	NONCANCELLABLE       VARCHAR(16777216) NULL,
	NONOWNEDAUTOCOVERAGETYPE VARCHAR(16777216) NULL,
	OPTIONFORMRECEIVEDINDICATOR VARCHAR(16777216) NULL,
	OPTIONFORMREQUIREDINDICATOR VARCHAR(16777216) NULL,
	ORIGINALPOLICYEFFECTIVEDATE DATE NULL,
	PAPERLESSPOLICYSTATUS VARCHAR(16777216) NULL,
	PAPERLESSPOLICYSTATUSDATE DATE NULL,
	PARTIALPAPERLESSSTATUS VARCHAR(16777216) NULL,
	PAYPLAN              VARCHAR(16777216) NULL,
	POLICYBINDERDATETIMESTAMP TIMESTAMP_NTZ(9) NULL,
	POLICYCLAIMSIDENTIFIER VARCHAR(16777216) NULL,
	POLICYNUMBER         VARCHAR(16777216) NULL,
	POLICYTERMID         VARCHAR(16777216) NULL,
	PREMIUM              FLOAT NULL,
	PREMIUMCHANGE        FLOAT NULL,
	PREMIUMWRITTEN       FLOAT NULL,
	PRIMARYRATINGSTATE   VARCHAR(16777216) NULL,
	PRODUCT              VARCHAR(16777216) NULL,
	PRORATEDADJUSTMENTAMOUNT FLOAT NULL,
	PROXYSTATUSCOUNTER   NUMBER(38,0) NULL,
	RATINGTIER           VARCHAR(16777216) NULL,
	REASONAMENDMENTCODE  VARCHAR(16777216) NULL,
	REISSUEWITHLAPSEDATE DATE NULL,
	REISSUEWITHLAPSEEFFECTIVEDATE DATE NULL,
	RELATIONTOPRIORPOLICYAPPLICANT VARCHAR(16777216) NULL,
	RENEWALSTATUS        VARCHAR(16777216) NULL,
	REQUESTEDMAILPACKAGECODE VARCHAR(16777216) NULL,
	RETENTIONKEY         VARCHAR(16777216) NULL,
	RETURNEDMAILINDICATOR VARCHAR(16777216) NULL,
	RISKSEGMENT          VARCHAR(16777216) NULL,
	RISKSEGMENTINCEPTIONDATE DATE NULL,
	SALESTYPE            VARCHAR(16777216) NULL,
	SOURCEOFADVERTISING  VARCHAR(16777216) NULL,
	SOURCEOFBUSINESS     VARCHAR(16777216) NULL,
	STATESTANDARDPOLICYINDICATOR VARCHAR(16777216) NULL,
	STATETENURESTARTDATE DATE NULL,
	STATUS               VARCHAR(16777216) NULL,
	SUPPRESSDOCUMENTOUTPUT VARCHAR(16777216) NULL,
	TENUREPOLICYYEARS    VARCHAR(16777216) NULL,
	TERM                 NUMBER(38,0) NULL,
	TERMFACTOR           FLOAT NULL,
	TEXASPROXYSTATUS     VARCHAR(16777216) NULL,
	UMBRELLAREFERRAL     VARCHAR(16777216) NULL,
	WASBALANCEOVERRIDDEN VARCHAR(16777216) NULL,
	WRITINGCOMPANY       VARCHAR(16777216) NULL,
	POLICY_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_PEAK_POLICY
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE SAT_PEAK_RISK_COVERAGE
(
	POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK BINARY(16) NOT NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	POLICY_NUMBER        VARCHAR(16777216) NULL,
	POLICYREADONLYHISTORYID VARCHAR(16777216) NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICY_ID            VARCHAR(16777216) NULL,
	LINE_ID              VARCHAR(16777216) NULL,
	LINE_CHANGE          FLOAT NULL,
	LINE_WRITTEN         FLOAT NULL,
	RISK_ID              VARCHAR(16777216) NULL,
	RISK_DELETED         BOOLEAN NULL,
	COVERAGE_ID          VARCHAR(16777216) NULL,
	COVERAGE_DELETED     BOOLEAN NULL,
	ACTION               VARCHAR(16777216) NULL,
	CANCELPREM           FLOAT NULL,
	CHANGE               FLOAT NULL,
	COVERAGECODE         VARCHAR(16777216) NULL,
	COVERAGELEVEL        VARCHAR(16777216) NULL,
	EFFECTIVEDATE        DATE NULL,
	EFFECTIVETYPECODE    VARCHAR(16777216) NULL,
	INDICATOR            BOOLEAN NULL,
	LASTCHANGEDACTION    VARCHAR(16777216) NULL,
	LASTCHANGEDDATE      DATE NULL,
	LASTPREMIUMCHANGEDACTION VARCHAR(16777216) NULL,
	LASTPREMIUMCHANGEDDATE DATE NULL,
	MANUALLYENTEREDPREMIUM FLOAT NULL,
	OFFSET               FLOAT NULL,
	ONSET                FLOAT NULL,
	PREMIUM              FLOAT NULL,
	PREMIUMEFFECTIVETYPECODE VARCHAR(16777216) NULL,
	PREMIUMREASONAMENDMENTCODE VARCHAR(16777216) NULL,
	PRIOR                FLOAT NULL,
	PRIORTERM            FLOAT NULL,
	REASONAMENDMENTCODE  VARCHAR(16777216) NULL,
	TYPE                 VARCHAR(16777216) NULL,
	WRITTEN              FLOAT NULL,
	DEDUCTIBLE_ID        VARCHAR(16777216) NULL,
	DEDUCTIBLECODE       VARCHAR(16777216) NULL,
	LIMIT_ID             VARCHAR(16777216) NULL,
	LIMITCODE            VARCHAR(16777216) NULL
);

ALTER TABLE SAT_PEAK_RISK_COVERAGE
ADD PRIMARY KEY (POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK,LOAD_TIMESTAMP);

CREATE TABLE SAT_PEAK_TRANSACTION
(
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	HASHDIFF             BINARY(16) NULL,
	RECORD_SOURCE        VARCHAR(100) NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICYADMIN_ID       VARCHAR(16777216) NULL,
	TRANSACTIONS_ID      VARCHAR(16777216) NULL,
	TRANSACTION_ID       VARCHAR(16777216) NULL,
	CHARGE               FLOAT NULL,
	COMMITFAILURE        BOOLEAN NULL,
	COMMITSTATUS         VARCHAR(16777216) NULL,
	CONVERTEDTRANSACTIONTYPE VARCHAR(16777216) NULL,
	CREATEDDATE          TIMESTAMP_NTZ(9) NULL,
	CREATEDUSER          VARCHAR(16777216) NULL,
	DEPRECATEDBY         VARCHAR(16777216) NULL,
	DESTINATIONPOLICYID  VARCHAR(16777216) NULL,
	EFFECTIVEDATE        DATE NULL,
	EXPIRATIONDATE       DATE NULL,
	GLOBALBUSINESSPROCESSID VARCHAR(16777216) NULL,
	ISOUTOFSEQUENCE      VARCHAR(16777216) NULL,
	ISSUEDDATE           TIMESTAMP_NTZ(9) NULL,
	ISSUEDUSERNAME       VARCHAR(16777216) NULL,
	ISVISIBLETOCUSTOMER  VARCHAR(16777216) NULL,
	NEWPREMIUM           FLOAT NULL,
	OFFSETDATE           DATE NULL,
	ONSETBY              VARCHAR(16777216) NULL,
	ORIGINALCHARGE       FLOAT NULL,
	ORIGINALEFFECTIVEDATE DATE NULL,
	ORIGINALID           VARCHAR(16777216) NULL,
	POLICYMANUSCRIPTID   VARCHAR(16777216) NULL,
	POLICYNUMBER         VARCHAR(16777216) NULL,
	POLICYSTATUS         VARCHAR(16777216) NULL,
	PREVIOUSHISTORYID    VARCHAR(16777216) NULL,
	PRIORPREMIUM         FLOAT NULL,
	PRODUCT              VARCHAR(16777216) NULL,
	PRODUCTISREADONLY    BOOLEAN NULL,
	PRORATEFACTOR        FLOAT NULL,
	QUOTETYPE            VARCHAR(16777216) NULL,
	RATINGACTIVITYID     VARCHAR(16777216) NULL,
	REPLACEMENTID        VARCHAR(16777216) NULL,
	REVISIONOF           VARCHAR(16777216) NULL,
	SCHEDULEDATE         DATE NULL,
	SHORTRATEFACTOR      FLOAT NULL,
	SOURCEPOLICYID       VARCHAR(16777216) NULL,
	STATE                VARCHAR(16777216) NULL,
	STATUS               VARCHAR(16777216) NULL,
	STATUSUSER           VARCHAR(16777216) NULL,
	STATUSUSERCONTEXT    VARCHAR(16777216) NULL,
	TERMEFFECTIVEDATE    DATE NULL,
	TERMPREMIUM          FLOAT NULL,
	TRANSACTIONDATE      TIMESTAMP_NTZ(9) NULL,
	TYPE                 VARCHAR(16777216) NULL,
	TYPECAPTION          VARCHAR(16777216) NULL,
	WAIVECHARGECHECK     BOOLEAN NULL,
	POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_PEAK_TRANSACTION
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK);

CREATE TABLE SAT_PEAK_VEHICLE
(
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_NUMBER        VARCHAR(16777216) NOT NULL,
	POLICYREADONLYHISTORYID VARCHAR(16777216) NOT NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICY_ID            VARCHAR(16777216) NULL,
	LINE_ID              VARCHAR(16777216) NULL,
	LINE_CHANGE          FLOAT NULL,
	LINE_WRITTEN         FLOAT NULL,
	RISK_ID              VARCHAR(16777216) NULL,
	RISK_DELETED         BOOLEAN NULL,
	VEHICLE_ID           VARCHAR(16777216) NULL,
	VEHICLE_DELETED      BOOLEAN NULL,
	ACTION               VARCHAR(16777216) NULL,
	ASSIGNEDDRIVER       VARCHAR(16777216) NULL,
	EFFECTIVEDATE        DATE NULL,
	EFFECTIVETYPECODE    VARCHAR(16777216) NULL,
	LASTCHANGEDACTION    VARCHAR(16777216) NULL,
	LASTCHANGEDDATE      DATE NULL,
	REASONAMENDMENTCODE  VARCHAR(16777216) NULL,
	REMOVALREASON        VARCHAR(16777216) NULL,
	REMOVEDDATE          DATE NULL,
	REPLACEDVEHICLEID    VARCHAR(16777216) NULL,
	STORAGEPLAN          VARCHAR(16777216) NULL,
	VEHICLEHISTORYDAMAGEDCODE VARCHAR(16777216) NULL,
	VEHICLEHISTORYDAMAGEDIMPACTCODE VARCHAR(16777216) NULL,
	VEHICLEHISTORYORDERDATE DATE NULL,
	VEHICLEHISTORYRATEBOOKDATE DATE NULL,
	VEHICLEHISTORYTITLETRANSFERREDCODE VARCHAR(16777216) NULL,
	VEHICLEHISTORYTITLETRANSFERREDIMPACTCODE VARCHAR(16777216) NULL,
	VEHICLELOCATION      VARCHAR(16777216) NULL,
	VEHICLENUMBER        NUMBER(38,0) NULL,
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_PEAK_VEHICLE
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_TRANSACTION_INSURES_VEHICLE_HK);

CREATE TABLE SAT_PEAK_VEHICLE_PII
(
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	REPLACEDVIN          VARCHAR(16777216) NULL,
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_PEAK_VEHICLE_PII
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_TRANSACTION_INSURES_VEHICLE_HK);

CREATE TABLE SAT_PEAK_VEHICLE_REGISTRATIONOWNERSHIP
(
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	VEHICLEAGE           VARCHAR(16777216) NULL,
	POLICY_NUMBER        VARCHAR(16777216) NULL,
	POLICYREADONLYHISTORYID VARCHAR(16777216) NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICY_ID            VARCHAR(16777216) NULL,
	LINE_ID              VARCHAR(16777216) NULL,
	LINE_CHANGE          FLOAT NULL,
	LINE_WRITTEN         FLOAT NULL,
	RISK_ID              VARCHAR(16777216) NULL,
	RISK_DELETED         BOOLEAN NULL,
	VEHICLE_ID           VARCHAR(16777216) NULL,
	VEHICLE_DELETED      BOOLEAN NULL,
	REGISTRATIONOWNERSHIP_ID VARCHAR(16777216) NULL,
	REGISTRATIONOWNERSHIP_DELETED BOOLEAN NULL,
	BUSINESSOWNERCLIENTKEY VARCHAR(16777216) NULL,
	BUSINESSOWNERINDICATOR VARCHAR(16777216) NULL,
	CONTENTSVALUE        FLOAT NULL,
	COOWNER              VARCHAR(16777216) NULL,
	COSTNEW              FLOAT NULL,
	ISNEWVEHICLE         VARCHAR(16777216) NULL,
	ORIGINALOWNER        VARCHAR(16777216) NULL,
	OWNER                VARCHAR(16777216) NULL,
	OWNERSHIPTYPE        VARCHAR(16777216) NULL,
	PURCHASEDATE         DATE NULL,
	PURCHASEPRICE        FLOAT NULL,
	REGISTEREDSTATE      VARCHAR(16777216) NULL,
	TAGNUMBER            VARCHAR(16777216) NULL,
	TAGSURRENDERDATE     DATE NULL,
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_PEAK_VEHICLE_REGISTRATIONOWNERSHIP
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_TRANSACTION_INSURES_VEHICLE_HK);

CREATE TABLE SAT_PEAK_VEHICLE_USAGEDETAILS
(
	ESTIMATEDANNUALMILEAGE NUMBER(38,0) NULL,
	ODOMETERREADING      NUMBER(38,0) NULL,
	ODOMETERDATE         DATE NULL,
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_NUMBER        VARCHAR(16777216) NOT NULL,
	POLICYREADONLYHISTORYID VARCHAR(16777216) NOT NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICY_ID            VARCHAR(16777216) NULL,
	LINE_ID              VARCHAR(16777216) NULL,
	LINE_CHANGE          FLOAT NULL,
	LINE_WRITTEN         FLOAT NULL,
	RISK_ID              VARCHAR(16777216) NULL,
	RISK_DELETED         BOOLEAN NULL,
	VEHICLE_ID           VARCHAR(16777216) NULL,
	VEHICLE_DELETED      BOOLEAN NULL,
	USAGEDETAILS_ID      VARCHAR(16777216) NULL,
	USAGEDETAILS_DELETED BOOLEAN NULL,
	ALLOTHERCLASSINDICATOR VARCHAR(16777216) NULL,
	ANNUALMILEAGERANGE   VARCHAR(16777216) NULL,
	BUSINESSUSAGEPERCENTAGE VARCHAR(16777216) NULL,
	BUSINESSUSAGETYPE    VARCHAR(16777216) NULL,
	CALCULATEDCOMMUTEMILEAGE NUMBER(38,0) NULL,
	DAYSTOSCHOOL         NUMBER(38,0) NULL,
	DAYSTOWORK           NUMBER(38,0) NULL,
	EXCESSVEHICLEINDICATOR VARCHAR(16777216) NULL,
	ISACCEPTABLEBUSINESSUSAGE VARCHAR(16777216) NULL,
	ISVEHICLEELIGIBLE    VARCHAR(16777216) NULL,
	MILESTOSCHOOL        NUMBER(38,0) NULL,
	MILESTOWORK          NUMBER(38,0) NULL,
	PRIORTERMANNUALMILEAGE NUMBER(38,0) NULL,
	RATINGVEHICLEUSE     VARCHAR(16777216) NULL,
	RVDAYSOFUSE          NUMBER(38,0) NULL,
	USAGE                VARCHAR(16777216) NULL,
	WASUSAGECHANGED      VARCHAR(16777216) NULL
);

ALTER TABLE SAT_PEAK_VEHICLE_USAGEDETAILS
ADD PRIMARY KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK,LOAD_TIMESTAMP);

CREATE TABLE SAT_PEAK_VEHICLE_VINSYMBOL
(
	HASHDIFF             DATE NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL,
	POLICY_NUMBER        VARCHAR(16777216) NOT NULL,
	POLICYREADONLYHISTORYID VARCHAR(16777216) NOT NULL,
	INCLUDEDELETED       BOOLEAN NULL,
	PRODUCTKIND          VARCHAR(16777216) NULL,
	RETRIEVEPOLICY_ID    VARCHAR(16777216) NULL,
	DATA_ID              VARCHAR(16777216) NULL,
	POLICY_ID            VARCHAR(16777216) NULL,
	LINE_ID              VARCHAR(16777216) NULL,
	LINE_CHANGE          FLOAT NULL,
	LINE_WRITTEN         FLOAT NULL,
	RISK_ID              VARCHAR(16777216) NULL,
	RISK_DELETED         BOOLEAN NULL,
	VEHICLE_ID           VARCHAR(16777216) NULL,
	VEHICLE_DELETED      BOOLEAN NULL,
	VINSYMBOL_ID         VARCHAR(16777216) NULL,
	VINSYMBOL_DELETED    BOOLEAN NULL,
	ANTILOCKBRAKES       VARCHAR(16777216) NULL,
	ANTILOCKBRAKETYPE    VARCHAR(16777216) NULL,
	ANTITHEFTDEVICE      VARCHAR(16777216) NULL,
	CLASSASSIGNMETHOD    VARCHAR(16777216) NULL,
	CLASSCODE            VARCHAR(16777216) NULL,
	COLLISIONSYMBOL      VARCHAR(16777216) NULL,
	COLLISIONSYMBOLOVERRIDE VARCHAR(16777216) NULL,
	COMPREHENSIVESYMBOL  VARCHAR(16777216) NULL,
	COMPREHENSIVESYMBOLOVERRIDE VARCHAR(16777216) NULL,
	COSTNEWWITHCUSTOMIZATIONS VARCHAR(16777216) NULL,
	DAYTIMERUNNINGLIGHTS VARCHAR(16777216) NULL,
	DESCRIPTION          VARCHAR(16777216) NULL,
	ENGINETYPE           VARCHAR(16777216) NULL,
	HASFAILEDVINVALIDATION VARCHAR(16777216) NULL,
	HIGHTHEFTVEHICLE     VARCHAR(16777216) NULL,
	ISOBODYSTYLE         VARCHAR(16777216) NULL,
	LASTCHANGEDCOLLISIONSYMBOL VARCHAR(16777216) NULL,
	LASTCHANGEDCOMPREHENSIVESYMBOL VARCHAR(16777216) NULL,
	LASTCHANGEDLIABILITYSYMBOL VARCHAR(16777216) NULL,
	LEGACYVEHICLETYPECODE VARCHAR(16777216) NULL,
	LIABILITYSYMBOL      VARCHAR(16777216) NULL,
	LIABILITYSYMBOLOVERRIDE VARCHAR(16777216) NULL,
	MAKE                 VARCHAR(16777216) NULL,
	MODEL                VARCHAR(16777216) NULL,
	PASSIVERESTRAINTDEVICE VARCHAR(16777216) NULL,
	PERFORMANCE          VARCHAR(16777216) NULL,
	VEHICLERATINGCODE    VARCHAR(16777216) NULL,
	VEHICLETYPE          VARCHAR(16777216) NULL,
	VINEDITOVERRIDE      VARCHAR(16777216) NULL,
	YEAR                 VARCHAR(16777216) NULL
);

ALTER TABLE SAT_PEAK_VEHICLE_VINSYMBOL
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_TRANSACTION_INSURES_VEHICLE_HK);

CREATE TABLE SAT_PEAK_VEHICLE_VINSYMBOL_PII
(
	VIN                  VARCHAR(16777216) NULL,
	HASHDIFF             BINARY(16) NULL,
	EFFECTIVE_TIMESTAMP  TIMESTAMP NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	POLICY_TRANSACTION_INSURES_VEHICLE_HK BINARY(16) NOT NULL
);

ALTER TABLE SAT_PEAK_VEHICLE_VINSYMBOL_PII
ADD PRIMARY KEY (LOAD_TIMESTAMP,POLICY_TRANSACTION_INSURES_VEHICLE_HK);

ALTER TABLE LNK_POLICY_HAS_POLICY_TRANSACTION
ADD CONSTRAINT R_6 FOREIGN KEY (POLICY_HK) REFERENCES HUB_POLICY (POLICY_HK);

ALTER TABLE LNK_POLICY_HAS_POLICY_TRANSACTION
ADD CONSTRAINT R_3 FOREIGN KEY (POLICY_TRANSACTION_HK) REFERENCES HUB_POLICY_TRANSACTION (POLICY_TRANSACTION_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_POLICY_TRANSACTION
ADD CONSTRAINT R_73 FOREIGN KEY (TRANSACTION_HK) REFERENCES HUB_POLICY_TRANSACTION (POLICY_TRANSACTION_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_POLICY_TRANSACTION
ADD CONSTRAINT R_74 FOREIGN KEY (RELATED_TRANSACTION_HK) REFERENCES HUB_POLICY_TRANSACTION (POLICY_TRANSACTION_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE
ADD CONSTRAINT R_13 FOREIGN KEY (VEHICLE_HK) REFERENCES HUB_VEHICLE (VEHICLE_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE
ADD CONSTRAINT R_15 FOREIGN KEY (COVERAGE_HK) REFERENCES HUB_COVERAGE (COVERAGE_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE
ADD CONSTRAINT R_71 FOREIGN KEY (RISK_HK) REFERENCES HUB_RISK (RISK_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE
ADD CONSTRAINT R_72 FOREIGN KEY (POLICY_TRANSACTION_HK) REFERENCES HUB_POLICY_TRANSACTION (POLICY_TRANSACTION_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_INSURES_VEHICLE
ADD CONSTRAINT R_38 FOREIGN KEY (VEHICLE_HK) REFERENCES HUB_VEHICLE (VEHICLE_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_INSURES_VEHICLE
ADD CONSTRAINT R_68 FOREIGN KEY (POLICY_TRANSACTION_HK) REFERENCES HUB_POLICY_TRANSACTION (POLICY_TRANSACTION_HK);

ALTER TABLE LNK_POLICY_TRANSACTION_INSURES_VEHICLE
ADD CONSTRAINT R_70 FOREIGN KEY (RISK_HK) REFERENCES HUB_RISK (RISK_HK);

ALTER TABLE SAT_BR_COVERAGE_REF_COV_DED_LIT
ADD CONSTRAINT R_54 FOREIGN KEY (COVERAGE_HK) REFERENCES HUB_COVERAGE (COVERAGE_HK);

ALTER TABLE SAT_BR_COVERAGE_REF_COV_LIMIT_LIT
ADD CONSTRAINT R_63 FOREIGN KEY (COVERAGE_HK) REFERENCES HUB_COVERAGE (COVERAGE_HK);

ALTER TABLE SAT_BR_COVERAGE_REF_COV_MNEMONICS
ADD CONSTRAINT R_62 FOREIGN KEY (COVERAGE_HK) REFERENCES HUB_COVERAGE (COVERAGE_HK);

ALTER TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_RATING_STRUCTURE
ADD CONSTRAINT R_79 FOREIGN KEY (POLICY_HAS_POLICY_TRANSACTION_HK) REFERENCES LNK_POLICY_HAS_POLICY_TRANSACTION (POLICY_HAS_POLICY_TRANSACTION_HK);

ALTER TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_REF_PRODCT_RSK_TYP_TBL
ADD CONSTRAINT R_77 FOREIGN KEY (POLICY_HAS_POLICY_TRANSACTION_HK) REFERENCES LNK_POLICY_HAS_POLICY_TRANSACTION (POLICY_HAS_POLICY_TRANSACTION_HK);

ALTER TABLE SAT_BR_POLICY_HAS_POLICY_TRANSACTION_RISK_SEGMENT
ADD CONSTRAINT R_78 FOREIGN KEY (POLICY_HAS_POLICY_TRANSACTION_HK) REFERENCES LNK_POLICY_HAS_POLICY_TRANSACTION (POLICY_HAS_POLICY_TRANSACTION_HK);

ALTER TABLE SAT_PEAK_POLICY
ADD CONSTRAINT R_76 FOREIGN KEY (POLICY_HAS_POLICY_TRANSACTION_HK) REFERENCES LNK_POLICY_HAS_POLICY_TRANSACTION (POLICY_HAS_POLICY_TRANSACTION_HK);

ALTER TABLE SAT_PEAK_RISK_COVERAGE
ADD CONSTRAINT R_23 FOREIGN KEY (POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK) REFERENCES LNK_POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE (POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK);

ALTER TABLE SAT_PEAK_TRANSACTION
ADD CONSTRAINT R_75 FOREIGN KEY (POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK) REFERENCES LNK_POLICY_TRANSACTION_HAS_POLICY_TRANSACTION (POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK);

ALTER TABLE SAT_PEAK_VEHICLE
ADD CONSTRAINT R_57 FOREIGN KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK) REFERENCES LNK_POLICY_TRANSACTION_INSURES_VEHICLE (POLICY_TRANSACTION_INSURES_VEHICLE_HK);

ALTER TABLE SAT_PEAK_VEHICLE_PII
ADD CONSTRAINT R_58 FOREIGN KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK) REFERENCES LNK_POLICY_TRANSACTION_INSURES_VEHICLE (POLICY_TRANSACTION_INSURES_VEHICLE_HK);

ALTER TABLE SAT_PEAK_VEHICLE_REGISTRATIONOWNERSHIP
ADD CONSTRAINT R_44 FOREIGN KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK) REFERENCES LNK_POLICY_TRANSACTION_INSURES_VEHICLE (POLICY_TRANSACTION_INSURES_VEHICLE_HK);

ALTER TABLE SAT_PEAK_VEHICLE_USAGEDETAILS
ADD CONSTRAINT R_39 FOREIGN KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK) REFERENCES LNK_POLICY_TRANSACTION_INSURES_VEHICLE (POLICY_TRANSACTION_INSURES_VEHICLE_HK);

ALTER TABLE SAT_PEAK_VEHICLE_VINSYMBOL
ADD CONSTRAINT R_45 FOREIGN KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK) REFERENCES LNK_POLICY_TRANSACTION_INSURES_VEHICLE (POLICY_TRANSACTION_INSURES_VEHICLE_HK);

ALTER TABLE SAT_PEAK_VEHICLE_VINSYMBOL_PII
ADD CONSTRAINT R_46 FOREIGN KEY (POLICY_TRANSACTION_INSURES_VEHICLE_HK) REFERENCES LNK_POLICY_TRANSACTION_INSURES_VEHICLE (POLICY_TRANSACTION_INSURES_VEHICLE_HK);