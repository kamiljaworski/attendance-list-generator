﻿using AttendanceListGenerator.Core.Data;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using System;
using System.Linq;

namespace AttendanceListGenerator.Core.Pdf
{
    public class AttendanceListDocumentGenerator : IAttendanceListDocumentGenerator
    {
        private readonly IAttendanceListData _data;
        private readonly ILocalizedNames _names;

        private const int _numberOfAdditionalColumns = 2;
        private const double _documentMargins = 30.0;
        private const int _fullnamesColumnWidth = 100;
        private const int _firstColumnWidth = 25;
        private const int _secondColumnWidth = 30;

        public bool CanAddColorsToTheDocument { get; set; } = true;
        public Color FullnamesBackgroundColor { get; set; } = new Color(220, 220, 220);
        public Color SundayBackgroundColor { get; set; } = new Color(192, 192, 192);
        public Color SaturdayBackgroundColor { get; set; } = new Color(215, 215, 215);

        public AttendanceListDocumentGenerator(IAttendanceListData data, ILocalizedNames names)
        {
            if (data == null || names == null)
                throw new ArgumentNullException();

            _data = data;
            _names = names;
        }

        public Document GenerateDocument()
        {
            Document document = CreateDocument();
            PageSetup setup = CreatePageSetup(document);
            Section section = GetDocumentSectionWithPageSetup(document, setup);

            Paragraph documentHeading = CreateDocumentHeadingInTheSection(section);
            Table table = CreateTableInTheSection(section);

            ChangeDocumentHeadingFormat(documentHeading);
            ChangeTableFormat(table);

            if (CanAddColorsToTheDocument)
                AddColorsToTheTable(table);

            return document;
        }

        #region Private document generating methods
        private Document CreateDocument()
        {
            // Create a document
            Document document = new Document();

            // Document information
            document.Info.Author = _names.DocumentAuthor;
            document.Info.Title = _names.GetDocumentTitle(_data.Month, _data.Year);

            return document;
        }

        private PageSetup CreatePageSetup(Document document)
        {
            // Get the default page setup and change its orientation to Landscape
            PageSetup setup = document.DefaultPageSetup.Clone();
            setup.Orientation = Orientation.Landscape;

            // Change margins
            setup.TopMargin = _documentMargins;
            setup.BottomMargin = _documentMargins;
            setup.LeftMargin = _documentMargins;
            setup.RightMargin = _documentMargins;

            return setup;
        }

        private Section GetDocumentSectionWithPageSetup(Document document, PageSetup setup)
        {
            // Add a section to the document
            Section section = document.AddSection();

            // Change section setup
            section.PageSetup = setup;

            return section;
        }

        private Paragraph CreateDocumentHeadingInTheSection(Section section)
        {
            // Add a paragraph and title text
            Paragraph documentHeading = section.AddParagraph();
            documentHeading.AddText(_names.GetDocumentTitle(_data.Month, _data.Year));

            return documentHeading;
        }

        private Table CreateTableInTheSection(Section section)
        {
            // Create a table
            Table table = section.AddTable();

            // Count number of all columns in the table
            int numberOfTableColumns = _data.MaxNumberOfFullnames + _numberOfAdditionalColumns;

            // Add columns to the table
            for (int i = 0; i < numberOfTableColumns; ++i)
                table.AddColumn();

            AddRowWithFullnamesToTheTable(table, numberOfTableColumns);

            // Add a row for each day with day number and day of week abbreviation
            foreach (IDay day in _data.Days)
                AddDayToTheTable(table, day);

            return table;
        }

        private void AddRowWithFullnamesToTheTable(Table table, int numberOfColumns)
        {
            // Add fullnames list to the table
            Row row = table.AddRow();
            for (int i = 1; i < numberOfColumns; ++i)
            {
                if (i <= _data.Fullnames.Count)
                    row.Cells[i + 1].AddParagraph(_data.Fullnames[i - 1]);
            }
        }

        private void AddDayToTheTable(Table table, IDay day)
        {
            // Add row to the table
            Row row = table.AddRow();

            // Add number of this day and it's abbreviation to the first 2 columns
            row.Cells[0].AddParagraph(day.FormattedDayOfMonth);
            row.Cells[1].AddParagraph(_names.GetDayOfWeekAbbreviation(day.DayOfWeek));

            // Add 'SUNDAY' centered text to all of the fullname columns
            if (day.DayOfWeek == DayOfWeek.Sunday)
                for (int i = 0; i < _data.MaxNumberOfFullnames; ++i)
                {
                    row.Cells[i + 2].AddParagraph(_names.GetDayOfWeekName(DayOfWeek.Sunday).ToUpper());
                    row.Cells[i + 2].Format.Alignment = ParagraphAlignment.Center;
                }
        }
        #endregion

        #region Private styling methods
        private void ChangeDocumentHeadingFormat(Paragraph documentHeading)
        {
            // Change document heading format
            documentHeading.Format.Font.Name = "Times New Roman";
            documentHeading.Format.Alignment = ParagraphAlignment.Center;
            documentHeading.Format.Font.Size = 18.0;
            documentHeading.Format.Font.Bold = true;
            documentHeading.Format.SpaceAfter = 15.0;
        }

        private void ChangeTableFormat(Table table)
        {
            // Add black borders to the table
            table.Borders.Color = Colors.Black;

            // Remove first two columns borders
            table.Rows[0].Cells[0].Borders.Top.Visible = false;
            table.Rows[0].Cells[1].Borders.Top.Visible = false;
            table.Rows[0].Cells[0].Borders.Left.Visible = false;
            table.Rows[0].Cells[1].Borders.Left.Visible = false;
            table.Rows[0].Cells[0].Borders.Right.Visible = false;
            table.Rows[0].Cells[1].Borders.Right.Visible = false;

            // Change first two columns width
            table.Columns[0].Width = _firstColumnWidth;
            table.Columns[1].Width = _secondColumnWidth;

            // Change width of fullnames columns
            for (int i = 0; i < _data.MaxNumberOfFullnames; i++)
                table.Columns[i + 2].Width = _fullnamesColumnWidth;

            // Change table default font name and size
            table.Format.Font.Name = "Times New Roman";
            table.Format.Font.Size = 12.0;

            // Change table headings font size and weight
            table.Rows[0].Format.Font.Size = 14.0;
            table.Rows[0].Format.Font.Bold = true;
        }

        private void AddColorsToTheTable(Table table)
        {
            Row firstRow = table.Rows[0];

            // Set background color of fullnames row to gray
            firstRow.Shading.Color = FullnamesBackgroundColor;

            // Reset first and second column background color
            firstRow.Cells[0].Shading.Color = Colors.White;
            firstRow.Cells[1].Shading.Color = Colors.White;

            // Get a list of saturdays and sundays
            var weekendDays = _data.Days.Where(d => d.DayOfWeek == DayOfWeek.Saturday || d.DayOfWeek == DayOfWeek.Sunday);

            // Change rows background depend if its saturday or sunday
            foreach (IDay day in weekendDays)
            {
                Row row = table.Rows[day.DayOfMonth];

                if (day.DayOfWeek == DayOfWeek.Saturday)
                    row.Shading.Color = SaturdayBackgroundColor;
                else
                    row.Shading.Color = SundayBackgroundColor;
            }
        }
        #endregion
    }
}
