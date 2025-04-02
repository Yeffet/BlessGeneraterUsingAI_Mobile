using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using Javax.Security.Auth;
using Models;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using ViewModel;
using Xamarin.Essentials;

namespace BlessMaker
{

    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class WelcomePage : Activity
    {
        Bitmap background;
        LinearLayout _mainLayout;

        TextView btnChangeBackround, NoCloseEvents;
        ImageButton Write;
        ListView DataLv;
        List<Bless> blesses;
        ISharedPreferences sp;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Create your application here
            SetContentView(Resource.Layout.WelcomePage);

            InitilizeViews();
            SetBackground();
        }

        void SetBackground() 
        {
            ISharedPreferences temp = this.GetSharedPreferences("details", Android.Content.FileCreationMode.Private);
            int State = temp.GetInt("usingColor", -1); // if color == -1 color is null

            if (State == -1) 
            {
                return;
            }
            
            if (State == 0) // if background is image
            {
                string B64Image = temp.GetString("background", null);

                Bitmap bitmap = Base64ToBitmap(B64Image);
                Drawable drawable = new BitmapDrawable(Resources, bitmap);
                _mainLayout.SetBackgroundDrawable(drawable);
            }
            else // if background is color
            {
                string color = temp.GetString("color", null);
                _mainLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor(color));
            }
        }
        void InitilizeViews()
        {
            sp = this.GetSharedPreferences("details", Android.Content.FileCreationMode.Private);
            string color = sp.GetString("color", null);

            Write = FindViewById<ImageButton>(Resource.Id.WriteABless);
            DataLv = FindViewById<ListView>(Resource.Id.dataLv2);
            DataLv.ItemClick += DataLv_ItemClick;


            btnChangeBackround = FindViewById<TextView>(Resource.Id.BtnChangebackround);
            _mainLayout = FindViewById<LinearLayout>(Resource.Id.mainlay);

            UpdateList();

            Write.Click += delegate  
            {
                StartActivity(typeof(MainActivity));
            };

            
            btnChangeBackround.Click += (s, e) =>
            {
                var alert = new AlertDialog.Builder(this);
                alert.SetTitle("Choose an action");
                alert.SetItems(new[] { "Choose Color", "Choose Photo", "Capture Photo"}, (sender, args) =>
                {
                    if (args.Which == 0)
                    {
                        // Color Picker Logic
                        var colorDialog = new ColorPickerDialog(this, (color) =>
                        {
                            var editor = sp.Edit();
                            editor.PutString("color", color);
                            editor.PutInt("usingColor", 1);
                            editor.Commit();
                            _mainLayout.SetBackgroundColor(Android.Graphics.Color.ParseColor(color));
                        });
                        colorDialog.Show();

                        

                    }
                    else if (args.Which == 1)
                    {
                        PickPhoto();
                    }
                    else
                    {
                        TakePhoto();
                    }
                });
                alert.Show();
            };
            
        }

        private void DataLv_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            Intent editIntent = new Intent(this, typeof(MainActivity));
            editIntent.PutExtra("id", blesses[e.Position].Id);
            editIntent.PutExtra("recipient", blesses[e.Position].Recipient);
            editIntent.PutExtra("subject", blesses[e.Position].Subject);
            editIntent.PutExtra("notes", blesses[e.Position].Notes);
            editIntent.PutExtra("result", blesses[e.Position].ResultTv);
            editIntent.PutExtra("date", blesses[e.Position].Date);

            StartActivity(editIntent);
        }

        private void UpdateList()
        {
            NoCloseEvents = FindViewById<TextView>(Resource.Id.NoCloseEvents);
            blesses = UserRepository.List();

            List<Bless> top3 = blesses
                .Where(bless => !string.IsNullOrEmpty(bless.Date)) // filter out null date
                .OrderByDescending(bless => DateTime.ParseExact(bless.Date, "dd/MM/yyyy", CultureInfo.InvariantCulture)) // order the list by recent date
                .Take(3) //take the first 3
                .ToList(); //convert to list
            DataLv.Adapter = new ListAdapter(top3, this);

            if (blesses != null)
                NoCloseEvents.Text = "";

        }
        public Bitmap Base64ToBitmap(string base64String) // convert it to image
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


        async void TakePhoto()
        {
            await CrossMedia.Current.Initialize();
            var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                PhotoSize = Plugin.Media.Abstractions.PhotoSize.Small,
                CompressionQuality = 40,
                Name = "BackgroundImage.jpg",
                Directory = "Sample"
            });
            if (file == null) return;
            //Convert file to byte array and set the resulting bitmap to ImageView
            byte[] imageArray = System.IO.File.ReadAllBytes(file.Path);
            background = BitmapFactory.DecodeByteArray(imageArray, 0, imageArray.Length);
            RunOnUiThread(() =>
            {
                Drawable drawable = new BitmapDrawable(Resources, background);

                _mainLayout.SetBackgroundDrawable(drawable);
                ISharedPreferencesEditor editor = sp.Edit();
                editor.PutString("background", BitmapToBase64(background));
                editor.PutInt("usingColor", 0);
                editor.Commit();

            });
        }
        public string BitmapToBase64(Android.Graphics.Bitmap bitmap)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            using (MemoryStream ms = new MemoryStream())
            {
                // Compress the bitmap into the memory stream in PNG format
                bitmap.Compress(Android.Graphics.Bitmap.CompressFormat.Png, 100, ms);

                // Get the byte array from the memory stream
                byte[] byteArray = ms.ToArray();

                // Convert byte array to Base64 string
                return Convert.ToBase64String(byteArray);
            }
        }


        async void PickPhoto()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported)
            {
                // Display an error message if not supported
                return;
            }

            var mediaFile = await CrossMedia.Current.PickPhotoAsync();

            if (mediaFile == null)
            {
                // User likely canceled the action
                return;
            }

            // Convert to Bitmap
            using (var stream = mediaFile.GetStream())
            {
                background = await BitmapFactory.DecodeStreamAsync(stream);
                RunOnUiThread(() =>
                {
                    Drawable drawable = new BitmapDrawable(Resources, background);

                    _mainLayout.SetBackgroundDrawable(drawable);
                    ISharedPreferencesEditor editor = sp.Edit();
                    editor.PutString("background", BitmapToBase64(background));
                    editor.PutInt("usingColor", 0);
                    editor.Commit();
                });
            }
        }
    }
}