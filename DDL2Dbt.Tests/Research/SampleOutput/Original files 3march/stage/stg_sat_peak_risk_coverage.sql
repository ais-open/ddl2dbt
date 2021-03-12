{{ config(tags = ['core']) }}

{%- set metadata_yaml -%}
source_model:
  PEAK_POLICY_CONFORMED: 'COVERAGE'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!PEAK'
  LOAD_TIMESTAMP: 'CURRENT_TIMESTAMP()'
  EFFECTIVE_TIMESTAMP: 'TO_TIMESTAMP(EFFECTIVEDATE)'
  POLICY_NUMBER: 'POLICY_NUMBER'
  POLICYREADONLYHISTORYID: 'POLICYREADONLYHISTORYID'
  INCLUDEDELETED: 'INCLUDEDELETED'
  PRODUCTKIND: 'PRODUCTKIND'
  RETRIEVEPOLICY_ID: 'RETRIEVEPOLICY_ID'
  DATA_ID: 'DATA_ID'
  POLICY_ID: 'POLICY_ID'
  LINE_ID: 'LINE_ID'
  LINE_CHANGE: 'LINE_CHANGE'
  LINE_WRITTEN: 'LINE_WRITTEN'
  RISK_ID: 'RISK_ID'
  RISK_DELETED: 'RISK_DELETED'
  COVERAGE_ID: 'COVERAGE_ID'
  COVERAGE_DELETED: 'COVERAGE_DELETED'
  ACTION: 'ACTION'
  CANCELPREM: 'CANCELPREM'
  CHANGE: 'CHANGE'
  COVERAGECODE: 'COVERAGECODE'
  COVERAGELEVEL: 'COVERAGELEVEL'
  EFFECTIVEDATE: 'EFFECTIVEDATE'
  EFFECTIVETYPECODE: 'EFFECTIVETYPECODE'
  INDICATOR: 'INDICATOR'
hashed_columns:
  POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK: ''
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'POLICY_TRANSACTION_HAS_VEHICLE_COVERAGE_HK'
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
      - 'COVERAGE_ID'
      - 'COVERAGE_DELETED'
      - 'ACTION'
      - 'CANCELPREM'
      - 'CHANGE'
      - 'COVERAGECODE'
      - 'COVERAGELEVEL'
      - 'EFFECTIVEDATE'
      - 'EFFECTIVETYPECODE'
      - 'INDICATOR'
      - 'LASTCHANGEDACTION'
      - 'LASTCHANGEDDATE'
      - 'LASTPREMIUMCHANGEDACTION'
      - 'LASTPREMIUMCHANGEDDATE'
      - 'MANUALLYENTEREDPREMIUM'
      - 'OFFSET'
      - 'ONSET'
      - 'PREMIUM'
      - 'PREMIUMEFFECTIVETYPECODE'
      - 'PREMIUMREASONAMENDMENTCODE'
      - 'PRIOR'
      - 'PRIORTERM'
      - 'REASONAMENDMENTCODE'
      - 'TYPE'
      - 'WRITTEN'
      - 'DEDUCTIBLE_ID'
      - 'DEDUCTIBLECODE'
      - 'LIMIT_ID'
      - 'LIMITCODE'
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