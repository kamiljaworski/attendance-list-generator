﻿using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.IO;
using MigraDoc.Rendering;
using System;

namespace AttendanceListGenerator.Core.IO
{
    public class FileSaver : IFileSaver
    {
        public bool SavePdfDocument(Document document, string path, string filename)
        {
            if (document == null)
                throw new ArgumentNullException("Document cannot be null");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("Path cannot be null or empty");

            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("Filename cannot be null or empty");

            try
            {
                string fullPath = GetFullPath(path, filename);

                DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

                PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp1_32.Pdf.PdfFontEmbedding.Always);
                renderer.Document = document;

                renderer.RenderDocument();

                renderer.PdfDocument.Save(fullPath);

                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        private string GetFullPath(string path, string filename) => path + "\\" + filename;
    }
}
