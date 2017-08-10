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
	
	//--Execute Behavior scripts-----------------------
	exec("./behaviors/ai/faceObject.cs");
	exec("./behaviors/movement/chase.cs");
	exec("./behaviors/movement/zigzag.cs");
	exec("./behaviors/movement/lunge.cs");
	exec("./behaviors/movement/strafe.cs");
	exec("./behaviors/movement/blink.cs");
	exec("./behaviors/movement/minDistance.cs");
	exec("./behaviors/movement/maxDistance.cs");
	
    //%this.setUpdateCallback(true);

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
	%this.behaviorExtras = "";
	%this.behaviorNamesSize = 0;
	%this.moveBehaviorCount = 0;
	%this.specialX = 0;
	%this.specialY = 0;

	echo("inside enemy");
	

	%this.myBehaviors = new SimSet();
	
	%this.setSceneLayer(10);
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") SPC Utility.getCollisionGroup("Wall")  SPC Utility.getCollisionGroup("Enemies") );
	%this.setCollisionCallback(true);
			
	//Parse local chromosome and build body
	%this.maxBodySize = 0;								//largest (abs) indices for myBody position
	%this.configureTools(%this.myChromosome);		// ordering: shield/parry/acid/tar/blade/shooter/blob (+1)
	
	echo("getWordCount(%this.myBehaviors)" SPC getWordCount(%this.myBehaviors));
	for(%i = 0; %i < getWordCount(%this.myBehaviors); %i++)		
	{
		echo("%this.behaviorNames   " @ %this.myBehaviors.friendlyName SPC %this.myBehaviors.number);
	}
	
	//echo("above setup behaviors");
	//%this.setupBehaviors();
	
	/*%this.segmentWidth = 12 * $pixelsToWorldUnits;
	%this.segmentHeight = 12 * $pixelsToWorldUnits;
	
	%this.drawHealthbar(%this.health);*/
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
	exec("./behaviors/movement/minDistance.cs");
	exec("./behaviors/movement/maxDistance.cs");
	
	//List of behaviors to attach
	
	//%this.behaviorNames = "BlinkBehavior 1 ChaseBehavior 1"; // ZigZagBehavior 1 LungeBehavior 1 StrafeBehavior 1";
	//%this.behaviorNamesSize = 0;
	//%this.addToolBehavior("BlinkBehavior", 1);
	for(%i = 0; %i < getWordCount(%this.myBehaviors); %i++)		
	{
		echo("%this.behaviorNames   " @ %this.myBehaviors.friendlyName SPC %this.myBehaviors.number);
	}
	
	
	/*for(%i = 0; %i < %this.behaviorNamesSize; %i += 2)
	{
		echo("1");
		%individualBehavior = %this.behaviorNames[%i].createInstance();
		%individualBehavior.target = %this.mainTarget;
		%individualBehavior.number = %this.behaviorNames[%i + 1];
		%this.addBehavior(%individualBehavior);
		echo("2");
		//for(%j = 0; %j < getWordCount(%this.myArena.pausers); %j++)
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
		//}
		
		echo("name :   " @ getWord(%this.behaviorNames, %i) @ getWord(%this.behaviorNames, %i + 1));
		
	}*/
	
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
}

//-----------------------------------------------------------------------------

function EnemyUnit::addEnemyBehavior( %this, %behavior, %number )
{
	echo("Inside addEnemyBehavior1");
	%this.addEnemyBehavior(%behavior, %number, -9999);
}

function EnemyUnit::addEnemyBehavior( %this, %behavior, %number, %extra )
{
	echo("Inside addEnemyBehavior2");
	if(getWordCount(%this.myBehaviors) != 0)
	{
		for(%i = 0; %i < getWordCount(%this.myBehaviors); %i++)		
		{
			echo("friendly    behavior" SPC %this.myBehaviors.friendlyName SPC %behavior);
			if(%this.myBehaviors.friendlyName $= %behavior)
			{
				%this.myBehaviors.number = %this.myBehaviors.number + %number;
				return;
			}
		}
	}
	%individualBehavior = %behavior.createInstance();
	%individualBehavior.target = %this.mainTarget;
	%individualBehavior.number = %number;
	%individualBehavior.extra = %extra;
	%this.addBehavior(%individualBehavior);
	%this.myBehaviors.add(%individualBehavior);
}

