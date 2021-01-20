{{ config(tags = ['policy']) }}

with CTE_1
as
(
    select V.*, T.EFFECTIVEDATE AS EFFECTIVEDATE 
    from  {{ source('PEAK_POLICY_CONFORMED', 'VEHICLE_REGISTRATIONOWNERSHIP') }} 
    as V 
    inner join 
    {{ source('PEAK_POLICY_CONFORMED', 'TRANSACTION') }} 
    as T
    on V.POLICYREADONLYHISTORYID = T.POLICYREADONLYHISTORYID
)

select * from CTE_1