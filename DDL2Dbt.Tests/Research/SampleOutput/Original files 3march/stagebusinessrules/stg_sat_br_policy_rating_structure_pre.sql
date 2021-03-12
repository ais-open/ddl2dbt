{{ config(tags = ['policy', 'businessrule']) }}

with cte_1 as (
  select pp.policy_hk
      ,pp.load_timestamp
      ,pp.effective_timestamp
      ,'RATING_STRUCTURE' as record_source
      ,pp.transaction_hk
      ,case when pp.lifesegment is null then 'ORS' else 'REVO' end as rating_structure
  from {{ ref ('sat_peak_policy') }} as pp
)

select * from cte_1