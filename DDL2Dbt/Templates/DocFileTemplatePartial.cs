namespace DDL2Dbt.Templates
{
    public partial class DocFileTemplate
    {
        public string TableName { get; }
        public string Model { get; }


        public DocFileTemplate(string tableName, string model)
        {
            TableName = tableName;
            Model = model;
        }
    }
}