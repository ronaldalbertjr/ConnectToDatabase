using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Widget;
using Java.Net;
using Java.IO;

namespace ConnectToDatabase
{
    class HTTPSendToServer
    {
        URL url;
        HttpURLConnection conn;

        public bool Connect()
        {
            //string URL_STRING = "http://10.0.2.2:8080/calculator_server/requests.php"; //Localhost
            //string URL_STRING = "http://10.10.11.41/calculator_server/requests.php"; //Localhost
            string URL_STRING = "http://192.168.1.18/trabjamv/GetDataFromDatabase"; //Localhost

            try
            {
                url = new URL(URL_STRING);
                conn = (HttpURLConnection)url.OpenConnection();

                if (conn.ResponseCode == HttpStatus.Ok)
                {
                    Android.Util.Log.Debug("SendToServer", "Connection OK");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (IOException e)
            {
                Android.Util.Log.Debug("SendToServer", "Connection Problems" + e.Message);
                return false;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Disconnect();
                }
            }
        }

        public object DoPost(Dictionary<object, object> forms)
        {
            if (forms.Count > 0)
            {
                return Post(GetQueryString(forms));
            }
            else
            {
                return Connect();
            }
        }

        private Object Post(string parameters)
        {
            try
            {
                conn = (HttpURLConnection)url.OpenConnection();
                conn.RequestMethod = "POST";
                conn.DoOutput = true;
                conn.DoInput = true;
                conn.Connect();

                Android.Util.Log.Debug("SendToServer", "parametros " + parameters);

                byte[] bytes = Encoding.ASCII.GetBytes(parameters);

                OutputStream outputStream = new BufferedOutputStream(conn.OutputStream);
                outputStream.Write(bytes);
                outputStream.Flush();
                outputStream.Close();

                InputStream inputStream = new BufferedInputStream(conn.InputStream);

                return ReadString(inputStream);
            }
            catch (IOException e)
            {
                Android.Util.Log.Debug("SendToServer", "Problems to send data to server!! " + e.Message);
                return false;
            }
            finally
            {
                conn.Disconnect();
            }
        }

        private string ReadString(InputStream inputStream)
        {
            ByteArrayOutputStream baos = new ByteArrayOutputStream();

            try
            {
                byte[] buffer = new byte[1024];
                int len;

                while ((len = inputStream.Read(buffer)) > 0)
                {
                    baos.Write(buffer, 0, len);
                }

                baos.Close();
                inputStream.Close();

                return baos.ToString();
            }
            catch (IOException e)
            {
                Android.Util.Log.Debug("SendToServer", "Problems to translate the server answer in string!!" + e.Message);
            }

            return "";
        }

        private String GetQueryString(Dictionary<object, object> forms)
        {
            if (forms == null || forms.Count() == 0)
                return null;

            string urlParams = null;

            foreach (KeyValuePair<object, object> kvp in forms)
            {
                urlParams = urlParams == null ? "" : urlParams + "&";
                urlParams += kvp.Key.ToString() + "=" + kvp.Value.ToString();
            }

            return urlParams;
        }
    }
}