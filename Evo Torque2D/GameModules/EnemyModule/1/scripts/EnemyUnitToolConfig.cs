//Separate file for tool configuration
//Same CLASS as EnemyUnit

//-----------------------------------------------------------------------------
///ordering: armor/parry/acid/tar/blade/shooter/blob
///Build actual body based off chromosome

function EnemyUnit::configureTools(%this, %chromosome)
{
	%this.numberToolTypes = 7;			//including Blob
	
	//names of corresponding tool types ->>> createToolNode() decodes in this order!!!
	%this.toolTypeNames[0] = "Armor";
	%this.toolTypeNames[1] = "Parry";
	%this.toolTypeNames[2] = "Acid";
	%this.toolTypeNames[3] = "Tar";
	%this.toolTypeNames[4] = "Blade";
	%this.toolTypeNames[5] = "Shooter";
	%this.toolTypeNames[6] = "Blob";		//should always be last!
	
	%this.blobToolName = %this.toolTypeNames[%this.numberToolTypes - 1];
	
	//number of each tool to add to body
	for(%i = 0; %i < %this.numberToolTypes; %i++)
	{
		%this.toolTypeCounts[%i] = getWord(%chromosome, %i);
	}
	
	%this.bodyIndexer = 0;
	
	%this.myBodyContainer = new SimSet();
	
	
	//Blobs first
	%this.myBodyContainer.add(%this.addToolNode(%this.blobToolName, "0 0"));	//0,0 is always first Blob node
	%this.nextBlobDirection = 0;			// 0=left, 1=down, 2=right, 3=up
	
	for(%i = 1; %i < %this.toolTypeCounts[%this.numberToolTypes - 1]; %i++) 		
	{
		%nextPosition = %this.findNextBlobPosition(%this.myBodyContainer.getObject(%i - 1).getAdjacentSlots(), 0);
		
		%this.myBodyContainer.add(%this.addToolNode(%this.blobToolName, %nextPosition));
	}
	
	//Rest of Tools
	%nextPosition = "";
	
	for(%i = 0; %i < %this.numberToolTypes; %i++)
	{
		%toolToAddTypeCounts[%i] = %this.toolTypeCounts[%i];
	}
	
	%stillAddingSum = 1;
	while(%stillAddingSum > 0)
	{
		%stillAddingSum = 0;
		
		for(%i = 0; %i < %this.numberToolTypes - 1; %i++)
		{
			%stillAddingSum = %stillAddingSum + %this.configureSingleTool(%this.toolTypeNames[%i], %toolToAddTypeCounts[%i]);
			
			%toolToAddTypeCounts[%i] = (%toolToAddTypeCounts[%i] - 1);
		}
	}
	
	//%this.checkLargeBodyBlobs();
	
	%this.initiateTools();		//ensures proper sprite layering
	
	%this.echoBodyLayout();
}

//-----------------------------------------------------------------------------

function EnemyUnit::configureSingleTool( %this, %toolType, %toAddCount )
{
	if(%toAddCount <= 0)
	{
		return 0;
	}
	else
	{
		%allPositions = "";
		
		for(%j = 0; %j < %this.myBodyContainer.getCount(); %j++)
		{
			%allPositions = %allPositions @ %this.myBodyContainer.getObject(%j).getAdjacentSlots();
		}
		
		
		%nextPosition = %this.findOpenSlot(%allPositions);
		
		if(%nextPosition $= "x")
		{
			%nextPosition = %this.findSameTypeSlot(%allPositions, %toolType, -1);
			
			%this.stackToolNode(%nextPosition, 1);
		}
		else
		{
			%nextNode = %this.addToolNode(%toolType, %nextPosition);
			%this.myBodyContainer.add(%nextNode);
		}
		
		return 1;
	}
}

//-----------------------------------------------------------------------------
///finds free body coordinate to add toolNode to (%positions is all possible positions in which to place new tool)

function EnemyUnit::findOpenSlot( %this, %positions)			
{
	for(%i = 0; %i < mFloor(getWordCount(%positions)/2); %i++)
	{
		%currPosition = getWord(%positions, %i*2) SPC getWord(%positions, %i*2 + 1);			//get position 
		
		if(!isObject(%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]))
		{
			return %currPosition;
		}
	}
	
	return "x";		// no open slots found
}

//-----------------------------------------------------------------------------
///finds a existing slot that is occupied by the same tool Type

