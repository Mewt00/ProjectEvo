//-----------------------------------------------------------------------------
// EnemyUnit: EnemyUnit class and functions
// Note: Tool/body construction is mostly handled in neighboring class EnemyUnitToolConfig.cs
//-----------------------------------------------------------------------------

function EnemyUnit::onAdd( %this )
{
	%this.myArena.EnemyCount++;
}

//-----------------------------------------------------------------------------
///ordering: armor/parry/acid/tar/blade/shooter/blob

function EnemyUnit::initialize(%this)
{
	exec("./EnemyUnitToolConfig.cs");		//separate file for tool config code
	//--Execute Node scripts-----------------------
	exec("./Tools/ToolNode.cs");
	exec("./Tools/Armor/ToolArmor.cs");
	exec("./Tools/Parry/ToolParry.cs");
	exec("./Tools/Acid/ToolAcid.cs");
	exec("./Tools/Tar/ToolTar.cs");
	exec("./Tools/Blade/ToolBlade.cs");
	exec("./Tools/Shooter/ToolShooter.cs");
	
    %this.setUpdateCallback(true);

	//-Stats---
	%this.fullHealth = 100;
	%this.health = %this.fullHealth;
	//%this.walkSpeed = 15;
	%this.walkSpeed = 15;
	%this.chaseSpeed = 0;
	//%this.turnSpeed = 60;
	%this.turnSpeed = 0;
	%this.sideSpeed = 0;
	%this.forwardSpeed = 0;
	%this.blinkFrequency = 0;
	
	%this.tempLinearVelocityX = 0;
	%this.tempLinearVelocityY = 0;
	
	%this.armorValue = 0;
	%this.parryChance = 0;
	
	//-DamageTracking---
	%this.shooterDamage = 0;
	%this.shooterShotsFired = 0;
	%this.bladeDamage = 0;
	%this.bladeAttackNums = 0;
	
	//-Info---
	%this.setSceneGroup(Utility.getCollisionGroup("Enemies"));		//Enemy Unit sceneGroup
	%targetRotation = Vector2AngleToPoint (%this.getPosition(), %this.mainTarget.getPosition());
	%this.setAngle(%targetRotation);
	
	%this.sizeRatio = $pixelsToWorldUnits;
	
	%this.behaviorNames = "";
	%this.moveBehaviorCount = 0;
	%this.specialX = 0;
	%this.specialY = 0;

	echo("inside enemy");
	
	//if(%this.noBehaviors != 1)  //?
	//{
		//%this.setAngle(getRandom(360));
	%this.myPausees = new SimSet();
	%this.myPausers = new SimSet();
	echo("above setup behaviors");
	%this.setupBehaviors();
	//}
	
	%this.setSceneLayer(10);
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") SPC Utility.getCollisionGroup("Wall")  SPC Utility.getCollisionGroup("Enemies") );
	%this.setCollisionCallback(true);
			
	//Parse local chromosome and build body
	%this.maxBodySize = 0;								//largest (abs) indices for myBody position
	%this.configureTools(%this.myChromosome);		// ordering: shield/parry/acid/tar/blade/shooter/blob (+1)
	
	//TODO add healthbar to the enemy compositesprite
	%this.myHealthbar = new CompositeSprite()
	{
		class = "Healthbar";
		owner = %this;
		xOffset = 0;
		yOffset = (%this.maxBodySize + 1)*%this.myBodyContainer.getObject(0).myHeight;
	};

  %this.getScene().add( %this.myHealthbar );
}

//-----------------------------------------------------------------------------

