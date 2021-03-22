{{ config(tags = ['tag']) }}

{%- set metadata_yaml -%}
source_model: 
    - 'stg_source_model_1'
    - 'stg_source_model_2'
src_pk: 'CUSTOMER_HK'
src_nk: 'CUSTOMER_NO'
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