using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;


namespace WpfApplication_KinectProjectToPHP
{
    
    // this is a basic http request note how some of the info is hard coated.
    class AsyncHttp2Php
    {
        private const String Url = "http://turing.csce.uark.edu/~ccw005/SecondLifeServer.php"; 

        public void SendRequest(String primCommand, String primName)
        {
             
            // start the url creation process
            var _request = (HttpWebRequest)WebRequest.Create(Url);
            _request.Method = "POST";
            _request.ContentType = "UTF-8";
            // specific to the php code on the recieveing end.
            String postString = "httpRequest=" + primCommand + "&primName=" + primName;
            _request.ContentType = "application/x-www-form-urlencoded";
            _request.ContentLength = postString.Length;
            // set up connection to php server
            var stOut = new StreamWriter(_request.GetRequestStream(), Encoding.ASCII);
            // write to the web
            stOut.Write(postString);
            // close connection
            stOut.Close();

            // get ready to receive a response
            var response = (HttpWebResponse)_request.GetResponse();
            
            // Get the stream associated with the response.
            var receiveStream = response.GetResponseStream(); //this is where you have to get the response from the php script

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);
            
            // place the response in to this string
            String _output = readStream.ReadToEnd();
            

            // close the connection
            response.Close();
            readStream.Close();
        }
    }
}
