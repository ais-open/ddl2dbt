namespace ddl2dbt.Templates
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