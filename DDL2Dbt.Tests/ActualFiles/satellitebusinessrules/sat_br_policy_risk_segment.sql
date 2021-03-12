{{ config(tags = ['core']) }}

{%- set metadata_yaml -%}
source_model: 'stg_sat_br_policy_risk_segment'
src_pk: 'POLICY_HK'
src_hashdiff: 'HASHDIFF'
src_eff: 'EFFECTIVE_TIMESTAMP'
src_ldts: 'LOAD_TIMESTAMP'
src_source: 'RECORD_SOURCE'
src_payload:
  - 'RISK_SEGMENT'
  - 'TRANSACTION_HK'
{%- endset -%}

{% set metadata_dict = fromyaml(metadata_yaml) -%}
{% set source_model = metadata_dict['source_model'] -%}
{% set src_pk = metadata_dict['src_pk'] -%}
{% set src_hashdiff = metadata_dict['src_hashdiff'] -%}
{% set src_payload = metadata_dict['src_payload'] -%}
{% set src_eff = metadata_dict['src_eff'] -%}
{% set src_ldts = metadata_dict['src_ldts'] -%}
{% set src_source = metadata_dict['src_source'] -%}

{{ dbtvault.sat(src_pk=src_pk, 
                src_hashdiff=src_hashdiff, 
                src_payload=src_payload,
                src_eff=src_eff, 
                src_ldts=src_ldts,
                src_source=src_source,
                source_model=source_model) }}