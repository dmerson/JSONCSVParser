# JSONCSVParser
A program that parsez out any column in SQL Server 2017 including the new JSON Columns.



# Configurable

You can configure the following parameters in the app.config:
1. Connection String (Intended server)
2. SQL Statement
3. JSON Column Keyword-word which if included in column name with denote it is a JSON column and deserves special handling
4. Final File Path- Where to place the final file
5. Command Timeout -In case the sql statement takes a while to run


# Future Development

In the near future, the app will autodetect the last column of the sql statement.
It will also try to autodetect JSON in the data (nvarchar(max) and starts with "[{" and ends with "]]".
Allow the deliminator to be changed via the app.config.

# Current abilities tested

This version has been tested with exporting a table from SQL with 43K records and 60 columns for 110MB file size within 30 seconds.



