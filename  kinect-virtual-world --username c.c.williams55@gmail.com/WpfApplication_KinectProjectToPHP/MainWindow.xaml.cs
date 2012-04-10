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
using Microsoft.Speech.AudioFormat;
using Microsoft.Speech.Recognition;
using System.Diagnostics;
using System.IO;



namespace WpfApplication_KinectProjectToPHP
{    
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // used to find the spech Recognizer
        private const String RecognizerId = "SR_MS_en-US_Kinect_10.0";
        // the instance of the Kinect Runtime
        private Runtime _nui;
        // allow referencing of current skeleton data
        private SkeletonData _skeleton;       
        private KinectModelGame _kGame;
        private SpeechRecognitionEngine _sre;
        private KinectAudioSource _source;
  

        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {            
            // establish the kinect runtime
            _nui = new Runtime();            
            //create a new instance of the game
            this._kGame = new KinectModelGame(this);           
            // start the kinect runtime.
            _nui.Initialize(RuntimeOptions.UseSkeletalTracking);
            // from the kinect tuts 
            #region TransformSmooth
            //Must set to true and set after call to Initialize
            _nui.SkeletonEngine.TransformSmooth = true;

            //Use to transform and reduce jitter
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.75f,
                Correction = 0.0f,
                Prediction = 0.0f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };

            _nui.SkeletonEngine.SmoothParameters = parameters;

            #endregion
            // add event to receive skeleton data
            _nui.SkeletonFrameReady += this.nui_SkeletonFrameReady;
            SetupSpeech();
            //this.Hide();            
        }
        

        void sre_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Debug.WriteLine("Rejected");
        }

        void sre_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
        {
            Debug.WriteLine("Hypothesized: {0}  {1}", e.Result.Text, e.Result.Confidence);
        }

        /// <summary>
        /// Sets the command state of the program 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Text.ToString().ToUpper().Equals("EXIT"))
            {
                if (e.Result.Confidence < 0.96)
                    return;
                else
                    this.Close();
            }
            Debug.WriteLine("Recognized: {0} {1}", e.Result.Text, e.Result.Confidence);
            if (e.Result.Confidence < 0.94)
                return;
            else
                this._kGame.GameUserState(e.Result.Confidence, e.Result.Text);            
        }

        void nui_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            // get all the kinect detected skeletons
            SkeletonFrame allSkeletons = e.SkeletonFrame;
            // get the first tracked skeleton with simple LINQ query
            this._skeleton = (from s in allSkeletons.Skeletons
                             where s.TrackingState == SkeletonTrackingState.Tracked
                             select s).FirstOrDefault();
            // check if the skeleton data is valid
            if(_skeleton != null)
            {
                SetEllipsePosition(headEllipse, _skeleton.Joints[JointID.Head]);
                SetEllipsePosition(rightWristEllipse, _skeleton.Joints[JointID.WristRight]);
                SetEllipsePosition(leftWristEllipse, _skeleton.Joints[JointID.WristLeft]);
                SetEllipsePosition(centerShoulderEllipse, _skeleton.Joints[JointID.ShoulderCenter]);
                SetEllipsePosition(leftShoulderEllipse, _skeleton.Joints[JointID.ShoulderLeft]);
                SetEllipsePosition(rightShoulderEllipse, _skeleton.Joints[JointID.ShoulderRight]);
                SetEllipsePosition(spineEllipse, _skeleton.Joints[JointID.Spine]);
                 
                lWristXvalue.Content = _skeleton.Joints[JointID.WristLeft].Position.X;
                lWristYvalue.Content = _skeleton.Joints[JointID.WristLeft].Position.Y;
                lWristZvalue.Content = _skeleton.Joints[JointID.WristLeft].Position.Z;

                rWristXvalue.Content = _skeleton.Joints[JointID.WristRight].Position.X;
                rWristYvalue.Content = _skeleton.Joints[JointID.WristRight].Position.Y;
                rWristZvalue.Content = _skeleton.Joints[JointID.WristRight].Position.Z;

                SpineZvalue.Content = _skeleton.Joints[JointID.Spine].Position.Z;
                SpineYvalue.Content = _skeleton.Joints[JointID.Spine].Position.Y;

                HeadYvalue.Content = _skeleton.Joints[JointID.Head].Position.Y;
               


                this._kGame.CommandSLObject(_skeleton);
            }
        }

        /// <summary>
        /// Set up the Audio capture and recognition engine and start it.
        /// Includes event to signal to the game when a key command 
        /// state word was detected.
        /// </summary>
        public void SetupSpeech()
        {
                _source = new KinectAudioSource();
                _source.FeatureMode = true;
                _source.AutomaticGainControl = false;
                _source.SystemMode = SystemMode.OptibeamArrayOnly;

                RecognizerInfo ri = SpeechRecognitionEngine.InstalledRecognizers().Where(r => r.Id == RecognizerId).FirstOrDefault();

                if (ri == null)
                {
                    Debug.WriteLine("Could not find speech reconginzer: {0}. Please refer to the sample requiremnets.", RecognizerId.ToString());
                    return;
                }

                Debug.WriteLine("Using: {0}", ri.Name);

                _sre = new SpeechRecognitionEngine(ri.Id);


                var commands = new Choices();
                commands.Add("stop");
                commands.Add("exit");
                commands.Add("reset");
                commands.Add("next");
                commands.Add("previous");
                commands.Add("fire");

                var gb = new GrammarBuilder();
                gb.Culture = ri.Culture;
                gb.Append(commands);


                var g = new Grammar(gb);

                _sre.LoadGrammar(g);
                _sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
                _sre.SpeechHypothesized += new EventHandler<SpeechHypothesizedEventArgs>(sre_SpeechHypothesized);
                _sre.SpeechRecognitionRejected += new EventHandler<SpeechRecognitionRejectedEventArgs>(sre_SpeechRecognitionRejected);

                Stream s = _source.Start();
                _sre.SetInputToAudioStream(s, new SpeechAudioFormatInfo(EncodingFormat.Pcm, 16000, 16, 1, 32000, 2, null));

                _sre.RecognizeAsync(RecognizeMode.Multiple);   
        }
              
        
        // used to scale the wpf application, see kinect tuts
        private void SetEllipsePosition(FrameworkElement ellipse, Joint joint)
        {
            var scaledJoint = joint.ScaleTo(540, 380, .5f, .5f);

            Canvas.SetLeft(ellipse, scaledJoint.Position.X);
            Canvas.SetTop(ellipse, scaledJoint.Position.Y);
        }


        // used to release the Kinect runtime and audio resources
        private void Window_Closed(object sender, EventArgs e)
        {
            _source.Stop();
            _sre.RecognizeAsyncCancel();
            _sre.RecognizeAsyncStop();
            _source.Dispose();
            _nui.Uninitialize();
        }
    }
}