/*//-----------------------------------------------------------------------------
function EnemyUnit::addToolBehavior( %this, %behavior, %number )
{
	%this.addToolBehavior(%behavior, %number, -9999);
}

function EnemyUnit::addToolBehavior( %this, %behavior, %number, %extra )
{
	echo("Inside addToolBehavior");
	if(%this.behaviorNamesSize != 0)
	{
		for(%i = 0; %i < %this.behaviorNamesSize; %i += 2)		
		{
			if(%this.behaviorNames[%i] $= %behavior)
			{
				%this.behaviorNames[%i + 1] = %this.behaviorNames[%i + 1] + %number;
				//%this.behaviorNamesSize++;
				return;
			}
		}
		%this.behaviorNames[%this.behaviorNamesSize] = %behavior;
		%this.behaviorNames[%this.behaviorNamesSize + 1] = %number;
		%this.behaviorNamesSize += 2;
	}
	else
	{
		%this.behaviorNames[0] = %behavior;
		%this.behaviorNames[1] = %number;
		%this.behaviorNamesSize += 2;
		//echo("%this.behaviorNames   " @ %this.behaviorNames[0] SPC %this.behaviorNames[1]);
	}
}*/


//-----------------------------------------------------------------------------

/*function EnemyUnit::assessDamage( %this )
{
	if(!isObject(%this.owner))
	{
		%this.safeDelete();
		return;
	}
		
	%this.clearSprites();

	%this.drawHealthbar();
	
}
//-----------------------------------------------------------------------------

function Healthbar::updateHealthbar(%this)
{
	%numberBars = %this.fullHealth/10;
	
	%currentHealthBars = %this.health/10;
	
	for(%i = 0; %i < %numberBars - %currentHealthBars; %i++)
	{
		%this.selectSpriteName("")
		%this.addSprite(%currX SPC "10");
		%this.setSpriteImage("GameAssets:healthBarSegment", 0);
		%this.setSpriteSize(%this.segmentWidth, %this.segmentHeight);
		
		%currX += %this.segmentWidth;
	}
	
}

//-----------------------------------------------------------------------------

function Healthbar::drawHealthbar(%this)
{
	%numSegs = %this.health/10;
	
	%currentX = -(%numSegs*%this.segmentWidth)/2;
	for(%i = 0; %i < %numSegs; %i++)
	{
		%this.addSprite(%currentX SPC "10");
		%this.setSpriteImage("GameAssets:healthBarSegment", 0);
		%this.setSpriteSize(%this.segmentWidth, %this.segmentHeight);
		%this.setSpriteName(%i);
		
		%currentX += %this.segmentWidth;
	}
}*/

//-----------------------------------------------------------------------------

function EnemyUnit::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player"))
	{
		%object.hit(5, %this);			//Hit player for 5 on contact
	}
}

//-----------------------------------------------------------------------------

function EnemyUnit::updateGroup(%this )
{
	echo("updateGroup in enemy");
	if(getWordCount(%this.myBehaviors) == 0)
		return;
    for(%i = 0; %i < getWordCount(%this.myBehaviors); %i++)
    {
        // iterate the group, call each object's update() method
        %obj = %this.myBehaviors.getObject(%i);
        %obj.onUpdate();
    }
    // do it again in 250 ms
    //schedule(32, "updateGroup", 0);
	%this.updateEnemy();
}

//-----------------------------------------------------------------------------

function EnemyUnit::updateEnemy( %this )
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

function EnemyUnit::onUpdate( %this )
{
	/*echo("enemy: " @ %this.getId());
	echo("enemy speed is " @ %this.tempLinearVelocityX @ " by " @ %this.tempLinearVelocityY);
	echo("");
	//%this.setLinearVelocityX(0);
	//%this.setLinearVelocityY(0);
	%this.setLinearVelocityX(%this.tempLinearVelocityX);
	%this.setLinearVelocityY(%this.tempLinearVelocityY);
	%this.tempLinearVelocityX = 0;
	%this.tempLinearVelocityY = 0;*/
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
	
	%this.clearBehaviors();
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
