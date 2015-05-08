//-----------------------------------------------------------------------------
// Little functions to be used throughout the project like math and common groups
//-----------------------------------------------------------------------------

function Utility::create( %this )
{
	echo("load utility");
	exec("./scripts/Healthbar.cs");
}
    
//-----------------------------------------------------------------------------

function Utility::destroy( %this )
{
}

//-----------------------------------------------------------------------------

function getAngle(%objectA, %objectB)
{
	%angle = Vector2AngleToPoint (%objectA.getPosition(), %objectB.getPosition());
	return %angle;
}

//-----------------------------------------------------------------------------

function Utility::getPivotReposition(%this, %offsetLength, %angle)
{
	%rad = mDegToRad(%angle);
	return mCos(%rad)*%offsetLength SPC mSin(%rad)*%offsetLength;
}

//-----------------------------------------------------------------------------
//collision test: if(%object.getSceneGroup() == Utility.getCollisionGroup(""))

function Utility::getCollisionGroup(%this, %groupName)
{
	%groupNumber = 0;
	
	if(%groupName $= "Player")
	{
		%groupNumber = 5;
	}
	else if(%groupName $= "PlayerAttacks")
	{
		%groupNumber = 6;
	}
	else if(%groupName $= "PlayerBlock")
	{
		%groupNumber = 7;
	}
	else if(%groupName $= "Pickups")
	{
		%groupNumber = 9;
	}
	else if(%groupName $= "Enemies")
	{
		%groupNumber = 10;
	}
	else if(%groupName $= "EnemyAttacks")
	{
		%groupNumber = 11;
	}
	else if(%groupName $= "Wall")
	{
		%groupNumber = 15;
	}
	
	
	return %groupNumber;
}
