﻿using System;

namespace AttendanceListGenerator.Core.Data
{
    public interface ILocalizedNames
    {
        string DocumentAuthor { get; }
        string ApplicationCatalogName { get; }
        string DocumentsCatalogName { get; }
        string GetDayOfWeekName(DayOfWeek dayOfWeek);
        string GetDayOfWeekAbbreviation(DayOfWeek dayOfWeek);
        string GetMonthName(Month month);
        string GetDocumentTitle(Month month, int Year);
        string GetHolidayName(Holiday holiday);
    }
}
