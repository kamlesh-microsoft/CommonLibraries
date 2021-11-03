﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonLibraries.Graal.Enums;

namespace CommonLibraries.Graal.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// Возвращает дату, отстоящую от заданной на указанное число единиц времени таймфрейма.
        /// </summary>
        /// <param name="date">Заданная дата.</param>
        /// <param name="timeFrame">Таймфрейм.</param>
        /// <param name="increment">Добавляемое число единиц времени таймфрейма.</param>
        /// <returns>Дата, отстоящая от заданной на указанное число единиц времени таймфрейма</returns>
        public static DateTime AddDate(this DateTime date, TimeFrameEnum timeFrame, int increment)
        {
            switch (timeFrame)
            {
                case (TimeFrameEnum.min1):
                    return date.AddMinutes(increment);
                case (TimeFrameEnum.min4):
                    return date.AddMinutes(increment * 4);
                case (TimeFrameEnum.H1):
                    return date.AddHours(increment);
                case (TimeFrameEnum.D1):
                    return date.AddDays(increment);
                case (TimeFrameEnum.W1):
                    return date.AddDays(increment * 7);
                case (TimeFrameEnum.M1):
                    return date.AddMonths(increment);
                case (TimeFrameEnum.Seasonly):
                    return date.AddMonths(increment * 3);
                case (TimeFrameEnum.Y1):
                    return date.AddYears(increment);

                default:
                    throw new ArgumentException($"Неподходящий таймфрейм - {timeFrame}", nameof(timeFrame));
            }
        }

        /// <summary>
        /// Возвращает разницу между датами в единицах времени таймфрейма.
        /// </summary>
        /// <param name="dt1">Дата 1.</param>
        /// <param name="dt2">Дата 2.</param>
        /// <param name="timeFrame">Таймфрейм.</param>
        /// <returns>Разница между датами в единицах времени таймфрейма.</returns>
        public static int DatesDifferent(DateTime dt1, DateTime dt2, TimeFrameEnum timeFrame)
        {
            var diff = new DateTime(Math.Max(dt1.Ticks, dt2.Ticks)) - new DateTime(Math.Min(dt1.Ticks, dt2.Ticks));

            return timeFrame switch
            {
                TimeFrameEnum.tick => (int)diff.TotalSeconds,
                TimeFrameEnum.min1 => (int)diff.TotalMinutes,
                TimeFrameEnum.min4 => (int)(diff.TotalMinutes / 4),
                TimeFrameEnum.H1 => (int)diff.TotalHours,
                TimeFrameEnum.D1 => (int)diff.TotalDays,
                TimeFrameEnum.W1 => (int)(diff.TotalDays / 7),
                TimeFrameEnum.M1 => (int)(diff.TotalDays / 30),
                TimeFrameEnum.Seasonly => (int)(diff.TotalDays / 120),
                TimeFrameEnum.Y1 => (int)(diff.TotalDays / 365.25),
                _ => throw new NotSupportedException($"Not supported timeframe '{timeFrame}'")
            };
        }

        /// <summary>
        /// Приведение даты к единому формату - усреднение незначащих разрядов
        /// </summary>
        /// <param name="date"></param>
        /// <param name="timeFrame"></param>
        /// <returns></returns>
        public static DateTime CorrectDateByTF(DateTime date, TimeFrameEnum timeFrame)
        {
            int year = date.Year, month = date.Month, day = date.Day, hour = date.Hour, min = date.Minute, sec = date.Second;

            switch (timeFrame)
            {
                case TimeFrameEnum.Y1: month = 6; day = 15; hour = 12; min = 0; sec = 0; break;
                case TimeFrameEnum.Seasonly:
                    {
                        if (date.Month <= 6)
                            month = date.Month <= 3 ? 2 : 5;
                        else
                            month = date.Month <= 9 ? 8 : 11;

                        day = 15; hour = 12; min = 0; break;
                    }
                case TimeFrameEnum.M1: day = 15; hour = 12; min = 0; sec = 0; break;
                case TimeFrameEnum.W1:
                    {
                        switch (date.DayOfWeek)
                        {
                            case DayOfWeek.Monday: date = date.AddDays(2); break;
                            case DayOfWeek.Tuesday: date = date.AddDays(1); break;
                            case DayOfWeek.Wednesday: date = date.AddDays(0); break;
                            case DayOfWeek.Thursday: date = date.AddDays(-1); break;
                            case DayOfWeek.Friday: date = date.AddDays(-2); break;
                            case DayOfWeek.Saturday: date = date.AddDays(-3); break;
                            case DayOfWeek.Sunday: date = date.AddDays(-4); break;
                        }
                        year = date.Year; month = date.Month; day = date.Day; hour = 12; min = 0; sec = 0; break;
                    }
                case TimeFrameEnum.D1: hour = 12; min = 0; sec = 0; break;
                case TimeFrameEnum.H1: min = 30; sec = 0; break;
                case TimeFrameEnum.min4: min = (int)Math.Floor((double)min / 4) * 4 + 2; sec = 0; break;
                case TimeFrameEnum.min1: sec = 30; break;
                case TimeFrameEnum.tick: break;

                default:
                    throw new NotSupportedException($"Not supported timeframe '{timeFrame}'");
            }

            return new DateTime(year, month, day, hour, min, sec);
        }

        /// <summary>
        /// Возвращает среднее значение между двумя датами
        /// </summary>
        /// <param name="dt1">Первая дата</param>
        /// <param name="dt2">Вторая дата</param>
        /// <returns></returns>
        public static DateTime GetMedian(DateTime dt1, DateTime dt2)
        {
            if (dt1 == dt2)
            {
                return dt1;
            }

            var min = new DateTime(Math.Min(dt1.Ticks, dt2.Ticks));
            var max = new DateTime(Math.Max(dt1.Ticks, dt2.Ticks));

            return new DateTime(min.Ticks + (max.Ticks - min.Ticks) / 2);
        }

        /// <summary>
        /// Последняя возможно существующая дата котировки на текущий момент
        /// </summary>
        /// <param name="timeFrame"></param>
        /// <returns></returns>
        public static DateTime GetPossibleEndDate(TimeFrameEnum timeFrame, DateTime now)
        {
            return timeFrame switch
            {
                TimeFrameEnum.tick => now,
                TimeFrameEnum.min1 => new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 59).AddMinutes(-1),
                TimeFrameEnum.min4 => throw new NotImplementedException(),//  return now;
                TimeFrameEnum.H1 => new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59).AddHours(-1),
                TimeFrameEnum.D1 => new DateTime(now.Year, now.Month, now.Day, 23, 59, 59).AddDays(-1),
                TimeFrameEnum.W1 => throw new NotImplementedException(),//  return new DateTime(now.Year, now.Month, now.Day, now.Hour, 59, 59).AddHours(-1);
                TimeFrameEnum.M1 => new DateTime(now.Year, now.Month, 28, 23, 59, 59).AddMonths(-1),
                TimeFrameEnum.Seasonly => throw new NotImplementedException(),
                TimeFrameEnum.Y1 => new DateTime(now.Year, 12, 31, 23, 59, 59).AddYears(-1),
                _ => throw new NotSupportedException($"Not supported timeframe '{timeFrame}'"),
            };
        }
    }
}
