using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models
{
    [Table("blesses")]
    public class Bless
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        public string Recipient { get; set; }
        public int Age { get; set; }
        public string Subject { get; set; }
        public string Notes { get; set; }
        public string ResultTv { get; set; }
        public string Date { get; set; }

    }
}