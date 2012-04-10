using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Research.Kinect.Nui;
using Microsoft.Research.Kinect.
Audio;
using Microsoft.Xna.Framework;

namespace WpfApplication_KinectProjectToPHP
{




    /// <summary>
    /// Christian Williams 
    /// University of Arkansas 010537189
    /// ccw005@uark.edu
    /// 
    /// This class establishes a new game for a user including setup of objects
    /// controls the game flow and handles interpreting user intentions 
    /// based on the gestures from a user. 
    /// </summary>
    class KinectModelGame
    {
        /// <summary>
        /// The types of states that the game can be in. 
        /// </summary>
        enum GameCommandState
        {
            Stop, Exit, Reset, Interact,
        }


        public int _currentMap = 1;

        // location for the prims that the game will use to display to the user.
        private const String _phpUrl
            = "http://turing.csce.uark.edu/~ccw005/AngryPrimsServer.php";
        // user game piece
        private ModelPrim _userPrim;

        private KinectHttpRequest _gameRequest;
        // random prim http request object no threading issues
        private KinectHttpRequest _cannonReq;

        // win prim request url
        private KinectHttpRequest _winConReq;


        // the current game state global variable
        private GameCommandState GameState { get; set; }

        // wpf application widow reference 
        private MainWindow _mw;

        int cycleCount;
        private volatile int _Xaxis;
        private double _Yaxis;
        private double _Zaxis;
        private double _powerUpCannon;


        private int _RotationOffset = 0;
        private bool _ReverseCoordinates = false;



        /// <summary>
        /// Establish the game from the WPF window. 
        /// </summary>
        /// <param name="mw"></param>
        public KinectModelGame(MainWindow mw)
        {
            this._gameRequest = new KinectHttpRequest(_phpUrl);
            this._userPrim = new ModelPrim();
            this.GameState = GameCommandState.Stop;
            this._mw = mw;
            StartGame();
            this._cannonReq = new KinectHttpRequest(this._userPrim.PrimUrl);
        }

        /// <summary>
        /// Get the current game objects from the server and store them 
        /// for game communication.  
        /// </summary>
        public void StartGame()
        {
            Dictionary<String, String> primKeyValuePair;
            // get the list of prims and initialize the user/game prims

            primKeyValuePair = this._gameRequest.GetPrimListPHP();

            foreach (var key in primKeyValuePair.Keys)
            {
                if (key == "cannon")
                {
                    this._userPrim.PrimName = key.ToString();
                    this._userPrim.PrimUrl = primKeyValuePair[key];
                    Debug.WriteLine("User Prim Name and URL:   "
                        + this._userPrim.ToString());
                }
            }


        }

        /// <summary>
        /// Set the state of the User prim at these default starting values
        /// </summary>
        public void DefaultUserPrim()
        {
            // set the default state of the user prim

            /*
            ChangeColor(this._userPrim, 1.0, 1.0, 1.0);
            ChangeShape(this._userPrim, this._primShapes.ElementAt(0));
            ChangeSize(this._userPrim, 1.0, 1.0, 1.0);
            */


        }



        /// <summary>
        /// Used to reset the current map.
        /// </summary>
        public void ResetMap()
        {
            this._gameRequest.ChangeMap(_currentMap);
        }

        /// <summary>
        ///  This method controls the state of the game by way of detecting audio events
        /// </summary>
        public void GameUserState(double confidence, String recognizeText)
        {
            Debug.WriteLine("Recognized: {0} {1}", confidence, recognizeText);
            switch (recognizeText.ToUpper())
            {
                case "STOP":
                    this.GameState = GameCommandState.Stop;
                    Debug.WriteLine("CURRENT STATE: stop " + recognizeText.ToUpper());
                    break;
                case "RESET":
                    Debug.WriteLine("CURRENT STATE: reset " + recognizeText.ToUpper());
                    ResetMap();
                    this.GameState = GameCommandState.Stop;
                    break;
                case "NEXT":
                    Debug.WriteLine("CURRENT STATE: next " + recognizeText.ToUpper());
                    NextMap();
                    this.GameState = GameCommandState.Stop;
                    break;
                case "PREVIOUS":
                    Debug.WriteLine("CURRENT STATE: previous " + recognizeText.ToUpper());
                    PreviousMap();
                    this.GameState = GameCommandState.Stop;
                    break;
                case "FIRE":
                    Debug.WriteLine("CURRENT STATE: fire " + recognizeText.ToUpper());
                    FireCannon();
                    this.GameState = GameCommandState.Stop;
                    break;

                default:
                    break;
            }

        }

        /// <summary>
        /// Received fire command from user. Take the current parameters and send fire command. 
        /// </summary>
        private void FireCannon()
        {
            this._cannonReq.CannonControlSL(1, _Xaxis, _Yaxis, _Zaxis, _powerUpCannon);
            this.GameState = GameCommandState.Stop;
        }

