<html><?php

// Christian Williams UarkId 010537189 ccw005@uark.edu
// This is a android to second life interface script 
// that allows for a android device to control a second life
// prim.

// This script is also used by the AngryPrimsCannon Object ONLY.

include 'DatabaseLogon.php';

$username="ccw005";
$dbpassword="aoR2aix0";
$database="ccw005";

$httpRequestString = $_POST['httpRequest'];
$primSelectedString = $_POST['primSelected'];
$outputMessageString = $_POST['outputMessage'];
$primURL = $_POST['primUrl'];
$primName = $_POST['primName'];

//////////////////////// user defined functions
// get a list from the db of prim names
function getPrimList() 
{
	$Query = "select PrimName from SecondLifePrims";
	$Result = mysql_query($Query);	
	while($rows = mysql_fetch_assoc($Result))
	{		
		echo " ".$rows['PrimName'];
	}
	
}

// get the prim url by prim name 
function getPrimUrl($primName) 
{
	// user exists ? return prim url 
	$Query = "select PrimUrl from SecondLifePrims where PrimName = '$primName' ";
	$Result = mysql_query($Query);
	if(!$Result)
		echo "Item not found.";
	$Result = mysql_fetch_assoc($Result);			
	return $Result['PrimUrl'];	
}

// set the prim url and name
function registerUrl($primName, $primUrl)
{
	// user does not exist so enter the prim and url 
	$Insert = "insert into SecondLifePrims values ('$primName', '$primUrl')";
	$Result = mysql_query($Insert);
	if($Result)		
		echo "Object and Url Registered.";
	else 
		echo "Register Object Failed.";
}

// remove the url by prim name
function removeUrl($primName)
{	
	// the user exist so delete	
	$Delete = "delete from SecondLifePrims where PrimName = '$primName' ";
	$Result = mysql_query($Delete);
	if($Result)		
		echo "Object and Url Removed";
	else 
		echo "Remove Object Url Failed";	
}

// not used for now... some thing wrong...
function valueExists($tableName, $columnName, $value)
{
	$valueExistsQuery = "Select * from '$tableName' where '$columnName' = '$value' ";
	if(mysql_num_rows(mysql_query($valueExistsQuery)))
	{
		return TRUE;
	} 
	else 
	{
		return FALSE;
	}
}

// This function was taken from the following source it generates a post request.
// http://wezfurlong.org/blog/2006/nov/http-post-from-php-without-curl/
function do_post_request($url, $data, $optional_headers = null)
{
  $params = array('http' => array(
              'method' => 'POST',
              'content' => $data
            ));
  if ($optional_headers !== null) {
    $params['http']['header'] = $optional_headers;
  }
  $ctx = stream_context_create($params);
  $fp = @fopen($url, 'rb', false, $ctx);
  if (!$fp) {
    throw new Exception("Problem with $url, $php_errormsg");
  }
  $response = @stream_get_contents($fp);
  if ($response === false) {
    throw new Exception("Problem reading data from $url, $php_errormsg");
  }
  return $response;
}

/////////////////////// End user functions

// get a object from my generic class for accessing db
$db = new DatabaseLogon($username, $dbpassword, $database);

// try to connect to the data base 
if($db->connectDB())
{	
	
	//////////   Interfacing with android
	if($httpRequestString == "getPrimList")
	{	
		// return the list as a whitespace delimited string
		getPrimList();
	}	
	
	if($httpRequestString == "changePrimColor")
	{		
		// get the url from the db by name
		$url = getPrimUrl($primName);
		// post to second life prim by the url
		do_post_request($url,"Color");
		echo "Change Color";
	}	
	
	if($httpRequestString == "changePrimOutput")
	{		
		// get the url from the db by name
		$url = getPrimUrl($primName);
		// post to second life prim by the url
		do_post_request($url,$outputMessageString);
		echo $primName." Change output ".$outputMessageString;		
	}	
		
	
	//////// Interfacing with second life
	if($httpRequestString == "registerPrim")
	{
		registerUrl($primName, $primURL);
	}
	
	if($httpRequestString == "removePrim")
	{		
		removeUrl($primName);
	}
	
	if($httpRequestString == "getPrimUrl")
	{
		echo getPrimUrl($primName);
	}	
// done with the db



$db->disconnectDB();
}	
else 
	echo "Could not connect to server.";

?></html>