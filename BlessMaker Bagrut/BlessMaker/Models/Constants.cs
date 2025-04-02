using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Nio.FileNio;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Environment = System.Environment;

namespace Models
{
    public class Constants
    {
        public static readonly string DbFilePath =
        Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "testDB.db");
    }
}
//System.Environment.SpecialFolder.Personal