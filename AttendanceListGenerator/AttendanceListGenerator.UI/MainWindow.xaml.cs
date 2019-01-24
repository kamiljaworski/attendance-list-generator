﻿using AttendanceListGenerator.Core.Data;
using AttendanceListGenerator.Core.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace AttendanceListGenerator.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public object PdfFontEmbedding { get; private set; }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IAttendanceListData attendanceListData = new AttendanceListData(new List<string> { "test", "test" }, Month.February, 2019);
            IAttendanceListDocumentGenerator documentGenerator = new AttendanceListDocumentGenerator(attendanceListData, new TempLocalizedNames());
            Document document = documentGenerator.GenerateDocument();
            SaveDocument(document);
        }

        private void SaveDocument(Document document)
        {
            MigraDoc.DocumentObjectModel.IO.DdlWriter.WriteToFile(document, "MigraDoc.mdddl");

            PdfDocumentRenderer renderer = new PdfDocumentRenderer(true, PdfSharp1_32.Pdf.PdfFontEmbedding.Always);
            renderer.Document = document;

            renderer.RenderDocument();

            // Save the document...
            string filename = "SimpleTable.pdf";
            renderer.PdfDocument.Save(filename);
            // ...and start a viewer.
            Process.Start(filename);
        }
    }
}
