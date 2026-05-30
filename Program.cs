using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;


namespace CreateCampusLogicImportFile
{
    class Program
    {
        private static bool IsCommaPresent(string columnValue)
        {
            return columnValue.Contains(",");
        }

        static void Main(string[] args)
        {
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["IntendedServer"].ConnectionString;
            string csvQuote = "\"";
            string escapedQuote = "\"\"";
            string endOfLine = "\r\n";
            var sqlStatement = System.Configuration.ConfigurationManager.AppSettings["sql"];
            string finalFilePath = System.Configuration.ConfigurationManager.AppSettings["finalFilePath"];
            var columnDelimiter = ",";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                Console.WriteLine(DateTime.Now);

                using (SqlCommand cmd = new SqlCommand(sqlStatement, con))
                {
                    cmd.CommandTimeout = int.Parse(System.Configuration.ConfigurationManager.AppSettings["commandTimeout"]);
                    con.Open();

                    using (var writer = new StreamWriter(finalFilePath, false, Encoding.UTF8))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        int lastColumnIndex = reader.FieldCount - 1;

                        // Write header row
                        for (int i = 0; i <= lastColumnIndex; i++)
                        {
                            writer.Write(reader.GetName(i));
                            if (i < lastColumnIndex)
                                writer.Write(columnDelimiter);
                        }
                        writer.Write(endOfLine);

                        // Write data rows
                        while (reader.Read())
                        {
                            for (int i = 0; i <= lastColumnIndex; i++)
                            {
                                string value = reader.IsDBNull(i) ? "" : reader.GetValue(i).ToString();

                                if (IsCommaPresent(value))
                                {
                                    // Wrap in double-quotes and escape any inner double-quotes
                                    writer.Write(csvQuote);
                                    writer.Write(value.Replace(csvQuote, escapedQuote));
                                    writer.Write(csvQuote);
                                }
                                else
                                {
                                    writer.Write(value);
                                }

                                if (i < lastColumnIndex)
                                    writer.Write(columnDelimiter);
                            }
                            writer.Write(endOfLine);
                        }
                    }
                }
            }

            Console.WriteLine(DateTime.Now);
            Console.WriteLine("Done");
        }
    }
}
