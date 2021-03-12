{{ config(tags = ['policy', 'reference', 'businessrule']) }}

{%- set metadata_yaml -%}
source_model: 'stg_sat_br_policy_ref_prodct_rsk_type_tbl_pre'
include_source_columns: true
hashed_columns:
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'POLICY_HK'
      - 'LOAD_TIMESTAMP'
      - 'EFFECTIVE_TIMESTAMP'
      - 'RECORD_SOURCE'
      - 'TRANSACTION_HK'
      - 'RISK_TYPE'
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
)

SELECT * FROM stg