﻿namespace TND.Domain.Interfaces.Services
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTimeUtc();
        DateOnly GetCurrentDateUtc();
    }
}
