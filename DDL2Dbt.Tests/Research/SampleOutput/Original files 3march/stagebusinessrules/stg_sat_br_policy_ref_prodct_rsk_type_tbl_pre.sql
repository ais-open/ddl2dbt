{{ config(tags = ['policy', 'reference', 'businessrule']) }}

with cte_1 as (
    select pp.policy_hk
        ,pp.load_timestamp
        ,pp.effective_timestamp
        ,'PRODCT_RSK_TYP_TBL' as record_source
        ,pp.transaction_hk
        ,rt.ins_risk_type_nm as risk_type
    from {{ ref('sat_peak_policy') }} as pp
    left join {{ ref('sat_rds_prodct_rsk_typ_tbl') }} as rt
        -- Rules that apply to all states or our policy rated state match
        on rt.risk_state_cd in (pp.primaryratingstate, 'ZZ', 'ZZZ')
            -- Rules that apply to our line of business code
            and rt.lob_cd = pp.lobcode
            -- Rules that apply to all companies or our written company
            and rt.master_company_nbr in (pp.writingcompany, 'ZZ')
            and pp.effectivedate >= rt.effective_dt
            and pp.effectivedate < rt.expiration_dt
    qualify row_number() over (
            partition by pp.policy_hk
                ,pp.transaction_hk
                ,pp.load_timestamp
            order by rt.lob_cd
                ,rt.risk_state_cd
                ,rt.master_company_nbr
                ,rt.effective_dt desc
        ) = 1
)

select * from cte_1