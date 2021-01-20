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
)

select * from CTE_1