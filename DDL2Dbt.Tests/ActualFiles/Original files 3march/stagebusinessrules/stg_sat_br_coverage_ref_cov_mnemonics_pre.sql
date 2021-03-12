{{ config(tags = ['policy', 'reference', 'businessrule']) }}

with cte_1 as (
    select hc.coverage_hk
        ,hc.load_timestamp
        ,cm.load_timestamp as effective_timestamp
        ,'COV_MNEMONICS' as record_source
        ,cm.pvcl_veh_cov_des as coverage_description
    from {{ ref ('hub_coverage') }} as hc
    left join {{ ref ('sat_rds_cov_mnemonics') }} as cm
        -- Rules that apply to all states or our policy rated state match
        on cm.risk_state_cd in (hc.state_code, 'ZZ', 'ZZZ')
            -- Coverage code matches
            and cm.pa_veh_cov_cd = hc.coverage_code
            -- PEAK is a service system, so rules that apply to service or all systems match
            and cm.bus_src_app_id in ('SERVICE', 'ZZZZZZZZ')
            -- We want the message in english instead of spanish
            and cm.lgg_cd2 = 'EN'
    qualify row_number() over (
            partition by hc.coverage_hk
            order by cm.risk_state_cd
                ,cm.pa_veh_cov_cd
                ,cm.bus_src_app_id
                ,cm.lgg_cd2
                ,cm.pvcl_veh_cov_des
        ) = 1
)

select * from cte_1