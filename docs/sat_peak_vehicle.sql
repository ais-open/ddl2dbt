{{ config(tags = ['policy'], materialized = 'table') }}

{%- set metadata_yaml -%}
source_model: 'stg_peak_vehicle'
src_pk: 'POLICY_INSURES_VEHICLE_HK'
src_hashdiff: 'HASHDIFF'
src_eff: 'EFFECTIVE_TIMESTAMP'
src_ldts: 'LOAD_TIMESTAMP'
src_source: 'RECORD_SOURCE'
src_payload:
  - 'TRANSACTION_HK'
  - 'POLICY_NUMBER'
  - 'POLICYREADONLYHISTORYID'
  - 'INCLUDEDELETED'
  - 'PRODUCTKIND'
  - 'RETRIEVEPOLICY_ID'
  - 'DATA_ID'
  - 'POLICY_ID'
  - 'LINE_ID'
  - 'LINE_CHANGE'
  - 'LINE_WRITTEN'
  - 'RISK_ID'
  - 'RISK_DELETED'
  - 'VEHICLE_ID'
  - 'VEHICLE_DELETED'
  - 'ACTION'
  - 'ASSIGNEDDRIVER'
  - 'EFFECTIVEDATE'
  - 'EFFECTIVETYPECODE'
  - 'LASTCHANGEDACTION'
  - 'LASTCHANGEDDATE'
  - 'REASONAMENDMENTCODE'
  - 'REMOVALREASON'
  - 'REMOVEDDATE'
  - 'REPLACEDVEHICLEID'
  - 'STORAGEPLAN'
  - 'VEHICLEHISTORYDAMAGEDCODE'
  - 'VEHICLEHISTORYDAMAGEDIMPACTCODE'
  - 'VEHICLEHISTORYORDERDATE'
  - 'VEHICLEHISTORYRATEBOOKDATE'
  - 'VEHICLEHISTORYTITLETRANSFERREDCODE'
  - 'VEHICLEHISTORYTITLETRANSFERREDIMPACTCODE'
  - 'VEHICLELOCATION'
  - 'VEHICLENUMBER'
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