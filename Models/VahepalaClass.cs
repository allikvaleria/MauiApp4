﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace MauiApp4.Models
{
    [Table("Vahepala")]
    public class VahepalaClass
    {
        [PrimaryKey, AutoIncrement, Column("_id")]
        public int Vahepala_id { get; set; }
        public DateTime Kuupaev { get; set; }
        public TimeSpan Kallaaeg { get; set; }
        public string? Roa_nimi { get; set; }
        public int Valgud { get; set; }
        public int Rasvad { get; set; }
        public int Susivesikud { get; set; }
        public int Kalorid { get; set; }
        public byte[]? Toidu_foto { get; set; }
    }
}
