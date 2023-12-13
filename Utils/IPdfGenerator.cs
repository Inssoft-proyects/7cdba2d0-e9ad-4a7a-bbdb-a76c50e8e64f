using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.IO;

namespace GuanajuatoAdminUsuarios.Utils
{
    public interface IPdfGenerator
    {
        (MemoryStream, string) CreatePdf<T>(string NamePdf, string Title, int SizeColumns, Dictionary<string, string> ColumnsNames, List<T> ModelData);
        (MemoryStream, string) CreatePdf<T>(string NamePdf, string Title, int SizeColumns, Dictionary<string, string> ColumnsNames, T ModelData);
        byte[] CreatePDFByHTML(string html, string cssText, Rectangle pageSize);
        //Byte[] PdfSharpConvert(String html);

    }
}
