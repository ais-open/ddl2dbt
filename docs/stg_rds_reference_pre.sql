{{ config(tags = ['reference']) }}

with STG_RDS_REFERENCE_PRE
as
(
    select 'COV_DED_LIT' as REFERENCE_RULE_TABLE_NAME
    union all select 'COV_LIMIT_LIT' as REFERENCE_RULE_TABLE_NAME
    union all select 'COV_MNEMONICS' as REFERENCE_RULE_TABLE_NAME
    union all select 'PRODUCT_RSK_TYP_TBL' as REFERENCE_RULE_TABLE_NAME
)

select * from STG_RDS_REFERENCE_PRE