        private void ControlCannon()
        {
            this._cannonReq.CannonControlSL(0, _Xaxis, _Yaxis, _Zaxis, _powerUpCannon);
        }
        /// <summary>
        /// Used to direct the Kinect data to correct gamestate action.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="kinectXYZ"></param>
        public void CommandSLObject(SkeletonData skeleton)
        {
            // always check to see if user is in position for cannon ops
            if (InpositionGesture(skeleton))
            {
                this.GameState = GameCommandState.Interact;
                Debug.WriteLine("INPOSITION AND READY TO INTERACT");
            }

            switch (this.GameState)
            {
                case GameCommandState.Interact:
                    InterpretGestureSize(skeleton);
                    break;
                default:
                    // no operation
                    break;
            }


            if (this.GameState.Equals(GameCommandState.Interact))
            {
                // check to see if the user is still intending to interact with 
                // SL cannon, if not then go to no operation state STOP. 
                if (!IsInteractingGesture(skeleton))
                {
                    this.GameState = GameCommandState.Stop;
                    Debug.WriteLine("OUT OF POSITION AND NOT READY!");
                }
            }
        }

        /// <summary>
        /// This method is used to check to see if the user
        /// is still in position and interacting with or intending
        /// to interact with the SL cannon. 
        /// </summary>
        /// <returns></returns>
        private bool IsInteractingGesture(SkeletonData skeleton)
        {
            if (skeleton.Joints[JointID.WristLeft].Position.Y >=
                skeleton.Joints[JointID.HipCenter].Position.Y
                && skeleton.Joints[JointID.WristRight].Position.Y >=
                skeleton.Joints[JointID.HipCenter].Position.Y)
                return true;

            return false;
        }

        /// <summary>
        /// This method is constantly checking to see if the user
        /// signals the intent to interact with the SL cannon
        /// so that commands start being sent to SL.
        /// </summary>
        /// <param name="skeleton"></param>
        /// <returns></returns>
        private bool InpositionGesture(SkeletonData skeleton)
        {
            if (skeleton.Joints[JointID.WristLeft].Position.Y >=
                skeleton.Joints[JointID.Head].Position.Y
                && skeleton.Joints[JointID.WristRight].Position.Y >=
                skeleton.Joints[JointID.Head].Position.Y)
                return true;

            return false;
        }



        /// <summary>
        /// Used to determine what the user is intending as far as selecting the size of the object.
        /// X and Y plane increases are determined by looking at which ever plane has a difference > 0.
        /// This means that the user would like the object to increase or decrease in size along the axis
        /// where the difference between the two points(hands) is > 0. 
        /// </summary>
        /// <param name="skeleton"></param>
        public void InterpretGestureSize(SkeletonData skeleton)
        {
            // used to slow down the number of times that the program cycles through this method and 
            // updates SL. Due to a restriction in communicating with SL. 
            if (!CycleCountCheck())
                return;


            PowerLevelGesture(skeleton);
            //LeftRightGesture(skeleton);
            // UpDownGesture(skeleton);
            ControlCannon();

        }

        public int RotationOffset
        {
            get { return _RotationOffset; }
            set
            {
                // Use the modulo in case the rotation value specified exceeds
                // 360.
                _RotationOffset = value % 360;
            }
        }

        public bool ReverseCoordinates
        {
            get { return _ReverseCoordinates; }
            set { _ReverseCoordinates = value; }
        }

        public double CalculateReverseCoordinates(double degrees)
        {
            return (-degrees + 180) % 360;
        }


