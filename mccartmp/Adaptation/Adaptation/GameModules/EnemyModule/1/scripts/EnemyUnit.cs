//-----------------------------------------------------------------------------
// PlayerModule: EnemyUnit class and functions
//-----------------------------------------------------------------------------

function EnemyUnit::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function EnemyUnit::initialize(%this)
{
	exec("./ToolNode.cs");
	%this.setSceneGroup(10);		//Enemy Unit sceneGroup

	%this.health = 100;
	
	%this.setAngle(90);
	
	%this.sizeRatio = 0.8;
	
	//%this.setupSprite();
	%this.setupBehaviors();

	%this.setSceneLayer(10);
	
    %this.createPolygonBoxCollisionShape(74, 78);
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( "5 15" );
	//%this.CollisionCallback = true;
	%this.setCollisionCallback(true);
					
	%this.configureTools(%this.myChromosome);		// ordering: shield/parry/acid/tar/blade/shooter/blob (+1)
}

//-----------------------------------------------------------------------------

function EnemyUnit::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteImage("GameAssets:basicenemy", 0);
	%this.setSpriteSize(74, 78);
}

//-----------------------------------------------------------------------------

function EnemyUnit::setupBehaviors( %this )
{
	exec("./behaviors/movement/Drift.cs");
	exec("./behaviors/ai/faceObject.cs");
	%driftMove = DriftBehavior.createInstance();
	%driftMove.minSpeed = %this.minSpeed;
	%driftMove.maxSpeed = %this.maxSpeed;
	%this.addBehavior(%driftMove);
	
	%faceObj = FaceObjectBehavior.createInstance();
	%faceObj.object = mainPlayer;
	%faceObj.rotationOffset = 180;
	%this.addBehavior(%faceObj);
}

//-----------------------------------------------------------------------------
///ordering: shield/parry/acid/tar/blade/shooter/blob

function EnemyUnit::configureTools(%this, %chromosome)
{
	%this.toolArmor_count = getWord(%chromosome, 0);
	%this.toolParry_count = getWord(%chromosome, 1);
	%this.toolAcid_count = getWord(%chromosome, 2);
	%this.toolTar_count = getWord(%chromosome, 3);
	%this.toolBlade_count = getWord(%chromosome, 4);
	%this.toolShooter_count = getWord(%chromosome, 5);
	%this.toolBlob_count = getWord(%chromosome, 6);
	
	%this.bodyIndexer = 0;
	
	%this.myBodyContainer = new SimSet();
	
	%this.myBodyContainer.add(%this.addToolNode("Blob", "0 0"));	//0,0 is always first Blob node
	
	for(%i = 1; %i < %this.toolBlob_count; %i++) 		
	{
		%nextPosition = %this.findViablePosition(%this.myBodyContainer.getObject(%i - 1).getOpenSlots(), true);
		
		%this.myBodyContainer.add(%this.addToolNode("Blob", %nextPosition));
	}
	
	%nextPosition = "";
	
	%this.configureSingleTool("Armor", %this.toolArmor_count);
	%this.configureSingleTool("Blade", %this.toolBlade_count);
	%this.configureSingleTool("Shooter", %this.toolShooter_count);
	
	%this.initiateToolSprites();		//ensures proper sprite layering
}

//-----------------------------------------------------------------------------

function EnemyUnit::configureSingleTool( %this, %toolType, %toolCount )
{
	for(%i = 0; %i < %toolCount; %i++) 		
	{
		%allPositions = "";
		
		for(%j = 0; %j < %this.myBodyContainer.getCount(); %j++)
		{
			%allPositions = %allPositions @ %this.myBodyContainer.getObject(%j).getOpenSlots() @ " ";
		}
		
		%nextPosition = %this.findViablePosition(%allPositions, true);
		
		%nextNode = %this.addToolNode(%toolType, %nextPosition);
		%this.myBodyContainer.add(%nextNode);
		
	}
}

//-----------------------------------------------------------------------------

function EnemyUnit::initiateToolSprites( %this )
{
	%this.orderTools();
	for(%j = %this.myBodyContainer.getCount() - 1; %j >= 0; %j--)
	{
		%this.myBodyContainer.getObject(%j).setupSprites();
	}
}

//-----------------------------------------------------------------------------

function EnemyUnit::sortTools( %this )
{
	for(%j = 1; %j < %this.myBodyContainer.getCount(); %j++)
	{
		%insertIndex = %j;
		for(%i = %j - 1; %i >= 0; %i--)
		{
			if(%this.myBodyContainer.getObject(%j).sortLevel < %this.myBodyContainer.getObject(%i).sortLevel)
			{
				%insertIndex = %i;
			}
		}
		
		if(%insertIndex != %j)
		{
			%temp = %this.myBodyContainer.getObject(%insertIndex);
			//insert, shift...
		}
	}
}

//-----------------------------------------------------------------------------

function EnemyUnit::orderTools( %this )
{
	for(%j = 0; %j < %this.myBodyContainer.getCount(); %j++)
	{
		if(%this.myBodyContainer.getObject(%j).toolType $= "Armor")
		{
			%this.myBodyContainer.bringToFront(%this.myBodyContainer.getObject(%j));
		}
	}
}

//-----------------------------------------------------------------------------
///call this to add tools to body. ensures toolNode is tracked in grid

function EnemyUnit::addToolNode( %this, %type, %position)
{
	%tool = %this.createToolNode(%type, getWord(%position, 0), getWord(%position, 1));
	%this.addToGrid(%tool);
	return %tool;
}

//-----------------------------------------------------------------------------

function EnemyUnit::addToGrid( %this, %tool)
{
	%this.myBody[%tool.bodyPosX, %tool.bodyPosY] = %tool;
}

//-----------------------------------------------------------------------------
///should only be called through addToolNode(...)

function EnemyUnit::createToolNode( %this, %type, %posX, %posY)
{
	%nextTool = new SceneObject()
	{
		class = "ToolNode";
		owner = %this;
		toolType = %type;
		bodyPosX = %posX;
		bodyPosY = %posY;
		orientation = %this.findToolOrientation(%posX, %posY);
	};
	
	return %nextTool;
}

//-----------------------------------------------------------------------------

function EnemyUnit::findToolOrientation( %this, %posX, %posY)
{
	if(isObject(%this.myBody[%posX, %posY - 1]))		//down
	{
		if(%this.myBody[%posX, %posY - 1].toolType $= "Blob")
			return 0;
	}
	if(isObject(%this.myBody[%posX + 1, %posY]))		//right
	{
		if(%this.myBody[%posX + 1, %posY].toolType $= "Blob")
			return 90;
	}
	if(isObject(%this.myBody[%posX, %posY + 1]))		//up
	{
		if(%this.myBody[%posX, %posY + 1].toolType $= "Blob")
			return 180;
	}
	if(isObject(%this.myBody[%posX - 1, %posY]))		//left
	{
		if(%this.myBody[%posX - 1, %posY].toolType $= "Blob")
			return 270;
	}
	return 0;
}

//-----------------------------------------------------------------------------
///finds free body coordinate to add toolNode to

function EnemyUnit::findViablePosition( %this, %positions , %flag)			
{
	for(%i = 0; %i < getWordCount(%positions) - 1; %i += 2)
	{
		%currPosition = getWord(%positions, %i) SPC getWord(%positions, %i + 1);
		
		if(!isObject(%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]))
		{
			return %currPosition;
		}
	}
	
	return "0 0";		// "0 0" reserved for first Blob tool node added, so this means no viable positions found
}

//-----------------------------------------------------------------------------
///concats and returns chromosome code

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

function EnemyUnit::destroy( %this )
{
}
