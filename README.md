# DataHelperLib
Data Helper for C# .NET
It will simplify your coding process


public static void BulkCopyToSqlServer(DataTable dataTable, string TableName, string connectionString);
public static string DESDecryptData(string EncryptedString);
public static string DESEncryptData(string data);
public static bool IsValidDate(string date, string dateFormat);
public static DataTable RetrieveDataFromSqlServer(string RawQuery, string connectionString);
public static void SaveDataTableToCsv(DataTable dataTable, string filePath);
