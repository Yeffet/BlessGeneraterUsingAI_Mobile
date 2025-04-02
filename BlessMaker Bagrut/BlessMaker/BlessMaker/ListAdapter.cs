using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlessMaker
{

    internal class ListAdapter : BaseAdapter<Bless>
    {

        private List<Bless> blesses;
        private LayoutInflater inflater;
        public ListAdapter(List<Bless> blesses, Context context)
        {
            this.blesses = blesses;
            this.inflater = LayoutInflater.FromContext(context);
        }

        public override int Count => blesses.Count;

        public override Bless this[int position]
        {
            get
            {
                return blesses[position];
            }
        }
        public override long GetItemId(int position) => position;

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            if (convertView == null)
            {
                convertView = inflater.Inflate(Resource.Layout.bless_item, parent, false); //take the xml layout and set the text of the current item to the current bless

            }
            TextView blessNameTv = convertView.FindViewById<TextView>(Resource.Id.blessName);

            blessNameTv.Text = blesses[position].Recipient + " " + blesses[position].Subject;
            TextView blessDateTv = convertView.FindViewById<TextView>(Resource.Id.blessDate);
            blessDateTv.Text = blesses[position].Date == null? "" : blesses[position].Date;
            return convertView;
        }

    }

    internal class RecipeAdapterViewHolder : Java.Lang.Object
    {
        //Your adapter views to re-use
        //public TextView Title { get; set; }
    }
}