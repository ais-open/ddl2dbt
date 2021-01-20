{{ config(tags = ['policy']) }}

with CTE_1 AS
(
    select R.*, P.PRIMARYRATINGSTATE AS STATECODE 
    from  {{ source('PEAK_POLICY_CONFORMED', 'RISK_COVERAGE') }} 
    as R 
    inner join 
    {{ source('PEAK_POLICY_CONFORMED', 'POLICY') }} 
    as P
    on R.POLICY_ID = P.POLICY_ID
),
CTE_2 AS
(
    select PR.*, V.VEHICLE_ID from CTE_1 
    as PR
    inner join
    {{ source('PEAK_POLICY_CONFORMED', 'VEHICLE') }} 
    as V
    on PR.RISK_ID = V.RISK_ID
)

select * from CTE_2