function EnemyUnit::setupBehaviors( %this )
{
	echo("inside setup behaviors");
	exec("./behaviors/ai/faceObject.cs");
	exec("./behaviors/movement/chase.cs");
	exec("./behaviors/movement/zigzag.cs");
	exec("./behaviors/movement/lunge.cs");
	exec("./behaviors/movement/strafe.cs");
	exec("./behaviors/movement/blink.cs");
	
	//List of behaviors to attach
	
	//%this.getBehaviorTypes();
	%this.behaviorNames = "BlinkBehavior 1";
	
	for(%i = 0; %i < getWordCount(%this.behaviorNames); %i += 2)
	{
		echo("1");
		%individualBehavior = getWord(%this.behaviorNames, %i).createInstance();
		%individualBehavior.target = %this.mainTarget;
		%individualBehavior.number = getWord(%this.behaviorNames, %i + 1);
		%this.addBehavior(%individualBehavior);
		echo("2");
		/*for(%j = 0; %j < getWordCount(%this.myArena.pausers); %j++)
		{
			echo("3");
			if(getWord(%this.behaviorNames, %i) $= getWord(%this.myArena.pausers, %j))
				%this.myPausers.add(%individualBehavior);
		}
		for(%j = 0; %j < getWordCount(%this.myArena.pausees); %j++)
		{
			echo("4");
			if(getWord(%this.behaviorNames, %i) $= getWord(%this.myArena.pausees, %j))
				%this.myPausees.add(%individualBehavior);
		}*/
		
		echo("name :   " @ getWord(%this.behaviorNames, %i) @ getWord(%this.behaviorNames, %i + 1));
		
	}
	
	/*if(getWordCount(%this.myPausers) > 0 && getWordCount(%this.myPausees) > 0)
		for(%i = 0; %i < getWordCount(%this.myPausers); %i++)
		{
			for(%j = 0; %j < getWordCount(%this.myPausees); %j++)
			{
				echo("5");
				//%this.Connect(%this.myPausers.getObject(%i), %this.myPausees.getObject(%j), pauseForZigZag, beingPaused);
				//%this.Connect(%this.myPausers.getObject(%i), %this.myPausees.getObject(%j), unpauseForZigZag, beingUnpaused);
			}
		}*/
	echo("forwardSpeed  " @ %this.forwardSpeed);
	/*%chaseObj = getWord(%behaviorNames, 0).createInstance();
	%chaseObj.target = %this.mainTarget;
	%this.addBehavior(%chaseObj);
	
	%zigZagObj = getWord(%behaviorNames, 2).createInstance();
	%zigZagObj.target = %this.mainTarget;
	%this.addBehavior(%zigZagObj);
	
	%this.Connect(%zigZagObj, %chaseObj, pauseForZigZag, beingPaused);
	%this.Connect(%zigZagObj, %chaseObj, unpauseForZigZag, beingUnpaused);
	//%this.Raise(%zigZagObj, pauseForZigZag);
	
	%faceObj = getWord(%behaviorNames, 1).createInstance();
	%faceObj.target = %this.mainTarget;
	%this.addBehavior(%faceObj);
	*/
	/*
	%driftMove = DriftBehavior.createInstance();
	%driftMove.speed = %this.walkSpeed;
	%this.addBehavior(%driftMove);
	
	%wanderMove = getWord(%behaviorNames, 0).createInstance();
	%wanderMove.turnDelay = 1;
	%wanderMove.numDires = 8;
	%wanderMove.moveSpeed = %this.walkSpeed;
	%wanderMove.turnSpeed = %this.turnSpeed;
	%this.addBehavior(%wanderMove);
	
	//%minDistance = MinDistanceBehavior.createInstance();
	//%this.addBehavior(%minDistance);
	
	//%maxDistance = MaxDistanceBehavior.createInstance();
	//%this.addBehavior(%maxDistance);
	
	%strafe = getWord(%behaviorNames, 2).createInstance();
	%this.addBehavior(%strafe);*/
}

//-----------------------------------------------------------------------------

//function EnemyUnit::getBehaviorTypes( %this )
//{
	
//}

//-----------------------------------------------------------------------------

function EnemyUnit::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%object.hit(5, %this);			//Hit player for 5 on contact
	}
}

//-----------------------------------------------------------------------------
//Move at walkSpeed

