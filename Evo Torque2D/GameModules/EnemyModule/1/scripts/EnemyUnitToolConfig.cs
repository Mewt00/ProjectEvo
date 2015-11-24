//This is a separate file for tool configuration
//Same CLASS as EnemyUnit

//"This is probably the hardest code to follow in all the scripts. I'll try to explain any illogical logic there is" -Mike

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
	%this.toolTypeNames[6] = "Blob";		//blob should always be last!
	
	%this.blobToolName = %this.toolTypeNames[%this.numberToolTypes - 1];
	
	//number of each tool to add to body
	for(%i = 0; %i < %this.numberToolTypes; %i++)
	{
		%this.toolTypeCounts[%i] = getWord(%chromosome, %i);
	}
	
	%this.bodyIndexer = 0;
	
	%this.myBodyContainer = new SimSet();
	
	//Build blobs first
	//Add first blob
	%this.myBodyContainer.add(%this.addToolNode(%this.blobToolName, "0 0"));	//0,0 is always first Blob node
	%this.nextBlobDirection = 0;			// 0=left, 1=down, 2=right, 3=up
	
	//Blobs form in spiral, outwards from centre blob. This maintains relatively square blob bodies
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
	
	//Add one tool at a time, cycling through types- ensures as many DIFFERENT tool types get 
	//spots on blobs, before they start stacking
	%stillAddingSum = 1;
	while(%stillAddingSum > 0)			//continue while tool has been added in the last loop
	{
		%stillAddingSum = 0;
		
		for(%i = 0; %i < %this.numberToolTypes - 1; %i++)		//cycle types
		{
			%stillAddingSum = %stillAddingSum + %this.configureSingleTool(%this.toolTypeNames[%i], %toolToAddTypeCounts[%i]);
			
			%toolToAddTypeCounts[%i] = (%toolToAddTypeCounts[%i] - 1);		//increment left toAdd count down
		}
	}
	
	//%this.checkLargeBodyBlobs();
	
	%this.initiateTools();		//ensures proper sprite layering
	
	%this.echoBodyLayout();
}

//-----------------------------------------------------------------------------
// TODO: would probably put %toAddCount check outside of function, where it is being called. No need/awkward to be inside

function EnemyUnit::configureSingleTool( %this, %toolType, %toAddCount )
{
	if(%toAddCount <= 0)
	{
		return 0;
	}
	else
	{
		%allPositions = "";
		
		//get all slots adjacent to blobs
		for(%j = 0; %j < %this.myBodyContainer.getCount(); %j++)
		{
			%allPositions = %allPositions @ %this.myBodyContainer.getObject(%j).getAdjacentSlots();
		}
		
		//look through all adjacent slots for any not taken already
		%nextPosition = %this.findOpenSlot(%allPositions);
	
		if(%nextPosition $= "x")		//if all slots are taken, start stacking
		{
			%nextPosition = %this.findSameTypeSlot(%allPositions, %toolType, -1);
			
			%this.stackToolNode(%nextPosition, 1);
		}
		else							//NOT ENOUGH BLOBS: add on an illegal spot. Should not have to happen
		{
			echo("Not enough blobs, illegal 5 tool types to 1 blob");
			%nextNode = %this.addToolNode(%toolType, %nextPosition);		//e.g.: 5 tool types, 1 blob
			%this.myBodyContainer.add(%nextNode);
		}
		
		return 1;
	}
}

//-----------------------------------------------------------------------------
///finds free body coordinate to add toolNode to (%positions is all possible positions in which to place new tool)

function EnemyUnit::findOpenSlot( %this, %positions)			
{
	for(%i = 0; %i < mFloor(getWordCount(%positions)/2); %i++)		//position is a string of points. 2 words = 1 position e.g.: "2 1 5 4" = {{2, 1}, {5, 4}}
	{
		%currPosition = getWord(%positions, %i*2) SPC getWord(%positions, %i*2 + 1);			//get position 
		
		if(!isObject(%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]))		//if no object at slot
		{
			return %currPosition;
		}
	}
	
	return "x";		// no open slots found
}

//-----------------------------------------------------------------------------
///finds a existing slot that is occupied by the same tool Type.
///recursive function.

function EnemyUnit::findSameTypeSlot( %this, %positions, %toolType, %lowestStackLevel)			//	%lowestStackLevel is initiated at -1 for first call
{
	%result = "0 0";
	
	if(getWordCount(%positions) == 0)							//no possible slots left, findOpenSlot() function eliminated all options
	{
		return "x";
	}
	
	for(%i = 0; %i < getWordCount(%positions) - 1; %i += 2)		//position is a string of points. 2 words = 1 position e.g.: "2 1 5 4" = {{2, 1}, {5, 4}}
	{
		%currPosition = getWord(%positions, %i) SPC getWord(%positions, %i + 1);			//get position 
		
		%currToolNode = (%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]);
		
		if(%currToolNode.toolType $= %toolType)					//if same tool type
		{
			%currStackLevel = %currToolNode.stackLevel;			//%lowestStackLevel is variable to ensure the to-add tool is stacked onto the set with the least stacked tools. (least first)
			
			if(%currStackLevel < %lowestStackLevel)				
			{
				return %currPosition;							//stack level is less than previous stack observed, suitable to add tool to
			}
			else
			{
				//copy positions string without first position
				%posistionsMinusCurr = removeWord(%positions, 0);				
				%posistionsMinusCurr = removeWord(%posistionsMinusCurr, 0);		
				
				//recursively call findSameTypeSlot()
				%nexToolPos = %this.findSameTypeSlot(%posistionsMinusCurr, %toolType, %currStackLevel);
				
				if(%lowestStackLevel == -1)		//first call
				{
					if(%nexToolPos $= "x")		//all equal stack levels
					{
						return %currPosition;
					}
				}
				
				return %nexToolPos;
			}
		}
	}
	
	return %result;								// no like-type slots found
}

