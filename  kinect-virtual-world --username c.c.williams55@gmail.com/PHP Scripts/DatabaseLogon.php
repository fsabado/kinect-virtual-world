<?php

class DatabaseLogon {
	
	private $DBusername;
	private $DBpword;
	private $DBname;
	private $connectStatus;
	
	// initalize the database
	public function __construct($username, $pword, $dbname) 
	{
		$this->DBusername = $username;
		$this->DBpword = $pword;
		$this->DBname = $dbname;
	}
	
	// connect to SQL and the right database inform caller of connection status
	public function connectDB()
	{
		$this->connectStatus = mysql_connect('localhost', $this->DBusername, $this->DBpword);
		if(!$this->connectStatus)
			return false;
		$this->connectStatus = mysql_select_db($this->DBname);	
		if(!$this->connectStatus)
			return false;	
			
		return $this->connectStatus;
	}	
	
	// disconnect and inform caller of status
	public function disconnectDB()
	{
		return mysql_close($this->connectStatus);
	}
	
	// perform a get names of all the tables in the db
	public function getDBTables()
	{		
		$sql = "show tables from $this->DBname";	
		$result = mysql_query($sql);
		return $result;		
	}
	
	
}

?>