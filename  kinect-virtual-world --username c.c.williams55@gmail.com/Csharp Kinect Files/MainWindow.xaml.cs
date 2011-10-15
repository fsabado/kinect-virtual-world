using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
// from the kinect tuts supplies helper methods for kinect
using Coding4Fun.Kinect.Wpf;
using Microsoft.Research.Kinect.Audio;
using Microsoft.Research.Kinect.Nui;
// from the kinect tuts supplies helper methods for kinect 
using Coding4Fun.Kinect;
using System.Threading;



namespace WpfApplication_KinectProjectToPHP
{
    internal delegate void HttpReqDel();
    
    
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // the instance of the Kinect Runtime
        private Runtime nui = new Runtime();
        // used in testing of code loops
        private int i;       
        // instance of connecting to PHP server
        private AsyncHttp2Php asyncHttp2Php = new AsyncHttp2Php();
        // allow referencing of current skeleton data
        private SkeletonData skeleton;
        // used for async comms.
        private HttpReqDel httpDel;
        
        private bool setBool;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // establish a delegate for the following method(s)
            httpDel = new HttpReqDel(sendRequest);

            // start the kinect runtime.
            nui.Initialize(RuntimeOptions.UseSkeletalTracking);

            // from the kinect tuts 
            #region TransformSmooth
            //Must set to true and set after call to Initialize
            nui.SkeletonEngine.TransformSmooth = true;

            //Use to transform and reduce jitter
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };

            nui.SkeletonEngine.SmoothParameters = parameters;

            #endregion

            // add event to receive skeleton data
            nui.SkeletonFrameReady += this.nui_SkeletonFrameReady;
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            // get all the kinect dectected sleletons
            SkeletonFrame allSkeletons = e.SkeletonFrame;
            // get the first tracked skeleton with simple LINQ query
            this.skeleton = (from s in allSkeletons.Skeletons
                                     where s.TrackingState == SkeletonTrackingState.Tracked
                                     select s).FirstOrDefault();
            // check if the skeleton data is valid
            if(skeleton != null)
            {
                // set position
                SetEllipsePosition(headEllipse,skeleton.Joints[JointID.Head]);
                SetEllipsePosition(leftEllipse, skeleton.Joints[JointID.HandLeft]);
                SetEllipsePosition(rightEllipse,skeleton.Joints[JointID.HandRight]);
                SetEllipsePosition(rightWristEllipse,skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(leftWristEllipse, skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(leftElbowEllipse, skeleton.Joints[JointID.ElbowLeft]);
                SetEllipsePosition(rightElbowEllipse, skeleton.Joints[JointID.ElbowRight]);
                SetEllipsePosition(centerShoulderEllipse, skeleton.Joints[JointID.ShoulderCenter]);
                SetEllipsePosition(leftShoulderEllipse, skeleton.Joints[JointID.ShoulderLeft]);
                SetEllipsePosition(rightShoulderEllipse, skeleton.Joints[JointID.ShoulderRight]);
                SetEllipsePosition(spineEllipse, skeleton.Joints[JointID.Spine]);

                
                // after the kinect updates all positions call this gesture check.
                HandFaceGesture();


            }
        }

        // trivial gesture if hand is at face then send message to SL.
        private void HandFaceGesture()
        {
            float diffX = skeleton.Joints[JointID.Head].Position.X 
                        - skeleton.Joints[JointID.HandRight].Position.X;
            float sqrX = diffX * diffX;
            float diffy = skeleton.Joints[JointID.Head].Position.Y 
                        - skeleton.Joints[JointID.HandRight].Position.Y;
            float sqrY = diffy * diffy;
            
            this.headXvalue.Content = skeleton.Joints[JointID.Head].Position.X;
            this.headYvalue.Content = skeleton.Joints[JointID.Head].Position.Y;
            this.handXvalue.Content = skeleton.Joints[JointID.HandRight].Position.X;
            this.handYvalue.Content = skeleton.Joints[JointID.HandRight].Position.Y;

            double distHandHead = Math.Sqrt(sqrX+sqrY);

            // toggle the hand is at face gesture, but only once
            if (distHandHead > .1)
            {
                if (setBool == false)
                {
                    setBool = true;
                    this.countlabel.Content = i++;
                    this.triggerEllipse.Fill = new SolidColorBrush(Colors.Red);
                }
            }
            else
            {
                if (setBool)
                {
                    setBool = false;
                    this.countlabel.Content = i++;
                    this.triggerEllipse.Fill = new SolidColorBrush(Colors.Green);
                    // fire and forget delegate, hand is a face.
                    httpDel.BeginInvoke(null, null);
                }
            }            
        }

        // call the following methods when event triggered
        private void sendRequest()
        {
           // trigger condition met send message to prim.           
           this.asyncHttp2Php.SendRequest("changePrimColor", "Object");
        }

        // used to scale the wpf application, see kinect tuts
        private void SetEllipsePosition(FrameworkElement ellipse, Joint joint)
        {
            var scaledJoint = joint.ScaleTo(640, 480, .5f, .5f);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X);
            Canvas.SetTop(ellipse, scaledJoint.Position.Y);
        }


        // used to release the Kinect runtime
        private void Window_Closed(object sender, EventArgs e)
        {
            nui.Uninitialize();
        }
    }
}
