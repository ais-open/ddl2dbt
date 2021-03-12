# ddl2dbt

Usage:


Example:
.\Ddl2Dbt.exe --ddl "D:\Policy Phase 1 v0.13.52 DDL.ddl" -m * -o "D:\ddl\\"


Usage:
  Ddl2Dbt [options]

Options:

  -d, --ddl  (REQUIRED)                            DDL File Path

  -c, --csv                                        CSV File Path. if not provided only hub,sat,lnk file generation will be
                                              applicable.

  -m, --models                                Options include: hub,sat,lnk and stg or * (Default) . Use comma to
                                              select multiple options Ex: sat,hub

  -o, --outputPath  (REQUIRED)                 Output File Path

  --version                                   Show version information

  -?, -h, --help                              Show help and usage information
