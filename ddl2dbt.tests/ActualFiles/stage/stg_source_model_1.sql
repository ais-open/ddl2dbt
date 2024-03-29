{{ config(tags = ['tag']) }}

{%- set metadata_yaml -%}
source_model:
  STG: 'MODEL_1'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!CUST'
  EFFECTIVE_TIMESTAMP: 'TO_TIMESTAMP(EFFECTIVEDATE)'
hashed_columns:
  CUSTOMER_HK: 'CUSTOMER_NO'
  NATION_DETAILS_HK: 'CUSTOMER_NO'
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'CUSTOMER_HK'
      - 'LOAD_TIMESTAMP'
      - 'RECORD_SOURCE'
      - 'CUSTOMER_NO'
      - 'SampleColumn1'
      - 'SampleColumn2'
      - 'NATION_DETAILS_HK'
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