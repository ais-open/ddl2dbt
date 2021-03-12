{{ config(tags = ['core']) }}

{%- set metadata_yaml -%}
source_model:
  PEAK_POLICY_CONFORMED: 'POLICY'
include_source_columns: true
derived_columns:
  RECORD_SOURCE: '!PEAK'
  EFFECTIVE_TIMESTAMP: TO_TIMESTAMP(EFFECTIVEDATE)
  POLICYREADONLYHISTORYID: 'POLICYREADONLYHISTORYID::VARCHAR'
hashed_columns:
  POLICY_HK: 'POLICY_NUMBER'
  TRANSACTION_HK: 'POLICYREADONLYHISTORYID'
  HASHDIFF:
    is_hashdiff: true
    columns:
      - 'TRANSACTION_HK'
      - 'POLICY_NUMBER'
      - 'POLICYREADONLYHISTORYID'
      - 'INCLUDEDELETED'
      - 'PRODUCTKIND'
      - 'RETRIEVEPOLICY_ID'
      - 'DATA_ID'
      - 'POLICY_ID'
      - 'ACCOUNTID'
      - 'AGENTID'
      - 'AIPVOLUNTARYRATEINDICATOR'
      - 'APPLICATIONORIGINALWRITTENDATE'
      - 'AVAILABLEVEHICLENUMBERID'
      - 'CANCELLATIONDATE'
      - 'CAPPINGPREVIOUSLOBCODE'
      - 'COMPANYINCEPTIONDATE'
      - 'CONTRACTOVERRIDE'
      - 'CONTRACTREVIEWDATE'
      - 'CONTRACTTYPE'
      - 'CONTRACTTYPEASSIGNDATE'
      - 'CORPORATEINCEPTIONDATE'
      - 'CTRAUTHORIZATIONSTATUS'
      - 'CTRRECEIVEDDATE'
      - 'CTRSENTDATE'
      - 'DECLINEOVERRIDE'
      - 'DECLINEOVERRIDEUSER'
      - 'DECPACKAGETYPECODE'
      - 'DEPENDENTS'
      - 'EFFECTIVEDATE'
      - 'EFFECTIVETYPECODE'
      - 'EXPIRATIONDATE'
      - 'FASTTRACKRENEWALINDICATOR'
      - 'GDRNAME'
      - 'INCEPTIONDATE'
      - 'INSURANCETYPE'
      - 'ISFULLCOVERAGE'
      - 'ISINTERNETSALE'
      - 'ISRENEWALQUESTIONNAIREREQUIRED'
      - 'ISSHORTTERMPOLICY'
      - 'LASTCHANGEDINSURANCETYPE'
      - 'LASTCHANGEDLIFESEGMENT'
      - 'LASTCHANGEDLOBCODE'
      - 'LASTCHANGEDPRIMARYRATINGSTATE'
      - 'LASTCHANGEDRISKSEGMENT'
      - 'LASTCHANGEDWRITINGCOMPANY'
      - 'LEGACYSYSTEMACTIVITYDATE'
      - 'LIFESEGMENT'
      - 'LIFESEGMENTINCEPTIONDATE'
      - 'LOBCODE'
      - 'MINIMUMPREMIUM'
      - 'MINIMUMPREMIUMWRITTEN'
      - 'NONCANCELLABLE'
      - 'NONOWNEDAUTOCOVERAGETYPE'
      - 'OPTIONFORMRECEIVEDINDICATOR'
      - 'OPTIONFORMREQUIREDINDICATOR'
      - 'ORIGINALPOLICYEFFECTIVEDATE'
      - 'PAPERLESSPOLICYSTATUS'
      - 'PAPERLESSPOLICYSTATUSDATE'
      - 'PARTIALPAPERLESSSTATUS'
      - 'PAYPLAN'
      - 'POLICYBINDERDATETIMESTAMP'
      - 'POLICYCLAIMSIDENTIFIER'
      - 'POLICYNUMBER'
      - 'POLICYTERMID'
      - 'PREMIUM'
      - 'PREMIUMCHANGE'
      - 'PREMIUMWRITTEN'
      - 'PRIMARYRATINGSTATE'
      - 'PRODUCT'
      - 'PRORATEDADJUSTMENTAMOUNT'
      - 'PROXYSTATUSCOUNTER'
      - 'RATINGTIER'
      - 'REASONAMENDMENTCODE'
      - 'REISSUEWITHLAPSEDATE'
      - 'REISSUEWITHLAPSEEFFECTIVEDATE'
      - 'RELATIONTOPRIORPOLICYAPPLICANT'
      - 'RENEWALSTATUS'
      - 'REQUESTEDMAILPACKAGECODE'
      - 'RETENTIONKEY'
      - 'RETURNEDMAILINDICATOR'
      - 'RISKSEGMENT'
      - 'RISKSEGMENTINCEPTIONDATE'
      - 'SALESTYPE'
      - 'SOURCEOFADVERTISING'
      - 'SOURCEOFBUSINESS'
      - 'STATESTANDARDPOLICYINDICATOR'
      - 'STATETENURESTARTDATE'
      - 'STATUS'
      - 'SUPPRESSDOCUMENTOUTPUT'
      - 'TENUREPOLICYYEARS'
      - 'TERM'
      - 'TERMFACTOR'
      - 'TEXASPROXYSTATUS'
      - 'UMBRELLAREFERRAL'
      - 'WASBALANCEOVERRIDDEN'
      - 'WRITINGCOMPANY'
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