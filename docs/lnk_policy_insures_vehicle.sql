{{ config(tags = ['policy'], materialized = 'table') }}

{%- set metadata_yaml -%}
source_model: 'stg_peak_vehicle_vinsymbol'
src_pk: 'POLICY_INSURES_VEHICLE_HK'
src_fk:
  - 'POLICY_HK'
  - 'VEHICLE_HK'
src_ldts: 'LOAD_TIMESTAMP'
src_source: 'RECORD_SOURCE'
{%- endset -%}

{% set metadata_dict = fromyaml(metadata_yaml) -%}
{% set source_model = metadata_dict['source_model'] -%}
{% set src_pk = metadata_dict['src_pk'] -%}
{% set src_fk = metadata_dict['src_fk'] -%}
{% set src_ldts = metadata_dict['src_ldts'] -%}
{% set src_source = metadata_dict['src_source'] -%}

{{ dbtvault.link(src_pk=src_pk, 
                src_fk=src_fk, 
                src_ldts=src_ldts,
                src_source=src_source, 
                source_model=source_model) }}
