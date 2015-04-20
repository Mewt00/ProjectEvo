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
	%this.walkSpeed = 15;
	%this.turnSpeed = 60;
	
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
	
	%this.sizeRatio = $pixelToWorldRatio;
	
	%this.moveBehaviorCount = 0;
	%this.specialX = 0;
	%this.specialY = 0;
  
	if(%this.noBehaviors != 1)
	{
		%this.setAngle(getRandom(360));
		%this.setupBehaviors();
	}
	
	%this.setSceneLayer(10);
    %this.setCollisionGroups( Utility.getCollisionGroup("Player") SPC Utility.getCollisionGroup("Wall")  SPC Utility.getCollisionGroup("Enemies") );
	%this.setCollisionCallback(true);
			
	//Parse local chromsome and build body
	%this.maxBodySize = 0;								//largest (abs) indices for myBody position
	%this.configureTools(%this.myChromosome);		// ordering: shield/parry/acid/tar/blade/shooter/blob (+1)
	
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
//TODO: I think you can delete this function. Ensure no one is calling it by mistake

function EnemyUnit::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:basicenemy", 0);
	%this.setSpriteSize(74, 78);
	
	%obj = new T2dShapeVector()   
    {   
        scenegraph = %this;   
    };    
    %obj.setPolyPrimitive( 4 );  
    %obj.setPolyCustom( 4, "0 0 0 1 1 1 1 0" );  
    %obj.setSize( 500, 300 );  
    %obj.setLineColor( "0 0 0 1" );  
    %obj.setFillMode( true );  
    %obj.setFillColor( "1 0 0" );  
    %obj.setFillAlpha( 0.6 );  
    %obj.setLayer( 1 ); 
}

//-----------------------------------------------------------------------------

function EnemyUnit::setupBehaviors( %this )
{
	exec("./behaviors/movement/Drift.cs");
	exec("./behaviors/movement/wanderAround.cs");
	exec("./behaviors/ai/faceObject.cs");
	exec("./behaviors/movement/minDistance.cs");
	exec("./behaviors/movement/maxDistance.cs");
	exec("./behaviors/movement/strafe.cs");
	/*
	%driftMove = DriftBehavior.createInstance();
	%driftMove.speed = %this.walkSpeed;
	%this.addBehavior(%driftMove);
	
	
	%wanderMove = WanderAroundBehavior.createInstance();
	%wanderMove.turnDelay = 1;
	%wanderMove.numDires = 8;
	%wanderMove.moveSpeed = %this.walkSpeed;
	%wanderMove.turnSpeed = %this.turnSpeed;
	%this.addBehavior(%wanderMove);
	*/
	%faceObj = FaceObjectBehavior.createInstance();
	%faceObj.object = %this.mainTarget;
	%faceObj.rotationOffset = 0;
	%this.addBehavior(%faceObj);
  
	%minDistance = MinDistanceBehavior.createInstance();
	%this.addBehavior(%minDistance);
  
	%maxDistance = MaxDistanceBehavior.createInstance();
	%this.addBehavior(%maxDistance);
  
	%strafe = StrafeBehavior.createInstance();
	%this.addBehavior(%strafe);
}

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
  %temp = %this.specialX SPC %this.specialY;
  %temp = VectorScale(VectorNormalize(%temp), %this.walkSpeed);
  %this.setLinearVelocity(getWord(%temp, 0), getWord(%temp, 1));
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
		%this.safeDelete();
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
	%this.myArena.EnemyCount--;
	
	echo("EnemyMod.EnemyUnit: body size:" SPC %this.myBodyContainer.getCount());
	%toolCount = %this.myBodyContainer.getCount();
	for(%i = 0; %i < %toolCount; %i++)
	{
		%currTool = %this.myBodyContainer.getObject(0);
		%currTool.safeDelete();
	}
	
	
	if(getRandom(100) < %this.myArena.dropPickupChance)
	{
		if(getRandom(100) < 70)
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
	
	if(%this.myArena.EnemyCount <= 0)
	{
		%this.myArena.schedule(1000, "finishRoom");
	}
}
