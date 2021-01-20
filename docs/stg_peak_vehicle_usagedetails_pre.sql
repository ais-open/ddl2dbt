{{ config(tags = ['policy']) }}

with STG_PEAK_VEHICLE_USAGEDETAILS_PRE
as
(
    select V.*, T.EFFECTIVEDATE AS EFFECTIVEDATE 
    from  {{ source('PEAK_POLICY_CONFORMED', 'VEHICLE_USAGEDETAILS') }} 
    as V 
    inner join 
    {{ source('PEAK_POLICY_CONFORMED', 'TRANSACTION') }} 
    AS T
    ON V.POLICYREADONLYHISTORYID = T.POLICYREADONLYHISTORYID
)

SELECT * FROM stg_peak_vehicle_usagedetails_pre