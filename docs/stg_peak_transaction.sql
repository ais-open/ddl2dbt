{{ config(tags = ['policy']) }}

{%- set metadata_yaml -%}
source_model:
  PEAK_POLICY_CONFORMED: 'TRANSACTION'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!PEAK'
  PREVIOUS_TRANSACTION_REFERENCE_ID: 'PREVIOUSHISTORYID'
  EFFECTIVE_TIMESTAMP: 'EFFECTIVEDATE'
  TRANSACTION_NK: 'TRANSACTION_ID'
hashed_columns:
  TRANSACTION_HK: 'POLICYREADONLYHISTORYID'
  POLICY_HK: 'POLICY_NUMBER'
  PREVIOUS_TRANSACTION_HK: 'PREVIOUSHISTORYID'
  POLICY_HAS_TRANSACTION_HK:
    - 'POLICY_HK'
    - 'TRANSACTION_HK'
  POLICY_HAS_PREVIOUS_TRANSACTION_HK:
    - 'POLICY_NUMBER'
    - 'PREVIOUSHISTORYID'
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

SELECT * FROM stg_loadtimestamp