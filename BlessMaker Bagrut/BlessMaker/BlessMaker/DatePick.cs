using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlessMaker;
using Javax.Security.Auth;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlessMaker
{
    [Activity(Label = "DatePick")]
    public class DatePick : Activity
    {
        TextView SaveDate, txtDate;
        DatePicker datePicker;
        LinearLayout _mainLayout;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Calander);
            // Create your application here
            InitilizeViews();
            SetBackground();
        }

        void InitilizeViews()
        {
            _mainLayout = FindViewById<LinearLayout>(Resource.Id.mainlay);

            SaveDate = FindViewById<TextView>(Resource.Id.SaveDate);
            datePicker = FindViewById<DatePicker>(Resource.Id.DatePicker);
            txtDate = FindViewById<TextView>(Resource.Id.textDate);
            txtDate.Text = getDate();
            SaveDate.Click += delegate  
            {
                txtDate.Text = getDate();
                Intent editIntent2 = new Intent(this, typeof(MainActivity));
                editIntent2.PutExtra("date", datePicker.DateTime.ToString("dd/MM/yyyy")); 
                SetResult(Result.Ok ,editIntent2);
                Finish();
            };
            datePicker.DateChanged += DatePicker_DateChanged;
        }

        private void DatePicker_DateChanged(object sender, DatePicker.DateChangedEventArgs e)
        {
            txtDate.Text = getDate();
        }

        private string getDate()
        {
            StringBuilder strCurrentDatedate = new StringBuilder();
            int month = datePicker.Month + 1;
            strCurrentDatedate.Append("Date: " + datePicker.DayOfMonth + "/" + month + "/" + datePicker.Year);
            return strCurrentDatedate.ToString();
        }

        void SetBackground()
        {
            ISharedPreferences temp = this.GetSharedPreferences("details", Android.Content.FileCreationMode.Private);
            int State = temp.GetInt("usingColor", -1);

            if (State == -1)
            {
                return;
            }

            if (State == 0)
            {
                string B64Image = temp.GetString("background", null);

                Bitmap bitmap = Base64ToBitmap(B64Image);
                Drawable drawable = new BitmapDrawable(Resources, bitmap);
                _mainLayout.SetBackgroundDrawable(drawable);
            }
            else
            {
                string color = temp.GetString("color", null);
                _mainLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor(color));
            }
        }
        public Bitmap Base64ToBitmap(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                throw new ArgumentNullException(nameof(base64String));
            }

            // Decode the Base64 string to a byte array
            byte[] imageBytes = Convert.FromBase64String(base64String);

            // Convert the byte array to a Bitmap
            return BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
        }
    }
}