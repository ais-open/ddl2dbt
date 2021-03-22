# ddl2dbt
dbt (https://docs.getdbt.com) is an open source platform for data warehouse automation. 

dbtvault (https://dbtvault.readthedocs.io/) macro, enables DataVault 2.0 architecture based data warehouse.  A typical dbt project using dbtvault macro, consists of .sql and .yml files.  

ddl2dbt is a CLI, written in C# that automates generation of dbt and dbtvault macro based .sql and .yml source code.  
It takes Sql Data Defintion Language (DDL) file and a CSV file containing source to target mapping as input and generates hub, links, satellites and stage .sql and .yml files.  

**Usage:**

  ddl2dbt [options]

**Options:**

-  -d, --ddl  (REQUIRED) :                           DDL File Path

 - -c, --csv                     :                   CSV File Path. if not provided only hub,sat,lnk file generation will bee applicable.

-  -m, --models                   :             Options include: hub,sat,lnk and stg or * (Default) . Use comma to select multiple options Ex: sat,hub

-  -o, --outputPath  (REQUIRED)    :             Output File Path

-  --version                      :             Show version information

-  -?, -h, --help                  :            Show help and usage information

**Example:**
ddl2dbt --ddl "D:\ddlfilename.ddl" -m * -c "D:\csvfilename.csv" -o "D:\ddl\\" 

**Video Demo:** https://youtu.be/UBYfKvcn3Wo

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
It is not required to run the ddl2dbt applications, but without the csv the stage files cannot be generated. For Hub, Lnk and Sat files the csv is used only to populate tags and source_model so if no csv is provided then the hub, lnk and sat files will still be generated with '???' for the tags and source_model.  
Eg:
|Table Name|Tags|Table Definition|Column Name|HASHDIFF|Column Definition|Source Model|Derived/Hashed Columns|
| ------ | ------ | ------ | ------ | ------ | ------ | ------ | ------ |
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|CUSTOMER_HK|TRUE|"This is ""CUSTOMER_HK"" column description"|STG.SOURCE.MODEL_1|MD5(CUSTOMER_NO)|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|LOAD_TIMESTAMP|TRUE||STG.SOURCE.MODEL_1|CURRENT_TIMESTAMP()|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|RECORD_SOURCE|TRUE||STG.SOURCE.MODEL_1|CUST|
|HUB_CUSTOMER|tag|This is HUB_Customer table definition|CUSTOMER_NO|TRUE||STG.SOURCE.MODEL_1|

#### CSV Columns:
| Columns | Use |
| ------ | ------ |
| Table Name | This column specifies the table name |
| Tags | This column specifies the tags for all the tables |
| Table Definition | This column specifies the Table Definition. It will be used to populate table details in the yaml file |
| Column Name | This column specifies the different columns present |
| HASHDIFF | This column contains a boolean value if found to be 'True' then only its corresponding 'Column Name' will be used to populate the HASHDIFF part fo the stage file.
| Column Definition | This column specifies the Column Definition. It will be used to populate column details in the yaml file |
| Source Model | For each unique value in this column a new stage file will be generated. This column is also used to populate 'source_model' for the all kinds of files |
| Derived/Hashed Columns | This column is used to populate derived and hashed columns of the stage file |

**If  you want to use a csv with different column names then the new column names can be specified in the 'FieldValue' property of the appSettings.json file. The appSettings.json file is part of the release and needs to be in the same directory as ddl2dbt.exe.**
  
## Output Files:
**1) hub_customer.sql**
```sh
{{ config(tags = ['tag']) }}

{%- set metadata_yaml -%}
source_model: 'stg_source_model_1'
src_pk: 'CUSTOMER_HK'
src_nk: 'CUSTOMER_NO'
src_ldts: 'LOAD_TIMESTAMP'
src_source: 'RECORD_SOURCE'
{%- endset -%}

{% set metadata_dict = fromyaml(metadata_yaml) -%}
{% set source_model = metadata_dict['source_model'] -%}
{% set src_pk = metadata_dict['src_pk'] -%}
{% set src_nk = metadata_dict['src_nk'] -%}
{% set src_ldts = metadata_dict['src_ldts'] -%}
{% set src_source = metadata_dict['src_source'] -%}

{{ dbtvault.hub(src_pk=src_pk, 
                src_nk=src_nk, 
                src_ldts=src_ldts,
                src_source=src_source, 
                source_model=source_model) }}
```
**2) hub_customer.yml**
```sh
version: 2
models:
  - name: hub_customer 
    description: "This is HUB_Customer table definition"
    columns:
      - name: CUSTOMER_HK
        description: "This is CUSTOMER_HK column description"
        tests:
          - unique
          - not_null
      - name: LOAD_TIMESTAMP
        description: ""
        tests:
          - unique
          - not_null
      - name: RECORD_SOURCE
        description: ""
        tests:
          - unique
          - not_null
      - name: CUSTOMER_NO
        description: ""
```
**3) stg_source_model_1.sql**
```sh
{{ config(tags = ['tag']) }}

{%- set metadata_yaml -%}
source_model:
  STG: 'Model_1'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!CUST'
hashed_columns:
  CUSTOMER_HK: 'CUSTOMER_NO'
{%- endset -%}

{% set metadata_dict = fromyaml(metadata_yaml) -%}

{% set source_model = metadata_dict['source_model'] -%}
{% set include_source_columns = metadata_dict['include_source_columns'] -%}
{% set hashed_columns = metadata_dict['hashed_columns'] -%}
{% set derived_columns = metadata_dict['derived_columns'] -%}

WITH stg AS (
  {{ dbtvault.stage(include_source_columns=include_source_columns,
                      source_model=source_model,
                      hashed_columns=hashed_columns,
                      derived_columns=derived_columns) }} 
  {{ limit_records() }}
),
stg_loadtimestamp AS (
  {{ append_loadtimestamp(stage_name = 'stg') }}
)

SELECT * FROM stg_loadtimestamp
```
