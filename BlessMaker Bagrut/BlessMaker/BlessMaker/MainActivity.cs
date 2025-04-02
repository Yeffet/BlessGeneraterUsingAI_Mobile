using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using AndroidX.AppCompat.App;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using Button = Android.Widget.Button;
using Models;
using ViewModel;
using Javax.Security.Auth;
using Android.Content;
using System.Globalization;
using Xamarin.Essentials;
using Android.Graphics.Drawables;
using Android.Graphics;

namespace BlessMaker
{
    [Activity(Label = "@string/app_name")]
    public class MainActivity : AppCompatActivity
    {
        LinearLayout _mainLayout;

        TextView PickADate, DateView, btnGen, btnSave, MyBlesses, Settings;
        EditText ageEt, recipientEt, subjectEt, notesEt, resultTv;
        int id;
        DateTime getDate;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            InitilizeViews();
            Intent intent = this.Intent;
            if (intent != null)
            {
                recipientEt.Text = intent.GetStringExtra("recipient");
                //ageEt.Text = intent.GetIntExtra("age", 0).ToString();
                subjectEt.Text = intent.GetStringExtra("subject");
                notesEt.Text = intent.GetStringExtra("notes");
                resultTv.Text = intent.GetStringExtra("result");
                string dateString = intent.GetStringExtra("date");
                if (dateString != null)
                {
                    getDate = DateTime.ParseExact(dateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    DateView.Text = "Date: " + getDate.Day + "/" + getDate.Month + "/" + getDate.Year;// write the date in "DataView"
                }

                id = intent.GetIntExtra("id", -1);


            }
            SetBackground();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        void InitilizeViews()
        {
            recipientEt = FindViewById<EditText>(Resource.Id.editText1);
            //ageEt = FindViewById<EditText>(Resource.Id.editText2);
            subjectEt = FindViewById<EditText>(Resource.Id.editText3);
            notesEt = FindViewById<EditText>(Resource.Id.editText4);
            resultTv = FindViewById<EditText>(Resource.Id.textView1);

            DateView = FindViewById<TextView>(Resource.Id.DateView);
            PickADate = FindViewById<TextView>(Resource.Id.PickDate);
            btnGen = FindViewById<TextView>(Resource.Id.BtnGenerate);
            btnSave = FindViewById<TextView>(Resource.Id.BtnSave);

            Settings = FindViewById<TextView>(Resource.Id.Settings);
            Settings.Click += delegate
            {
                StartActivity(typeof(WelcomePage));
            };

            MyBlesses = FindViewById<TextView>(Resource.Id.MyBlesses);

            MyBlesses.Click += delegate 
            {
                StartActivity(typeof(BlessDataBase));
            };
            PickADate.Click += delegate  
            {
                StartActivityForResult(typeof(DatePick), 123);
            };

            btnGen.Click += delegate  
            {
                getWishes(recipientEt.Text, subjectEt.Text, notesEt.Text, ChangeResult);
            };
            btnSave.Click += delegate 
            {
                if(getDate.Year < 1000 || recipientEt.Text.Trim() == "" || subjectEt.Text.Trim() == "")
                {
                    RunOnUiThread(() =>
                    {
                        Toast.MakeText(this, "Missing date or recipient or subject", ToastLength.Short);
                    });
                    return;
                }
                if (id >= 0)
                {
                    UpdateToDataBase(id);
                }
                else
                {
                    AddToDataBase();
                }
                StartActivity(typeof(BlessDataBase));
            };

            _mainLayout = FindViewById<LinearLayout>(Resource.Id.mainlay);

        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if(requestCode == 123) 
            {
                if(resultCode == Result.Ok) // if result was succsessful
                {
                    getDate = DateTime.ParseExact(data.GetStringExtra("date"),"dd/MM/yyyy",CultureInfo.InvariantCulture);
                    DateView.Text = "Date: " + getDate.Day + "/" + getDate.Month + "/" + getDate.Year;// write the date in "DataView"

                }
            }
            base.OnActivityResult(requestCode, resultCode, data);
        }
        class Message
        {
            public string role;
            public string content;
            public Message(string role, string content)
            {
                this.role = role;
                this.content = content;
            }
        }
        class ChatGPTrequest //defines The parameters to the AI for the request
        {
            public string model = "gpt-3.5-turbo";
            public List<Message> messages;
            public ChatGPTrequest(List<Message> messages)
            {
                this.messages = messages;
            }
        }
        void ChangeResult(string text)
        {
            RunOnUiThread(() => // update the UI thread
            {
                resultTv.Text = text;
            });
        }
        class gptMessage
        {
            public string content { get; set; }
        }
        class Choice
        {
            public gptMessage Message { get; set; }
        }
        class GPTResponse
        {
            public List<Choice> Choices { get; set; }
        }
        async void getWishes(string recipient, string subject, string notes, Action<string> ChangeText) // changeTexe is a function that get called at the end with the result
        {
            try
            {
                //massege to the API with instructions
                Message systemMessage = new Message("system", "You write wish cards with the recipient name, subject and notes about the recipient and occasion," +
                    " be ready to recieve the info from the client. ALWAYS USE THE LANGUAGE OF THE NEXT MESSAGE FIELDS (RECIPIENT AND SUCH)," +
                    "DON'T ASK QUESTIONS WRITE WITH THE INFO YOU WERE GIVEN AND NOTHING MORE." +
                    " dont show a titel for the blessing and dont write the word recipient in the bless," +
                    " i want you to use the info only to make the bless more acurate for the recipient," +
                    "and return a nice and organized greeting that is convenient for the user to see");
                //massege from the user to the API with the parameters the user wants
                Message userMessage = new Message("user", $"Recipient Name: {recipient}, Subject: {subject}, Notes: {notes}");
                List<Message> messages = new List<Message>();
                messages.Add(systemMessage);
                messages.Add(userMessage);
                string result = await CallAPI(new ChatGPTrequest(messages));
                GPTResponse responseObj = JsonConvert.DeserializeObject<GPTResponse>(result);
                ChangeText(responseObj.Choices[0].Message.content); // example from openAi
            }
            catch
            {
                return;
            }
        }
        public async Task<string> CallAPI(object requestObject) // send an http request to the API
        {
            try
            {

                string endpoint = "https://api.openai.com/v1/chat/completions";
                var _client = new HttpClient(); //send request 
                string token = "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";
                string requestBodyString = JsonConvert.SerializeObject(requestObject);

                var request = new HttpRequestMessage //prepere the request
                {
                    Content = new StringContent(requestBodyString, Encoding.UTF8, "application/json"),
                    Method = HttpMethod.Post,
                    RequestUri = new Uri(endpoint)
                };

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token); // determant the API key 

                var result = await _client.SendAsync(request);
                var content = result.Content.ReadAsStringAsync().Result;


                return content;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void AddToDataBase()
        {
            Bless bless = new Bless()
            {
                Recipient = recipientEt.Text,
                Subject = subjectEt.Text,
                ResultTv = resultTv.Text,
                Date = getDate.ToString("dd/MM/yyyy")
            };

            var id = UserRepository.AddBless(bless); // add bless to the data base
            var newBless = UserRepository.GetBless(id); // The user is pulled from the database according to the ID for the purpose of checking for integrity

            if (newBless == null)
                Toast.MakeText(this, $"Bless: Recipient={bless.Recipient}, Subject={bless.Subject} wasn't properly saved!",
                ToastLength.Long).Show();
            else
                Toast.MakeText(this, $"Bless saved successfully", ToastLength.Long).Show();
        }
        private void UpdateToDataBase(int updateId)
        {
            Bless bless = new Bless()
            {
                Recipient = recipientEt.Text,
                Subject = subjectEt.Text,
                ResultTv = resultTv.Text,
                Date = getDate.ToString("dd/MM/yyyy"),
                Id = updateId,
            };

            UserRepository.EditBless(bless); // Updating a bless to the data base

            Toast.MakeText(this, $"Bless saved successfully", ToastLength.Long).Show();


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