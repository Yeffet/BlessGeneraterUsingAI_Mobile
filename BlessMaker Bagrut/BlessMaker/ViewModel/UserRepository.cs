using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViewModel
{
    public class UserRepository
    {
        public SQLiteConnection db;
        protected static UserRepository me;
        static UserRepository()
        {
            me = new UserRepository();
        }
        protected UserRepository()
        {
            db = new SQLiteConnection(Constants.DbFilePath);
            db.CreateTable<Bless>();
        }
        public static void EditBless(Bless bless)
        {
            me.db.Update(bless);
        }
        public static int AddBless(Bless bless)
        {

            me.db.Execute("INSERT INTO blesses (Recipient,Subject, resultTv, Date)" +
            "VALUES (?,?,?,?)", new string[] { bless.Recipient, bless.Subject, bless.ResultTv, bless.Date });
            return me.db.ExecuteScalar<int>("SELECT MAX(Id) FROM blesses");
        }
        public static void DeleteBless(int id)
        {
            me.db.Delete<Bless>(id);
        }
        public static Bless GetBless(int id)
        {
            return me.db.Get<Bless>(id);
        }
        public static List<Bless> List()
        {
            var output = me.db.Query<Bless>("SELECT * FROM blesses");
            return output.ToList();
        }
    }
}