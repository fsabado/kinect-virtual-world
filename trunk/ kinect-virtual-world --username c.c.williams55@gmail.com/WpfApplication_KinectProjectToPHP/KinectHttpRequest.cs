using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Diagnostics;


namespace WpfApplication_KinectProjectToPHP
{

    /// <summary>
    /// Used to handle async calls to SecondLife objects and php server.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    internal delegate String CommSlPhpDel(String command);

    /// <summary>
    /// Christian Williams 010537189 ccw005@uark.edu
    /// 
    /// This class is used to represent the network connection of a game.
    /// Note the use of a fire and forget delegate for the Http requests.
    /// GetPrimList is meant to query a php server the other methods query SL.
    /// 
    /// </summary>
    class KinectHttpRequest
    {
        private const String PHPURL
            = "http://turing.csce.uark.edu/~ccw005/AngryPrimsServer.php";

        /// <summary>
        /// Used for comms so that other prims can use the
        /// same object to contact the correct SL location.
        /// </summary>
        public String Url { get; set; }

        public CommSlPhpDel _commSlPhpDel;

        /// <summary>
        /// set the prop to something when class is first used
        /// </summary>
        /// <param name="url"></param> 
        public KinectHttpRequest(String url)
        {
            this.Url = url;
            this._commSlPhpDel = SendRequest;
        }

        /// <summary>
        /// Get the available prims for the game, from php server.
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, String> GetPrimListPHP()
        {
            this.Url = PHPURL;
            // specific to the SL code on the receiving end.
            String postString = "httpRequest=" + "getPrimList";
            String responce = SendRequest(postString);

            Debug.WriteLine("RESPONCE:    " + responce);
            //separate the data out from the <html> tags.
            String[] responceArray = responce.Split('|');
            // separate the data into object+url format
            String[] nameUrlArray = responceArray[1].Split(' ');
            // split the arrays by "+" into object url and insert into dictionary
            var dictionary = new Dictionary<String,String>();
            foreach (var item in nameUrlArray)
            {
                String[] temp = item.Split('+');
                if(temp.Length > 1)
                    dictionary.Add(temp[0], temp[1]);
            }
            return dictionary;
        }


        public void ChangeMap(int mapNumber)
        {
            // specific to the SL code on the receiving end.
            String postString = "httpRequest=" + "Map" + "&number=" + mapNumber;
            this._commSlPhpDel.BeginInvoke(postString, null, null);
        }


        /// <summary>
        /// Command the prim to change color.
        /// </summary>
        public void ChangeColorSL( double red, double green, double blue )
        {
            // specific to the SL code on the receiving end.
            String postString = "httpRequest=" + "color" + "&red=" + red
                +"&green=" +green + "&blue=" +blue;
            this._commSlPhpDel.BeginInvoke(postString, null, null);
        }

        /// <summary>
        /// Command the prim to change size.
        /// </summary>
        public void ChangeSizeSL(double x, double y, double z)
        {
            // specific to the SL code on the receiving end.
            String postString = "httpRequest=" + "size" + "&x=" + x
                + "&y=" + y + "&z=" + z;
            this._commSlPhpDel.BeginInvoke(postString, null, null);
        }
        /// <summary>
        /// Command the prim to change shape
        /// </summary>
        /// <param name="shape"></param>
        public void ChangeShapeSL(String shape)
        { 
            String postString = "httpRequest=" + "type" + "&type=" + shape;
            this._commSlPhpDel.BeginInvoke(postString, null, null);
        }

        /// <summary>
        /// Command the prim to change rotation.
        /// </summary>
        public void ChangeRotationSL(double x, double y, double z)
        {
            // specific to the SL code on the receiving end.
            String postString = "httpRequest=" + "rotation" + "&x=" + x
                + "&y=" + y + "&z=" + z;
            this._commSlPhpDel.BeginInvoke(postString, null, null);
        }

        public void CannonControlSL(int fire, double X, double Y, double Z, double powerUp)
        {
            // specific to the SL code on the receiving end.
            String postString = "code=comm1" + "&fire=" + fire
                                + "&x=" + X + "&y=" + Y
                                + "&z=" + Z
                                + "&power=" + powerUp;
            this._commSlPhpDel.BeginInvoke(postString, null, null);
        }
        /// <summary>
        /// used to simulate network comms of SendRequestMethod
        /// and display them to debug console.
        /// </summary>
        /// <param name="requestString"></param>
        /// <returns></returns>
        public String DebugRequest(String requestString)
        {
            
            Debug.WriteLine(requestString + "    " + this.Url);
            return "somereturnvalue";
        }

        /// <summary>
        /// actual method that conducts all network operations to SL or PHP
        /// </summary>
        /// <param name="requestString"></param>
        /// <returns></returns>
        public String SendRequest(String requestString)
        {
            // start the url creation process            
            var request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "UTF-8";            
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = requestString.Length;
            // set up connection to php server
            var stOut = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
            // write to the web
            stOut.Write(requestString);
            // close connection
            stOut.Close();
            // get ready to receive a response
            var response = (HttpWebResponse)request.GetResponse();            
            // Get the stream associated with the response.
            //this is where you have to get the response from the resource
            var receiveStream = response.GetResponseStream(); 
            // Pipes the stream to a higher level stream 
            // reader with the required encoding format. 
            var readStream = new StreamReader(receiveStream, Encoding.UTF8);            
            // place the response in to this string
            String responseString = readStream.ReadToEnd();

            Debug.WriteLine("RECIEVED RESPONCE:      " + responseString);

            // close the connection
            response.Close();
            readStream.Close();
            // return the responce to the calling method.
            return responseString;
        }
    }
}
