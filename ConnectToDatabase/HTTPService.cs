using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;

namespace ConnectToDatabase
{
    [Service]
    class HTTPService : Service
    {
        private MyBoundService connection;
        private HTTPRequests requests;

        public override void OnCreate()
        {
            base.OnCreate();
            requests = new HTTPRequests();
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            base.OnStartCommand(intent, flags, startId);
            return StartCommandResult.NotSticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            connection = new MyBoundService(this);
            return connection;
        }

        public override bool OnUnbind(Intent intent)
        {
            requests = null;
            return base.OnUnbind(intent);
        }

        public HTTPRequests Requests
        {
            get { return requests; }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            requests = null;
        }
    }
}