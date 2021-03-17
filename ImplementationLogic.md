**Ddl2dbt -DRAFT**

**DDL parsing**

The sequence of parsing the ddl is described below.

1. Once the ddl is loaded, we split the file contents using a &quot;;+newline&quot; characters, to get a list of individual statements.
2. From the individual statements we search for the string &quot;_CREATE TABLE_&quot; HUB or SAT or LNK, when we find a statement, we do the following operations
  - -  **Extract table name** : the string is split based on the first occurrence of the char &quot;(&quot; and the length of the &quot;CREATE TABLE&quot;, which will give us the name of the table.
  - -  **Extract the primary keys:** Once we have the table name, to retrieve the primary keys we search for the following string in the ddl statements, _ALTER TABLE {tableName}&quot; +_ newline _+ &quot;ADD PRIMARY KEY&quot; with_ the table name retrieved from the above step. We extract the values between the &#39;(&#39; and &#39;)&#39; and do a string split using &#39;,&#39; to check if it&#39;s a composite key and add it to a collection.

  - -  **Extract the foreign keys** : **:** Once we have the table name, to retrieve the foreign keys we search for the following string in the ddl statements, &quot;_ALTER TABLE {tableName} +_ newline&quot;and then the string _&quot;FOREIGN KEY&quot;_ retrieve a collection of foreign key ddl statements for a particular table name, then for each ddl statement we extract the values between the &#39;(&#39; and &#39;)&#39; and add it to a collection.

  - -  **Extract the column name and datatypes:** we extract the contents between the &#39;(&#39; and &#39;)&#39; in the create table statement and split the result using _&#39;,&#39;+newline character_ and trim the result, this will result in a list of strings containing both the column and data type. We then find the first occurrence of a space char in the string and then retrieve the substring from the beginning of the string to index of the space char, which will give us the column name, and then rest of the string contain the data type and a trim is performed on both he name and data types to remove any space chars.

**Building stg files**

Each hub, sat and lnk table in the ddl will have a corresponding hub sat and lnk files, and each hub, sat and lnk in turn will have their own corresponding stg files if there are records for that table in the csv.

The following are the sections of a stg file and the description involves on how the populate them.

1. tags: tags are populated from the &quot;Tags&quot; column present in the csv file.
2. Source\_model : It&#39;s a label value pair where the label is from the csv column &quot;Source Model&quot; and the value is the last segment of its Table Name.
In case we have more than one value in &quot;Source Model&quot;, then multiple stage files will be generated with each one having a unique Source\_model.

 3. derived\_columns : It contains multiple label-value/label-list pairs. The csv file is checked if column &#39;Derived/Hashed Columns&#39; contains any data, if it does then the &#39;Column Name&#39; is used as the label and &#39;Derived/Hashed Columns&#39; is used as value/list.
Rows with &#39;Columns Name&#39; equals &#39;LOAD\_TIMESTAMP&#39; or contains &quot;\_HK&quot; are excluded from the derived\_columns.

5. hashed\_columns : It has two parts.

  - 1. Part-1
It contains multiple label-value/label-list pairs. First, we take all the columns containing &quot;\_HK&quot; in the &#39;Column Name&#39;, then for the selected columns &#39;Derived/Hashed Columns&#39; value is checked. If the value is in the form MD5(value-1,value-2,...) or MD5(CONCAT(value-1, value-2,...)) then the value is parsed and finally mapped with the &#39;Column Name&#39;.

  - 2. Part-2
The csv file is checked if the current table contains the column &#39;HASHDIFF&#39;, if yes then only the Part-2 will be displayed.
It contains:

        1. is\_hashdiff: true.
        2. columns:
It contains all the column names whose corresponding. &quot;HASHDIFF&quot; value is true.

Except for the derived columns for br files, the rest of the columns are common.

**Building Hub Files:**

Inside the ddl file, For each table starting with &quot;CREATE TABLE HUB&quot; will have a corresponding hub file.
The following are the sections of a hub files and the description involves on how the populate them.

1. tags: tags: tags are populated from the &quot;Tags&quot; column present in the csv file.
2. source\_model: It is the concatenation of the keyword &quot;stg\_&quot; with the table name.
Eg: For table &#39;HUB\_CUSTOMER&#39;, its Source\_Model will be &#39;stg\_hub\_customer&#39;.
3. src\_pk: It contains the primary key for the current table. The Primary Key columns are extracted using the logic defined in the DDL Parsing file. Out of the extracted column names only the one ending with &#39;\_HK&#39; is taken and populated with src\_pk.
4. src\_nk: All the columns of the table are extracted and out of those, the ones that are not present in the primary key list (retrieved using the logic in the ddl paraser) and are not &#39;RECORD\_SOURCE&#39; or &#39;LOAD\_TIMESTAMP&#39; are populated with the src\_nk.
5. src\_ldts: This field has a fixed value of &#39;LOAD\_TIMESTAMP&#39;.
6. src\_source: This field has a fixed value of &#39;RECORD\_SOURCE&#39;.

**Building Link Files:**

Inside the ddl file, For each table starting with &quot;LNK&quot; will have a corresponding lnk file.
The following are the sections of a lnk files and the description involves on how the populate them.

1. tags: tags are populated from the &quot;Tags&quot; column present in the csv file.
2. source\_model: It is the concatenation of the keyword &quot;stg\_&quot; with the table name.
Eg: For table &#39;LNK\_CUSTOMER\_NATION&#39;, its Source\_Model will be &#39;stg\_lnk\_customer\_nation&#39;.
3. src\_pk: It contains the primary key for the current table. The Primary Key columns are extracted using the logic defined in the DDL Parsing file. Out of the extracted column names only the one ending with &#39;\_HK&#39; is taken and populated with src\_pk.
4. src\_fk: It contains the foreign key for the current table. The Foreign Key columns are extracted using the logic defined in the DDL Parsing file. These extracted columns are populated with src\_fk.
5. src\_ldts: This field has a fixed value of &#39;LOAD\_TIMESTAMP&#39;.
6. src\_source: This field has a fixed value of &#39;RECORD\_SOURCE&#39;.

**Building sat files:**

Mentioned below are the sections and the description for generating the sat files.

1. tags: tags are populated from the &quot;Tags&quot; column present in the csv file.
2. SourceModel: The source model is a combination of &#39;stg&#39; keyword and the table name coming from the ddl file.
Ex: For table name &#39;SAT\_Table&#39; the SourceModel would be &#39;stg\_sat\_table&#39;
3. Src\_pk: It contains the primary key for the current table. The Primary Key columns are extracted using the logic defined in the DDL Parsing file. Out of the extracted column names only the one ending with &#39;\_HK&#39; is taken and populated with src\_pk.
4. Src\_hashdiff: The src\_hashdiff has a fixed value of &#39;HASHDIFF&#39; which is coming from the constants file which we created.
5. Src\_eff: This field has a fixed value of &#39;EFFECTIVE\_TIMESTAMP&#39;.
6. src\_ldts: This field has a fixed value of &#39;LOAD\_TIMESTAMP&#39;.
7. src\_source: This field has a fixed value of &#39;RECORD\_SOURCE&#39;.
8. Src\_payload: The src\_payload is a list of columns present inside the ddl file which are not part of Src\_pk, Src\_hashdiff, Src\_eff, Src\_ldts and Src\_source.

**Building yaml files:**

Each hub, sat and lnk table in the ddl will have a corresponding yaml file.

- The DDL file will be parsed to get all the hub, lnk and sat tables with their respective columns.
- The table description is fetched from the csv and populated inside the yaml file.
- Similarly all the column description are also fetched and populated with it's respective column name inside the yaml file.
- If the table description or the column description are empty inside the csv then an empty-string( "" ) will be populated.