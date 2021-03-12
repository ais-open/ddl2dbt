{{ config(tags = ['reference']) }}

{%- set metadata_yaml -%}
source_model: 
  RDS_REFERENCE_CONFORMED: 'COV_MNEMONICS'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!RDS'
  REFERENCE_RULE_TABLE_NAME: '!SAT_RDS_COV_MNEMONICS'
hashed_columns:
  HUB_REFERENCE_RULE_HK: 'REFERENCE_RULE_TABLE_NAME'
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'RISK_STATE_CD'
      - 'PA_VEH_COV_CD'
      - 'BUS_SRC_APP_ID'
      - 'LGG_CD2'
      - 'PVCL_VEH_COV_DES'
      - 'SHORT_COV_DES'
      - 'COV_POS'
      - 'LEGACY_COV_BACS_CD'
      - 'LEGACY_COV_DESC'
      - 'DSPLY_LEVEL_CD'
      - 'STATED_AMT_IND'
      - 'NON_DED_DESC_IND'
      - 'PROJECT_ID'
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
