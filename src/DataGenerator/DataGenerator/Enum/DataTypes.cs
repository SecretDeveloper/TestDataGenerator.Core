using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataGenerator.Enum
{
    public enum DataTypes
    {
        Name,
        Phone,
        Email,
        StreetAddress,
        City,
        ZipCode,
        StateProvince,
        Country,
        Date,
        Text,
        /// <summary>
        /// Fixed string
        /// </summary>
        TextFixed,
        /// <summary>
        /// Random number of words
        /// </summary>
        TextRandom,
        Custom,
        AutoIncrement,
        NumberRange,
        /// <summary>
        /// Auto incrementing value
        /// </summary>
        Alphanumeric,
        List
    }
}
