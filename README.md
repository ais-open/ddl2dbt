# ddl2dbt

Usage:


Example:
.\Ddl2Dbt.exe --ddl "D:\filename.ddl" -m * -o "D:\ddl\\"


Usage:
  Ddl2Dbt [options]

Options:

-  -d, --ddl  (REQUIRED) :                           DDL File Path

 - -c, --csv                     :                   CSV File Path. if not provided only hub,sat,lnk file generation will bee applicable.

-  -m, --models                   :             Options include: hub,sat,lnk and stg or * (Default) . Use comma to select multiple options Ex: sat,hub

-  -o, --outputPath  (REQUIRED)    :             Output File Path

-  --version                      :             Show version information

-  -?, -h, --help                  :            Show help and usage information

### DDL File :
It is required to run the ddl2dbt application. This file will contain the ddl statements.
The DDL file can be of type text, sql or ddl.  Accepted extentions are: '.txt', '.sql' and '.ddl'.
Eg:
```sh
CREATE TABLE HUB_CUSTOMER
(
	CUSTOMER_HK            BINARY() NOT NULL,
	LOAD_TIMESTAMP       TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
	RECORD_SOURCE        VARCHAR(100) NULL,
	CUSTOMER_NO        VARCHAR(50) NULL
);

ALTER TABLE HUB_CUSTOMER
ADD PRIMARY KEY (CUSTOMER_HK);
```

### CSV File:
It is not required to run the ddl2dbt applications, but without the csv the stage files cannot be generated.
Eg:
|Table Name|Tags|Table Definition|Column Name|HASHDIFF|Column Definition|Source Model|Derived/Hashed Columns|
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|CUSTOMER_HK|TRUE|"This is ""CUSTOMER_HK"" column description"|STG.SOURCE.MODEL_1|MD5(CUSTOMER_NO)|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|LOAD_TIMESTAMP|TRUE||STG.SOURCE.MODEL_1|CURRENT_TIMESTAMP()|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|RECORD_SOURCE|TRUE||STG.SOURCE.MODEL_1|CUST|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|CUSTOMER_NO|TRUE||STG.SOURCE.MODEL_1|
