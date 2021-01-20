{{ config(tags = ['policy', 'reference']) }}

{%- set metadata_yaml -%}
source_model: 
  RDS_REFERENCE_CONFORMED: 'PRODCT_RSK_TYP_TBL'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!RDS'
  REFERENCE_RULE_TABLE_NAME: '!PRODCT_RSK_TYP_TBL'
hashed_columns:
  HUB_REFERENCE_RULE_HK: 'REFERENCE_RULE_TABLE_NAME'
  HASHDIFF:
    is_hashdiff: true
    exclude_columns: true
    columns:
      - 'CONFORMED_DIGEST'
      - 'CONFORMED_INGESTION_TS'
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

SELECT LOAD_TIMESTAMP AS EFFECTIVE_TIMESTAMP, stg_loadtimestamp.* FROM stg_loadtimestamp