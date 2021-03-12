{{ config(tags = ['borg']) }}

{%- set metadata_yaml -%}
source_model:
  ???: 'VEHICLE'
include_source_columns: true
derived_columns:
  EFFECTIVE_TIMESTAMP: 'TO_TIMESTAMP(EFFECTIVEDATE)'
  RECORD_SOURCE: '!PEAK'
hashed_columns:
  POLICY_TRANSACTION_INSURES_VEHICLE_HK: 
    - 'POLICY_TRANSACTION_HK'
    - 'VEHICLE_HK'
    - 'RISK_ID'
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'RECORD_SOURCE'
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
      - 'POLICY_TRANSACTION_INSURES_VEHICLE_HK'
      - 'REPLACEDVIN'
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