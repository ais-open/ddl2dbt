{{ config(tags = ['reference']) }}

{%- set metadata_yaml -%}
source_model: 
  RDS_REFERENCE_CONFORMED: 'COV_DED_LIT'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!RDS'
  REFERENCE_RULE_TABLE_NAME: '!SAT_RDS_COV_DED_LIT'
hashed_columns:
  HUB_REFERENCE_RULE_HK: 'REFERENCE_RULE_TABLE_NAME'
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'VEH_COV_DED_CD'
      - 'RISK_STATE_CD'
      - 'PROJECT_ID'
      - 'PADL_DED_DES'
      - 'LGG_CD2'
      - 'LEGACY_DED_DESC'
      - 'BUS_SRC_APP_ID'
      - 'ACTION_IND'
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
