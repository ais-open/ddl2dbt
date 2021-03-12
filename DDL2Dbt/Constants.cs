namespace DDL2Dbt
{
    internal class Constants
    {
        public const string HubFileName = "hub";
        public const string LnkFileName = "lnk";
        public const string SatFileName = "sat";
        public const string SatBrFileName = "sat_br";
        public const string LoadTimestamp = "LOAD_TIMESTAMP";
        public const string SrcHashDiff = "HASHDIFF";
        public const string SrcEff = "EFFECTIVE_TIMESTAMP";
        public const string RecordSource = "RECORD_SOURCE";
        public const string CouldNotFindTagInConfig = "Could not find tags in the csv file for table: ";
        public const string InvalidDDLFileError = "Invalid DDL File, parser was not able to extract ddl statements.";

        public const string InvalidDDLFileExensionError =
            "Invalid DDL File extenstion, supported extensions include .txt or .ddl or .sql";

        public const string InvalidCSVFileError = "Invalid CSV File";
        public const string NotFoundString = "???";

        public const string ModelErrorMessage =
            "Please enter a valid model, Options include: hub,sat and lnk or * (Default). Use comma to select multiple options Ex: sat,hub. To generate stg files either a * or a specific model name with stg should be included Ex: -m hub,stg";

        public const string InvalidDDlFileCouldNotFindCreateTable =
            "Invalid DDL File, parser was not able to find create table ddl statements.";

        public const string InvalidDDLFileLocation = "Could not find the DDL file at the specified location.";
    }
}