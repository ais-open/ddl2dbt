{{ config(tags = ['policy', 'reference', 'businessrule']) }}

with cte_1 as
(
    select hc.coverage_hk
        ,hc.load_timestamp
        ,dl.load_timestamp as effective_timestamp
        ,'COV_DED_LIT' as record_source
        ,dl.padl_ded_des as deductible_description
    from {{ ref('hub_coverage') }} as hc
    left join {{ ref('sat_rds_cov_ded_lit') }} as dl
        -- Rules that apply to all states or our policy rated state match
        on dl.risk_state_cd in (hc.state_code, 'ZZ', 'ZZZ')
            -- limit code matches
            and dl.veh_cov_ded_cd = hc.deductible_code
            -- PEAK is a service system, so rules that apply to service or all systems match
            and dl.bus_src_app_id in ('SERVICE', 'ZZZZZZZZ')
            -- We want the message in english instead of spanish
            and dl.lgg_cd2 = 'EN'
    qualify row_number() over (
            partition by hc.coverage_hk
            order by dl.risk_state_cd
                ,dl.veh_cov_ded_cd
                ,dl.bus_src_app_id
                ,dl.lgg_cd2
                ,dl.padl_ded_des
        ) = 1
)

select * from cte_1