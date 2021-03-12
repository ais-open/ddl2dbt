{{ config(tags = ['policy', 'reference', 'businessrule']) }}

with cte_1 as
(
    select hc.coverage_hk
        ,hc.load_timestamp
        ,ll.load_timestamp as effective_timestamp
        ,'COV_LIMIT_LIT' as record_source
        ,ll.pall_limit_des as limit_description
    from {{ ref('hub_coverage') }} as hc
    left join {{ ref('sat_rds_cov_limit_lit') }} as ll
        -- Rules that apply to all states or our policy rated state match
        on ll.risk_state_cd in (hc.state_code, 'ZZ', 'ZZZ')
            -- Coverage code matches or rule applies to all coverages
            and ll.pa_veh_cov_cd in (hc.coverage_code, 'ZZZZ')
            -- limit code matches
            and ll.veh_cov_lim_cd = hc.limit_code
            -- PEAK is a service system, so rules that apply to service or all systems match
            and ll.bus_src_app_id in ('SERVICE', 'ZZZZZZZZ')
            -- We want the message in english instead of spanish
            and ll.lgg_cd2 = 'EN'
    qualify row_number() over (
            partition by hc.coverage_hk
            order by ll.risk_state_cd
                ,ll.veh_cov_lim_cd
                ,ll.pa_veh_cov_cd
                ,ll.bus_src_app_id
                ,ll.lgg_cd2
                ,ll.pall_limit_des
        ) = 1
)

select * from cte_1