{{ config(tags = ['reference']) }}

{%- set metadata_yaml -%}
source_model: 'stg_sat_rds_cov_limit_lit'
src_pk: 'HUB_REFERENCE_RULE_HK'
src_hashdiff: 'HASHDIFF'
src_eff: 'EFFECTIVE_TIMESTAMP'
src_ldts: 'LOAD_TIMESTAMP'
src_source: 'RECORD_SOURCE'
src_payload:
  - 'RISK_STATE_CD'
  - 'VEH_COV_LIM_CD'
  - 'PA_VEH_COV_CD'
  - 'BUS_SRC_APP_ID'
  - 'LGG_CD2'
  - 'PALL_LIMIT_DES'
  - 'LEGACY_LIMIT_DESC'
  - 'LIMIT_WGT_NBR'
  - 'PROJECT_ID'
  - 'ACTION_IND'
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