        //Adjust power level based on angle
        private void PowerLevelGesture(SkeletonData skeleton)
        {

            Joint joint1, joint2, joint3;
            joint1 = skeleton.Joints[JointID.WristLeft];
            joint2 = skeleton.Joints[JointID.ShoulderLeft];
            joint3 = skeleton.Joints[JointID.HipLeft];


            Vector3 vectorJoint1ToJoint2 = new Vector3(joint1.Position.X - joint2.Position.X, joint1.Position.Z - joint2.Position.Z, 0);
            Vector3 vectorJoint2ToJoint3 = new Vector3(joint2.Position.X - joint3.Position.X, joint2.Position.Z - joint3.Position.Z, 0);


            Vector3 crossProduct = Vector3.Cross(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            double crossProductLength = crossProduct.Z;
            double dotProduct = Vector3.Dot(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            double segmentAngle = Math.Atan2(crossProductLength, dotProduct);
            vectorJoint1ToJoint2.Normalize();
            vectorJoint2ToJoint3.Normalize();

            // Convert the result to degrees.
            double degrees = segmentAngle * (180 / Math.PI);

            // Add the angular offset.  Use modulo 360 to convert the value calculated above to a range
            // from 0 to 360.
            degrees = (degrees + _RotationOffset) % 360;



            //Calculate whether the coordinates should be reversed to account for different sides 
            if (_ReverseCoordinates)
            {

                degrees = CalculateReverseCoordinates(degrees);

            }



            this._powerUpCannon = degrees;
            return;
        }





        private void LeftRightGesture(SkeletonData skeleton)
        {

            Joint joint1, joint2, joint3;
            joint1 = skeleton.Joints[JointID.WristRight];
            joint2 = skeleton.Joints[JointID.ShoulderRight];
            joint3 = skeleton.Joints[JointID.HipLeft];


            Vector3 vectorJoint1ToJoint2 = new Vector3(joint1.Position.X - joint2.Position.X, joint1.Position.Z - joint2.Position.Z, 0);
            Vector3 vectorJoint2ToJoint3 = new Vector3(joint2.Position.X - joint3.Position.X, joint2.Position.Z - joint3.Position.Z, 0);
            vectorJoint1ToJoint2.Normalize();
            vectorJoint2ToJoint3.Normalize();

            Vector3 crossProduct = Vector3.Cross(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            double crossProductLength = crossProduct.Z;
            double dotProduct = Vector3.Dot(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            double segmentAngle = Math.Atan2(crossProductLength, dotProduct);


            // Convert the result to degrees.
            double degrees = segmentAngle * (180 / Math.PI);

            // Add the angular offset.  Use modulo 360 to convert the value calculated above to a range
            // from 0 to 360.
            degrees = (degrees + _RotationOffset) % 360;



            //Calculate whether the coordinates should be reversed to account for different sides 
            if (_ReverseCoordinates)
            {

                degrees = CalculateReverseCoordinates(degrees);

            }



            this._Zaxis = degrees;
            return;

        }

        private void UpDownGesture(SkeletonData skeleton)
        {

            Joint joint1, joint2, joint3;
            joint1 = skeleton.Joints[JointID.HandRight];
            joint2 = skeleton.Joints[JointID.ShoulderRight];
            joint3 = skeleton.Joints[JointID.HipRight];


            Vector3 vectorJoint1ToJoint2 = new Vector3(joint1.Position.X - joint2.Position.X, joint1.Position.Y - joint2.Position.Y, 0);
            Vector3 vectorJoint2ToJoint3 = new Vector3(joint2.Position.X - joint3.Position.X, joint2.Position.Y - joint3.Position.Y, 0);
            vectorJoint1ToJoint2.Normalize();
            vectorJoint2ToJoint3.Normalize();


            Vector3 crossProduct = Vector3.Cross(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            double crossProductLength = crossProduct.Z;
            double dotProduct = Vector3.Dot(vectorJoint1ToJoint2, vectorJoint2ToJoint3);
            double segmentAngle = Math.Atan2(crossProductLength, dotProduct);




            //// Convert the result to degrees.
            double degrees = segmentAngle * (180 / Math.PI);

            //// Add the angular offset.  Use modulo 360 to convert the value calculated above to a range
            //// from 0 to 360.

            degrees = (degrees + _RotationOffset) % 360;



            //// Calculate whether the coordinates should be reversed to account for different sides 

            if (_ReverseCoordinates)
            {

                degrees = CalculateReverseCoordinates(degrees);

            }



            this._Yaxis = degrees;
            return;

        }









        /// <summary>
        /// Helper method to determine which axis has the greater difference.
        /// Thus the user is indicting the desire to change this axis. 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public String MaxOfXYZ(double x, double y, double z)
        {
            if (x > y && x > z)
            {
                return "x";
            }
            if (y > x && y > z)
            {
                return "y";
            }
            if (z > x && z > y)
            {
                return "z";
            }

            return "";
        }

        /// <summary>
        /// Helper method to determine dead zone of the kinect. 
        /// If the users hands are greater than this amount or closer than
        /// this other amount then movement, else do nothing. 
        /// </summary>
        /// <param name="diffrence"></param>
        /// <returns></returns>
        //public String DecideSize(double diffrence)
        //{
        //    if (diffrence < .25)
        //    {
        //        return "down";
        //    }
        //    if (diffrence > .35)
        //    {
        //        return "up";
        //    }

        //    return "none";
        //}

        /// <summary>
        /// Helper method used to reduce the perceived checking of this gesture by the 
        /// kinect runtime. Runtime is normally 30fps so cycle count to 30 sets the check at 1/sec
        /// </summary>
        /// <returns></returns>
        public bool CycleCountCheck()
        {
            this.cycleCount++;
            if (this.cycleCount > 40)
            {
                cycleCount = 0;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Cycle forward through the maps. 
        /// </summary>
        private void NextMap()
        {
            if (_currentMap <= 2)
            {
                ++_currentMap;
                // send request to server
                this._gameRequest.ChangeMap(_currentMap);
            }
            else
            {
                _currentMap = 1;
                // send request to server
                this._gameRequest.ChangeMap(_currentMap);
            }
        }

        /// <summary>
        /// Cycle through the available maps backwards. 
        /// </summary>
        private void PreviousMap()
        {
            if (_currentMap >= 2)
            {
                --_currentMap;
                // send request to server
                this._gameRequest.ChangeMap(_currentMap);
            }
            else
            {
                _currentMap = 3;
                // send request to server
                this._gameRequest.ChangeMap(_currentMap);
            }
        }
    }
}
