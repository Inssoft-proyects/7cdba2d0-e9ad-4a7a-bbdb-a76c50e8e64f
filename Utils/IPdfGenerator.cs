using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Utils
{
    public interface IPdfGenerator<T> where T : class
    {
        (MemoryStream, string) CreatePdf(string NamePdf, string Title, int SizeColumns, Dictionary<string, string> ColumnsNames, List<T> ModelData);
        (MemoryStream, string) CreatePdf(string NamePdf, string Title, int SizeColumns, Dictionary<string, string> ColumnsNames, T ModelData);
    }
}
