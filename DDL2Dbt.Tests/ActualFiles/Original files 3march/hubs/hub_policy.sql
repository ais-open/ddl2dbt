{{ config(tags = ['core']) }}

{%- set metadata_yaml -%}
source_model: 'stg_hub_policy'
src_pk: 'POLICY_HK'
src_nk: 'POLICY_NUMBER'
src_ldts: 'LOAD_TIMESTAMP'
src_source: 'RECORD_SOURCE'
{%- endset -%}

{% set metadata_dict = fromyaml(metadata_yaml) -%}
{% set source_model = metadata_dict['source_model'] -%}
{% set src_pk = metadata_dict['src_pk'] -%}
{% set src_nk = metadata_dict['src_nk'] -%}
{% set src_ldts = metadata_dict['src_ldts'] -%}
{% set src_source = metadata_dict['src_source'] -%}

{{ dbtvault.hub(src_pk=src_pk, 
                src_nk=src_nk, 
                src_ldts=src_ldts,
                src_source=src_source, 
                source_model=source_model) }}