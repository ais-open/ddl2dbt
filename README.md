# ddl2dbt

Usage:

  Ddl2Dbt [options]

Options:

-  -d, --ddl  (REQUIRED) :                           DDL File Path

 - -c, --csv                     :                   CSV File Path. if not provided only hub,sat,lnk file generation will bee applicable.

-  -m, --models                   :             Options include: hub,sat,lnk and stg or * (Default) . Use comma to select multiple options Ex: sat,hub

-  -o, --outputPath  (REQUIRED)    :             Output File Path

-  --version                      :             Show version information

-  -?, -h, --help                  :            Show help and usage information

Example:
Ddl2Dbt --ddl "D:\ddlfilename.ddl" -m * -c "D:\csvfilename.csv" -o "D:\ddl\\" 

### DDL File :
It is required to run the ddl2dbt application. This file will contain the ddl statements.
The DDL file can be of type text, sql or ddl.  Accepted file-extentions are: '.txt', '.sql' and '.ddl'.  
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
It is not required to run the ddl2dbt applications, but without the csv the stage files cannot be generated. For Hub, Lnk and Sat files the csv is used only to populate tags so if no csv is provided then the hub, lnk and sat files will still be generated with '???' for the tags.  
Eg:
|Table Name|Tags|Table Definition|Column Name|HASHDIFF|Column Definition|Source Model|Derived/Hashed Columns|
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|CUSTOMER_HK|TRUE|"This is ""CUSTOMER_HK"" column description"|STG.SOURCE.MODEL_1|MD5(CUSTOMER_NO)|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|LOAD_TIMESTAMP|TRUE||STG.SOURCE.MODEL_1|CURRENT_TIMESTAMP()|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|RECORD_SOURCE|TRUE||STG.SOURCE.MODEL_1|CUST|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|CUSTOMER_NO|TRUE||STG.SOURCE.MODEL_1|

#### CSV Columns:
| Columns | Type | Use |
| ------ | ------ | ------ |
| Table Name | Required | This column specifies the table name |
| Tags | Required | This column specifies the tags for all the tables |
| Table Definition | Optional | This column specifies the Table Definition. It will be used to populate table details in the yaml file |
| Column Name | Required | This column specifies the different columns present |
| HASHDIFF | Required | This column contains a boolean value if found to be 'True' then only its corresponding 'Column Name' will be used to populate the HASHDIFF part fo the stage file.
| Column Definition | Optional | This column specifies the Column Definition. It will be used to populate column details in the yaml file |
| Source Model | Required | This column is used to populate 'source_model' for the stage files |
| Derived/Hashed Columns | Required | This column is used to populate derived and hashed columns of the stage file |

**If  you want to use a csv with different column names then the new column names can be specified in the appSettings.json file.**