function EnemyUnit::findSameTypeSlot( %this, %positions, %toolType, %lowestStackLevel)			
{
	%result = "0 0";
	
	if(getWordCount(%positions) == 0)
	{
		return "x";
	}
	
	for(%i = 0; %i < getWordCount(%positions) - 1; %i += 2)
	{
		%currPosition = getWord(%positions, %i) SPC getWord(%positions, %i + 1);			//get position 
		
		%currToolNode = (%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]);
		
		if(%currToolNode.toolType $= %toolType)
		{
			%currStackLevel = %currToolNode.stackLevel;
			
			if(%currStackLevel < %lowestStackLevel)
			{
				return %currPosition;
			}
			else
			{
				%posistionsMinusCurr = removeWord(%positions, 0);
				%posistionsMinusCurr = removeWord(%posistionsMinusCurr, 0);
				
				%nexToolPos = %this.findSameTypeSlot(%posistionsMinusCurr, %toolType, %currStackLevel);
				
				if(%lowestStackLevel == -1)		//first call
				{
					if(%nexToolPos $= "x")		//all equal
					{
						return %currPosition;
					}
				}
				
				return %nexToolPos;
			}
		}
	}
	
	return %result;		// no like slots found
}

//-----------------------------------------------------------------------------
function EnemyUnit::initiateTools( %this )
{
	%this.orderTools();
	for(%j = %this.myBodyContainer.getCount() - 1; %j >= 0; %j--)
	{
		%this.myBodyContainer.getObject(%j).initialize();
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
	%xPos = getWord(%position, 0);
	%yPos = getWord(%position, 1);

	%tool = %this.createToolNode(%type, %xPos, %yPos);
	%this.addToGrid(%tool);
	
	//track body size
	if(mAbs(%xPos) > %this.maxBodySize)
		%this.maxBodySize = mAbs(%xPos);
		
	if(mAbs(%yPos) > %this.maxBodySize)
		%this.maxBodySize = mAbs(%xPos);
	
	return %tool;
}

//-----------------------------------------------------------------------------
///call this to add tools to body. ensures toolNode is tracked in grid

function EnemyUnit::stackToolNode( %this, %position, %stackCount)
{
	%xPos = getWord(%position, 0);
	%yPos = getWord(%position, 1);
	
	%this.myBody[%xPos, %yPos].stackLevel = (%this.myBody[%xPos, %yPos].stackLevel + %stackCount);
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
	if(%type $= %this.toolTypeNames[0]){
		%nextTool = ToolArmor::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	else if(%type $= %this.toolTypeNames[1]){
		%nextTool = ToolParry::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	else if(%type $= %this.toolTypeNames[2]){
		%nextTool = ToolAcid::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	else if(%type $= %this.toolTypeNames[3]){
		%nextTool = ToolTar::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	else if(%type $= %this.toolTypeNames[4]){
		%nextTool = ToolBlade::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	else if(%type $= %this.toolTypeNames[5]){
		%nextTool = ToolShooter::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	else{
		%nextTool = ToolNode::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));
	}
	
	return %nextTool;
}

//-----------------------------------------------------------------------------

function EnemyUnit::findToolOrientation( %this, %posX, %posY)
{
	if(isObject(%this.myBody[%posX, %posY - 1]))		//down
	{
		if(%this.myBody[%posX, %posY - 1].toolType $= %this.blobToolName)	//"Blob"
			return 90;
	}
	if(isObject(%this.myBody[%posX + 1, %posY]))		//right
	{
		if(%this.myBody[%posX + 1, %posY].toolType $= %this.blobToolName)
			return 180;
	}
	if(isObject(%this.myBody[%posX, %posY + 1]))		//up
	{
		if(%this.myBody[%posX, %posY + 1].toolType $= %this.blobToolName)
			return 270;
	}
	if(isObject(%this.myBody[%posX - 1, %posY]))		//left
	{
		if(%this.myBody[%posX - 1, %posY].toolType $= %this.blobToolName)
			return 0;
	}
	return 0;
}

//-----------------------------------------------------------------------------
///finds free body coordinate to add blob to (%positions is all possible positions in which to place new tool)
///works in outward spiral. e.g: starting at A then B then C...
///      J I H G .
///      K B A F .
///      L C D E .
///      M N O P Q

function EnemyUnit::findNextBlobPosition( %this, %positions, %bootstrap )			
{
	%bootstrap++;
	if(%bootstrap >= 4)
		return "0 0";		// "0 0" reserved for first Blob tool node added, so this means no viable positions found
	
	for(%i = 0; %i < getWordCount(%positions) - 1; %i += 2)
	{
		if((%i/2) == %this.nextBlobDirection)
		{
			%currPosition = getWord(%positions, %i) SPC getWord(%positions, %i + 1);
			
			
			if(!isObject(%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]))
			{
				%this.nextBlobDirection++;
				if(%this.nextBlobDirection > 3)
					%this.nextBlobDirection = 0;
					
				return %currPosition;
			}
			else
			{				
				%this.nextBlobDirection--;
				if(%this.nextBlobDirection < 0)
					%this.nextBlobDirection = 3;	
					
				return %this.findNextBlobPosition( %positions, %bootstrap);		//recursive search for position
			}
		}
	}
}

//-----------------------------------------------------------------------------
///finds free body coordinate to add toolNode to (%positions is all possible positions in which to place new tool)

function EnemyUnit::checkLargeBodyBlobs( %this )			
{
	%scale = %this.maxBodySize;
	
	for(%i = %scale; %i > -%scale; %i--)
	{
		for(%j = %scale; %j > -%scale; %j--)
		{
			%currNode = %this.myBody[%i, %j];
			
			if( isObject(%currNode) )
			{
				if(%currNode.toolType $= %this.blobToolName)
				{
					if(%currNode.drawSprite == true)
					{
						%this.blobSearch2x2( %currNode );
					}
				}
			}
		}
	}
}

//-----------------------------------------------------------------------------
///finds free body coordinate to add toolNode to (%positions is all possible positions in which to place new tool)

function EnemyUnit::blobSearch2x2( %this, %rootBlob )			
{
	%scale = %this.maxBodySize;
	
	%nodeList = new SimSet();
	
	echo("Enemy.EnemyUnit: Check" SPC %rootBlob.bodyPosX SPC %rootBlob.bodyPosY);
	
	%objA = %this.myBody[%rootBlob.bodyPosX, %rootBlob.bodyPosY - 1];
	%objB = %this.myBody[%rootBlob.bodyPosX - 1, %rootBlob.bodyPosY];
	%objC = %this.myBody[%rootBlob.bodyPosX - 1, %rootBlob.bodyPosY - 1];
	
	if(!isObject(%objA) || !isObject(%objB) || !isObject(%objC))
		return false;
	
	%nodeList.add(%objA);		//down
	%nodeList.add(%objB);		//left
	%nodeList.add(%objC);	//left-down

	echo("Enemy.EnemyUnit: check");
	
	for(%i = 0; %i < %nodeList.getCount(); %i++)
	{
		if( isObject(%nodeList.getObject(0)) )
		{
			if(! %nodeList.getObject(0).toolType $= "Blob")
			{
				return false;
			}
			else if( %nodeList.getObject(0).drawSprite == false )		//already used in larger body check
			{
				return false;
			}
		}
		else
		{
			return false;
		}
	}
	
	echo("Enemy.EnemyUnit: found big body");
	
	for(%i = 0; %i < %nodeList.getCount(); %i++)
	{
		%nodeList.getObject(0).drawSprite = false;
	}
	
	return true;
}

//-----------------------------------------------------------------------------

function EnemyUnit::echoBodyLayout( %this)
{
	%scale = %this.maxBodySize;
	echo("");	//newline
	echo("Enemy.EnemyUnit: Obj ID:" SPC %this.getID());
	
	%xAxisLabel = "  ";
	for(%i = -%scale; %i <= %scale; %i++)
	{
		if(%i >= 0)
			%xAxisLabel = %xAxisLabel SPC "" SPC %i;
		else
			%xAxisLabel = %xAxisLabel SPC %i;
	}
	echo(%xAxisLabel);
	
	for(%i = %scale; %i >= -%scale; %i--)
	{
		if(%i >= 0)
			%line = " " @ %i;
		else
			%line = %i;
		
		for(%j = -%scale; %j <= %scale; %j++)
		{
			%nodeChar = " ";
			%currNode = %this.myBody[%j, %i];
			if( isObject(%currNode) )
			{
				if(%currNode.toolType $= "Blob")
					%nodeChar = "#";
				else if(%currNode.toolType $= "Armor")		//(Acid is 'A')
					%nodeChar = "@";
				else
					%nodeChar = getSubStr(%currNode.toolType, 0, 1);
			}
			%line = %line SPC "" SPC %nodeChar;
		}
		echo(%line);
	}
	
	echo("");
}