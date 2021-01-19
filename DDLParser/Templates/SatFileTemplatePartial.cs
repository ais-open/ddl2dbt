namespace DDLParser.Templates
{
    public partial class SatFileTemplate
    {
        public SatTableMetadata SatTableMetadata;

        public SatFileTemplate(SatTableMetadata satTableMetadata)
        {
            SatTableMetadata = satTableMetadata;
        }
    }
}