//-----------------------------------------------------------------------------

function EnemyUnit::initiateTools( %this )
{
	%this.orderTools();
	
	//initiate body tools from back to front of list
	for(%j = %this.myBodyContainer.getCount() - 1; %j >= 0; %j--)
	{
		%this.myBodyContainer.getObject(%j).initialize();
	}
}

//-----------------------------------------------------------------------------
///"I believe this is an unused function" -Mike
///TODO: ensure function is unused and delete it

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
///bring armor objects to front of list to ensure they are drawn on top of everything

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
///call this to add tools to body. ensures toolNode is tracked in body-grid

function EnemyUnit::addToolNode( %this, %type, %position)
{
	%xPos = getWord(%position, 0);
	%yPos = getWord(%position, 1);

	%tool = %this.createToolNode(%type, %xPos, %yPos);
	
	%this.addToGrid(%tool);
	
	//track body size										//quick, useful number to know rough radius of enemy unit
	if(mAbs(%xPos) > %this.maxBodySize)
		%this.maxBodySize = mAbs(%xPos);
		
	if(mAbs(%yPos) > %this.maxBodySize)
		%this.maxBodySize = mAbs(%xPos);
	
	return %tool;
}

//-----------------------------------------------------------------------------
///call this to add tools to body if only stack spots are available

function EnemyUnit::stackToolNode( %this, %position, %stackCount)
{
	%xPos = getWord(%position, 0);
	%yPos = getWord(%position, 1);
	
	%this.myBody[%xPos, %yPos].stackLevel = (%this.myBody[%xPos, %yPos].stackLevel + %stackCount);			//increment stack level
}

//-----------------------------------------------------------------------------
///add tool object to the body grid

function EnemyUnit::addToGrid( %this, %tool)
{
	%this.myBody[%tool.bodyPosX, %tool.bodyPosY] = %tool;
}

//-----------------------------------------------------------------------------
///should only be called through addToolNode(...)
///Checks tool type and returns a new instance of tool of said type

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
		%nextTool = ToolNode::CreateInstance(%this, %type, %posX, %posY, %this.findToolOrientation(%posX, %posY));		//catch all. might not behave correctly
	}
	
	return %nextTool;
}

//-----------------------------------------------------------------------------
///determine what direction a blob is to attach the base of the tool. (so shooters/blades/etc face logical direction)

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
///finds free body-adjacent coordinate to add blob to (%positions is all empty positions in which to place new blob)
///works in outward spiral. e.g: starting at A then B then C...
///      J I H G .
///      K B A F .
///      L C D E .
///      M N O P Q

function EnemyUnit::findNextBlobPosition( %this, %positions, %bootstrap )				//%positions is 4 adjacent spots to previously placed blob		
{
	%bootstrap++;			//recursion should only ever need to check 2, but 4 is max possible if method is changed
	if(%bootstrap >= 4)
		return "0 0";		// "0 0" reserved for first Blob tool node added, so this means no viable positions found (should not be reached in code)
	
	for(%i = 0; %i < getWordCount(%positions) - 1; %i += 2)		// %positions is a string of points. 2 words = 1 position e.g.: "2 1 5 4" = {{2, 1}, {5, 4}}
	{
		if((%i/2) == %this.nextBlobDirection)					//check in direction of nextBlobDirection, ignore all others
		{
			%currPosition = getWord(%positions, %i) SPC getWord(%positions, %i + 1);
			
			if(!isObject(%this.myBody[getWord(%currPosition, 0), getWord(%currPosition, 1)]))		//if slot is open
			{
				%this.nextBlobDirection++;															// turn nextBlobDirection counter-clockwise for next blob
				if(%this.nextBlobDirection > 3)
					%this.nextBlobDirection = 0;
					
				return %currPosition;																// return open position
			}
			else																					//if slot is blocked
			{				
				%this.nextBlobDirection--;															// turn nextBlobDirection back, clockwise in order to try again
				if(%this.nextBlobDirection < 0)
					%this.nextBlobDirection = 3;	
					
				return %this.findNextBlobPosition( %positions, %bootstrap);							// try again recursively, with new nextBlobDirection
																									// this method ensures that the blobs are formed in tight spirals and is continuously trying to turn into the center and turns back to straight if it is not rounding a corner.
			}
		}
	}
}

//-----------------------------------------------------------------------------
///This function was developed in an attempt to change the sprite of clusters of Blobs into one big blob.
///    So four little blobs in a 2x2 arrangement, would be replaced by one big blob image- drawn to be bigger
///    This would add variety to the bodies but the idea was never finished.

///"I believe this is an unused function" -Mike
///TODO: ensure function is unused and delete it

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
///"I believe this is an unused function (besides in checkLargeBodyBlobs())" -Mike
///TODO: ensure function is unused and delete it

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
///Prints a text layout of the body to the console. Useful for seeing enemy make ups.

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