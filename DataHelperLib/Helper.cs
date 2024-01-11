using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DataHelperLib
{
    public class Helper
    {
        public static string publicKey = "WBK12345";
        public static string privateKey = "WBK54321";

        // Encrypt String
        public static string DESEncryptData(string data)
        {
            var result = string.Empty;
            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);
            byte[] inputByteArray = Encoding.UTF8.GetBytes(data);
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                var memoryStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memoryStream,
                provider.CreateEncryptor(publicKeyBytes, privateKeyBytes),
                 CryptoStreamMode.Write);
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();
                result = Convert.ToBase64String(memoryStream.ToArray());
            }
            return result;
        }

        // Decrypt String
        public static string DESDecryptData(string data)
        {
            string result = "";
            byte[] privateKeyBytes = Encoding.UTF8.GetBytes(privateKey);
            byte[] publicKeyBytes = Encoding.UTF8.GetBytes(publicKey);

            byte[] inputByteArray = new byte[data.Replace(" ", "+").Length];
            inputByteArray = Convert.FromBase64String(data.Replace(" ", "+"));
            using (DESCryptoServiceProvider provider = new DESCryptoServiceProvider())
            {
                var memoryStream = new MemoryStream();
                var cryptoStream = new CryptoStream(memoryStream,
                provider.CreateDecryptor(publicKeyBytes, privateKeyBytes),
                CryptoStreamMode.Write);
                cryptoStream.Write(inputByteArray, 0, inputByteArray.Length);
                cryptoStream.FlushFinalBlock();
                result = Encoding.UTF8.GetString(memoryStream.ToArray());
            }

            return result;
        }

        // Validate String is DateTIme
        public static bool IsValidDate(string date, string dateFormat)
        {
            // Coba parsing input sebagai tanggal dengan format YYYYMMDD
            if (DateTime.TryParseExact(date, dateFormat, null, System.Globalization.DateTimeStyles.None, out _))
            {
                return true;
            }
            else
            {
                // Jika parsing gagal, input tidak valid
                return false;
            }
        }

        // Retrieve Data From SQL Server
        public static DataTable RetrieveDataFromSqlServer(string RawQuery, string connectionString)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlDataAdapter adapter = new SqlDataAdapter(RawQuery, connection))
                {
                    adapter.Fill(dataTable);
                }
            }
            return dataTable;
        }

        // DataTable to SQL
        public static void BulkCopyToSqlServer(DataTable dataTable, string TableName, string connectionString)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    // Membuat objek SqlBulkCopy
                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        // Menetapkan nama tabel tujuan di SQL Server
                        bulkCopy.DestinationTableName = TableName;

                        // Menetapkan pemetaan kolom antara DataTable dan tabel SQL Server
                        foreach (DataColumn column in dataTable.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                        }

                        // Melakukan bulk copy
                        bulkCopy.WriteToServer(dataTable);
                    }
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during bulk copy: {ex.Message}");
            }
        }

        // Generate File from DataTable
        public static void SaveDataTableToCsv(DataTable dataTable, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Menulis header
                writer.WriteLine(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName)));

                // Menulis baris data
                foreach (DataRow row in dataTable.Rows)
                {
                    // Membuat array dari nilai kolom yang diinginkan
                    string[] rowValues = dataTable.Columns.Cast<DataColumn>()
                        .Select(column => row[column].ToString())
                        .ToArray();

                    // Menulis baris ke dalam file CSV
                    writer.WriteLine(string.Join(",", rowValues));
                }
            }
        }
    }
}
