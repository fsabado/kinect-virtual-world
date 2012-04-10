using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;

namespace WpfApplication_KinectProjectToPHP
{
    class HttpWebRequestBeginGetRequest
    {
        private static ManualResetEvent allDone = new ManualResetEvent(false);

        public String HttpRequest { get; set; }

        public void SendRequest()
        {
            // Create a new HttpWebRequest object.
            var request = (HttpWebRequest)
                WebRequest.Create( "http://turing.csce.uark.edu/~ccw005/SecondLifeServer.php");

            request.ContentType = "application/x-www-form-urlencoded";

            // Set the Method property to 'POST' to post data to the URI.
            request.Method = "POST";

            // start the asynchronous operation
            request.BeginGetRequestStream(GetRequestStreamCallback, request);

            // Keep the main thread from continuing while the asynchronous
            // operation completes. A real world application
            // could do something useful such as updating its user interface. 
            allDone.WaitOne();
        }

        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            Stream postStream = request.EndGetRequestStream(asynchronousResult);

            // the input data to be posted
            string postData = "httpRequest=" + "changePrimColor" + "&primName=" + "Object";

            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write to the request stream.
            postStream.Write(byteArray, 0, postData.Length);
            postStream.Close();

            // Start the asynchronous operation to get the response
            request.BeginGetResponse(GetResponseCallback, request);
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            var request = (HttpWebRequest)asynchronousResult.AsyncState;

            // End the operation
            var response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
            Stream streamResponse = response.GetResponseStream();
            if (streamResponse != null)
            {
                var streamRead = new StreamReader(streamResponse);
                string responseString = streamRead.ReadToEnd();
                Console.WriteLine(responseString);
                // Close the stream object
                streamResponse.Close();
                streamRead.Close();
            }

            // Release the HttpWebResponse
            response.Close();
            allDone.Set();
        }

    }
}
