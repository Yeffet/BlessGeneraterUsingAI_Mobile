using Android;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using System;

namespace BlessMaker
{
    public class ColorPickerDialog : AppCompatDialog
    {
        LinearLayout _mainLayout;
        TextView colorRed, colorGreen, colorBlue, colorYellow, colorPurple;
        private readonly Action<string> _onColorSelected;

        public ColorPickerDialog(Context context, Action<string> onColorSelected) : base(context)
        {
            _onColorSelected = onColorSelected;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.dialog_color_picker);

            var colorButtons = new[]
            {
                FindViewById<TextView>(Resource.Id.colorPurple),
                FindViewById<TextView>(Resource.Id.colorRed),
                FindViewById<TextView>(Resource.Id.colorGreen),
                FindViewById<TextView>(Resource.Id.colorBlue),
                FindViewById<TextView>(Resource.Id.colorYellow)
                
            };

            var colors = new[]
            {
                "#D7A0FF", "#FF0000", "#00FF00", "#0000FF", "#FFFF00"
            };

            for (int i = 0; i < colorButtons.Length; i++)
            {
                var color = colors[i];
                colorButtons[i].SetBackgroundColor(Color.ParseColor(color));
                colorButtons[i].Click += (s, e) =>
                {
                    _onColorSelected(color);
                    Dismiss();
                };
            }
        }

        
    }
}