function EnemyUnit::onUpdate( %this )
{
	echo("enemy: " @ %this.getId());
	echo("enemy speed is " @ %this.tempLinearVelocityX @ " by " @ %this.tempLinearVelocityY);
	echo("");
	//%this.setLinearVelocityX(0);
	//%this.setLinearVelocityY(0);
	%this.setLinearVelocityX(%this.tempLinearVelocityX);
	%this.setLinearVelocityY(%this.tempLinearVelocityY);
	%this.tempLinearVelocityX = 0;
	%this.tempLinearVelocityY = 0;
}
//-----------------------------------------------------------------------------

function EnemyUnit::takeDamage( %this, %dmgAmount, %dmgType )
{
	if(%dmgType $= "Ranged")			//check armor
	{
		%dmgAmount -= %this.armorValue;
		
		if(%dmgAmount < 0)
		{
			%dmgAmount = 0;
		}
	}
	else if(%dmgType $= "Melee")		//chance to parry
	{
		%rollRand = getRandom();
		
		//echo("EnemyUnit.Parry: roll:" SPC %rollRand  SPC "<" SPC %this.parryChance);
		
		if(%rollRand < %this.parryChance)
		{
			%dmgAmount = 0;
			%this.generateParrySpark();
			echo("EnemyUnit.takeDamage: Melee attacked parried!");
		}
	}
	
	%this.health -= %dmgAmount;
	
	if( %this.health <= 0)
	{
		%this.kill();
		//%this.safeDelete();
	}

	%this.myHealthbar.assessDamage();
	
	return %dmgAmount;
}



//-----------------------------------------------------------------------------
//concats and returns chromosome code

function EnemyUnit::getChromosome( %this )
{
	%chromosome = %this.toolArmor_count SPC
	%this.toolParry_count SPC
	%this.toolAcid_count SPC
	%this.toolTar_count SPC
	%this.toolBlade_count SPC
	%this.toolShooter_count SPC
	%this.toolBlob_count;
	
	return %chromosome;
}

//-----------------------------------------------------------------------------

function EnemyUnit::kill( %this )
{
	
	echo("EnemyMod.EnemyUnit: body size:" SPC %this.myBodyContainer.getCount());
	
	//iterates through the enemy's tools and deletes them
	%toolCount = %this.myBodyContainer.getCount();
	for(%i = 0; %i < %toolCount; %i++)
	{
		%currTool = %this.myBodyContainer.getObject(0);
		%currTool.safeDelete();
	}
	
	//check whether a pickup drops
	if(getRandom(100) < %this.myArena.dropPickupChance)
	{
		if(getRandom(100) < 50)
		{
			//add health pickup------------------------
			%healthPickup = new CompositeSprite()
			{
				class = "Pickup";
			};	
				
			%healthPickup.setPosition(%this.getPosition());
			%this.getScene().add( %healthPickup );
		}
		else
		{
			//add speedshot pickup------------------------
			%speedPickup = new CompositeSprite()
			{
				class = "SpeedShotPickup";
			};
				
			%speedPickup.setPosition(%this.getPosition());
			%this.getScene().add( %speedPickup );
		}
	}
	
	%this.safeDelete();
	%this.myArena.EnemyCount--;
}

//-----------------------------------------------------------------------------

function EnemyUnit::getMyScene( %this )
{
	return %this.getScene();
}

//-----------------------------------------------------------------------------

function EnemyUnit::onRemove( %this )
{
	//echo("EnemyMod.EnemyUnit: Deleted");
	
	//echo( %this.shooterDamage );
	//echo( %this.shooterShotsFired );
	//echo( %this.bladeDamage );
	//echo( %this.bladeAttackNums );
	
	//tally enemy unit's stats to room's
	%this.myArena.roomShooterDamage += %this.shooterDamage;
	%this.myArena.roomShooterShotsFired += %this.shooterShotsFired;
	%this.myArena.roomBladeDamage += %this.bladeDamage;
	%this.myArena.roomBladeAttackNums += %this.bladeAttackNums;
	
	if(isObject(%this.myArena) && %this.myArena.EnemyCount <= 0)
	{
		%this.myArena.schedule(1000, "finishRoom");
	}
}
