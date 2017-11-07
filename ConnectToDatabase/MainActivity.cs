using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Java.Lang;

namespace ConnectToDatabase
{
    [Activity(Label = "HttpSample", MainLauncher = true)]
    public class MainActivity : Activity, IServiceConnection, Java.Lang.IRunnable
    {
        /* objetos de interface (TextView, Buttons, EditText, etc são declarados aqui */

        private int pingTime;
        private Intent i;
        Handler handler;

        private IServiceConnection connection;
        private HTTPService myService;
        private HTTPRequests.Services currService;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            i = new Intent(this, typeof(HTTPService));

            connection = this;

            handler = new Handler();

            /* objetos de interface são instânciados aqui (FindViewById) */

            pingTime = 0;

            StartService(i);
        }

        protected override void OnResume()
        {
            base.OnResume();
            Toast.MakeText(this, "Binding service!!", ToastLength.Short).Show();
            BindService(i, connection, Bind.AutoCreate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            handler.RemoveCallbacks(this);

            if (myService.Requests != null)
            {
                Toast.MakeText(this, "Unbinding service!!", ToastLength.Short).Show();
                UnbindService(connection);
                StopService(i);
            }
        }

        /* exemplo da chamada do web service de login */
        private void Login(string login, string pass)
        {
            myService.Requests.Login(login, pass);
            currService = HTTPRequests.Services.LOGIN;
            pingTime = 0;
            handler.Post(this);
        }

        void IServiceConnection.OnServiceConnected(ComponentName name, IBinder service)
        {
            MyBoundService binder = (MyBoundService)service;
            myService = binder.MyService;
            myService.Requests.Connect();
            currService = HTTPRequests.Services.CONNECT;
            pingTime = 0;
            handler.Post(this);
        }

        void IServiceConnection.OnServiceDisconnected(ComponentName name)
        {
            myService = null;
            handler.RemoveCallbacks(this);
        }

        void IRunnable.Run()
        {
            if (myService.Requests.ServerResponse() != null)
            {
                if (currService == HTTPRequests.Services.CONNECT)
                {
                    myService.Requests.ServerResponse(); //resposta que chega do servidor já aqui na interface
                }
                else if (currService == HTTPRequests.Services.LOGIN)
                {
                    myService.Requests.ServerResponse(); //resposta que chega do servidor já aqui na interface
                }
            }
            else
            {
                if (pingTime < 5)
                {
                    pingTime++;
                    handler.PostDelayed(this, 3000);
                    Toast.MakeText(this, "Waiting server response - Attempt: " + pingTime, ToastLength.Short).Show();
                }
                else
                {
                    pingTime = 0;
                    Toast.MakeText(this, "Server doesn't response. Try again later.", ToastLength.Short).Show();
                    Finish(); //backing to previous Activity
                }
            }

            myService.Requests.NullServerResponse();
        }

        private void ConnectionResponse(object serverResponse)
        {
            if ((bool)serverResponse)
            {
                Toast.MakeText(this, "Server connected!!", ToastLength.Short).Show();
            }
            else
            {
                UnbindService(connection);
                StopService(i);

                Toast.MakeText(this, "Problems to connect with server", ToastLength.Short).Show();
            }
        }

        private void LoginResponse(object serverResponse)
        {
            //Faz algo com a resposta
        }
    }
}

