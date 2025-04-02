using Android.App;
using Android.Content;
using Android.Graphics.Drawables;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BlessMaker;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ViewModel;

namespace BlessMaker
{
    [Activity(Label = "BlessDataBase")]
    public class BlessDataBase : Activity
    {
        TextView Delete, Add, Edit;
        List<Bless> blesses;
        TextView tvDataSquare;
        ListView dataLv;
        int selectedPosition = -1;

        LinearLayout _mainLayout;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.DataBase);
            // Create your application here
            InitilizeViews();
            InitilizeDB();
            UpdateList();
            SetBackground();
        }
        private void UpdateList()
        {
            blesses = UserRepository.List();
            dataLv.Adapter = new ListAdapter(blesses, this);
        }

        private void InitilizeViews()
        {
            Add = FindViewById<TextView>(Resource.Id.btn_add);
            Delete = FindViewById<TextView>(Resource.Id.btn_delete);
            dataLv = FindViewById<ListView>(Resource.Id.dataLv1);
            dataLv.ItemClick += DataLv_ItemClick;
            tvDataSquare = FindViewById<TextView>(Resource.Id.tvDataSquare);

            _mainLayout = FindViewById<LinearLayout>(Resource.Id.mainlay);

            Edit = FindViewById<TextView>(Resource.Id.btn_edit);
            Edit.Click += delegate
            {
                if (selectedPosition == -1)
                {
                    return;
                }
                Intent editIntent = new Intent(this, typeof(MainActivity));
                editIntent.PutExtra("id", blesses[selectedPosition].Id);
                editIntent.PutExtra("recipient", blesses[selectedPosition].Recipient);
                editIntent.PutExtra("subject", blesses[selectedPosition].Subject);
                editIntent.PutExtra("notes", blesses[selectedPosition].Notes);
                editIntent.PutExtra("result", blesses[selectedPosition].ResultTv);
                editIntent.PutExtra("date", blesses[selectedPosition].Date);

                StartActivity(editIntent);
            };

            Add.Click += delegate  
            {
                StartActivity(typeof(MainActivity));
            };
            Delete.Click += delegate
            {
                if (selectedPosition == -1)
                {
                    return;
                }
                UserRepository.DeleteBless(blesses[selectedPosition].Id);
                StartActivity(typeof(BlessDataBase));
            };
        }

        private void DataLv_ItemClick(object sender, AdapterView.ItemClickEventArgs e) 
        {
            tvDataSquare.Text = blesses[e.Position].ResultTv;
            selectedPosition = e.Position;
        }

        private void InitilizeDB()
        {
            blesses = new List<Bless>();
            foreach (var bless in UserRepository.List())
            {
                blesses.Add(bless);
            }

        }

        protected override void OnResume()
        {
            base.OnResume();
            UpdateList();
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