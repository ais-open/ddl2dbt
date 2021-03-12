{{ config(tags = ['policy', 'businessrule']) }}

with cte_1 as (
    select pp.policy_hk
        ,pp.load_timestamp
        ,pp.effective_timestamp
        ,'RISK_SEGMENT' as record_source
        ,pp.transaction_hk
        ,case pp.risksegment
            when 'B' then 'Preferred'
            when 'C' then 'Standard'
            when 'D' then 'Non-Standard'
            else '' end as risk_segment
    from {{ ref('sat_peak_policy') }} as pp
)

select * from cte_1
