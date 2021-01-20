{{ config(tags = ['policy']) }}

{%- set metadata_yaml -%}
source_model: 'stg_peak_vehicle_registrationownership_pre'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!PEAK'
  EFFECTIVE_TIMESTAMP: 'EFFECTIVEDATE'
hashed_columns:
  VEHICLE_HK: 'VEHICLE_ID'
  POLICY_HK: 'POLICY_NUMBER'
  TRANSACTION_HK: 'POLICYREADONLYHISTORYID'
  POLICY_INSURES_VEHICLE_HK:
    - 'POLICY_HK'
    - 'VEHICLE_HK'
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