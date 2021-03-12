{{ config(tags = ['core']) }}

{%- set metadata_yaml -%}
source_model:
  ???: '???'
include_source_columns: true
derived_columns:
hashed_columns:
  POLICY_TRANSACTION_HAS_POLICY_TRANSACTION_HK: ''
  TRANSACTION_HK: 'POLICYREADONLYHISTORYID'
  RELATED_TRANSACTION_HK: 'HISTORYID'
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