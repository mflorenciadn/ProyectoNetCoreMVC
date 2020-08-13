using System;

namespace MVC.ReservaDeCine.Extensions
{
    public static class DateTimeExtensions
    {
        public static int CalcularEdad(this DateTime value)
        {
            var anios = DateTime.Today.Year - value.Year;
            return value > DateTime.Today.AddYears(-anios) ? anios - 1 : anios;
        }
    }
}
