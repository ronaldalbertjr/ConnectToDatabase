using System.Collections.Generic;
using Java.Lang;
using Android.Content;

namespace HttpSample
{
    class HTTPRequests
    {
        private Dictionary<object, object> forms;
        private HTTPSendToServer sendToServer;
        private object serverResponse = null;

        public enum Services
        {
            //CONNECT, enumerado de serviços
        }

        public HTTPRequests()
        {
            forms = new Dictionary<object, object>();
            sendToServer = new HTTPSendToServer();
        }

        public object ServerResponse()
        {
            return serverResponse;
        }

        public void NullServerResponse()
        {
            serverResponse = null;
        }

        private void StartThread(string name)
        {
            Thread thread = new Thread(SendToServer);
            thread.Name = name;
            thread.Start();
        }

        public void Connect()
        {
            forms.Clear();
            StartThread("Connect Thread");
        }

        public void Login(string login, string pass)
        {
            forms.Clear();
            forms.Add("sID", (int)Services.LOGIN);

            StartThread("LoginThread");
        }

        private void SendToServer()
        {
            serverResponse = sendToServer.DoPost(forms);
        }
    }
}