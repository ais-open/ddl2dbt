# Readme

The PublishSingleFile Flag : go to the solution folder and run the following cmd, to generate a single executable.

dotnet publish -r win-x64 -c Release /p:PublishSingleFile=true

To execute, run the following command with the appropriate file path

.\Ddl2Dbt.exe -ddl "{DDL FILE PATH}" -m * -o "{OUTPUT FOLDER}"

Example:
.\Ddl2Dbt.exe -ddl "D:\madhu\GeicoDDLTransformers\docs\Policy Phase 1 v0.13.52 DDL.ddl" -m * -o "D:\madhu\ddl\"
