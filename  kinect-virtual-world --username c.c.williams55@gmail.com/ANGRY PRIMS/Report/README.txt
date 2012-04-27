This is all the code for the AngryPrims project. 
There are 4 parts: Kinect Client, Server Scripts, Server MySQL DB, Second Life Scripts.

//////////////////////////// Kinect Clinet: 
The client needs to be built and run using the kinect beta 2 version as during the time of 
development Microsoft was still working on the final version of the kinect SDK. 
The appplication is based on a WPF project and consists of the following files:
To build the client 
1) install the beta 2 version of Kinect SDK 
2) Create new WPF application
3) add these files to project and set the required referances to the needed .dlls

Discriptions for the class functions and purposes are given in the class code comments.  
MainWindow.xaml.cs    
MainWindow.xaml
KinectModelGame.cs
KinectHttpRequest.cs



///////////////////////////// Server Scripts:
The server scripts, all of them, need to be placed in the public_html folder of the user directory on the turing server.
The user must have access to a MySql Database on turing. 
The scripts must have execute privilages. 
Discriptions for the functions and purposes are given in the code comments but, the point of
the scripts are to communicate to 1) the kinect client 2) the objects in SL

AngryPrimsServer.php
DatabaseLogon.php
SecondLifeServer.php



////////////////////////////// Server MySQL DB
The database must be set up with the correct names and values:
Run the following text files and create the needed tables. 
The txt files have the table creation statements and values to make a map.
MySqlCapture.PNG      //shows the correct server state required. 
Map0.txt
Map1.txt
Map2.txt
Map3.txt



/////////////////////////////// Second Life Scripts:
This part consists of 3 parts: Map object pool scripts, Cannon and Bullet Scripts, ScoreTracker and HUD objects

////////// Map object pool scripts
These scripts are placed in the objects that will be map objects
1) create a pool of about 20 objects in SecondLife and create 2 new scripts and copy the code from
the following txt files in to the scripts and then place both scripts in all pool objects.
Read code comments to understand script purpose. 

AngryPrimsCollisionBulletFinal.txt
AngryPrimsPhyCollv2Final.txt


////////// ScoreTracker and HUD
The score tracker and HUD objects work together to display game information to the user. 
1) create a object near the map and cannon and create a script and copy the code from the following file
in to it and place the script into the object. There is only one of this objects per game area. 

AngryPrimsScoreTrackerFinal.txt

2)Create the HUD objects and place the following scripts into them, then ware them as the user avatar.
HUD 1.
//Score Text
initSetup.txt
scoreTextCommands.txt
XyText.txt


HUD 2.
//Score
initSetup.txt
scoreCommands.txt
XyText.txt


HUD 3.
//PowerLevel
initSetup.txt
powerIndicator.txt


HUD 4.
//GameMessage
gameMessage.txt
initSetup.txt
XyText.txt


///////// Cannon and Bullet Scripts

cannon.txt
commands.txt
movement_custom.txt
movement_inc.txt
serverComm.txt
Documentation.docx

Ball.txt

Sensor gravity.txt
reset.txt
push.txt
look(Boccaccio) 2.txt
listen die 2.txt
gravity.txt

bullet.txt
