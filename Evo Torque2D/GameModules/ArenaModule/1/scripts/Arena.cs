//-----------------------------------------------------------------------------
// Room setup
//  -Wall objects, player start, enemy locations
//  -Default first chromosome
//  -Text count down
//  -Background images
//  -Music
// Split room chromosome
// End room logic
//-----------------------------------------------------------------------------

function Arena::buildArena(%this)
{
    // Background
    %background = new Sprite() {class="backgroundObj";};
    %background.setBodyType( "static" );
    %background.setImage( "GameAssets:background" );
    %background.setSize( $roomWidth, $roomHeight );
    %background.setCollisionSuppress();
    %background.setAwake( false );
    %background.setActive( false );
    %background.setSceneLayer(30);
    %background.setSceneGroup( Utility.getCollisionGroup("") );
    %this.getScene().add( %background );
    
    // Arena Edges
    %roomEdges = new Sprite() {class="backgroundObj";};
    %roomEdges.setBodyType( "static" );
    %roomEdges.setImage( "GameAssets:backgroundedging" );
    %roomEdges.setSize( $roomWidth, $roomHeight );
    %roomEdges.setCollisionSuppress();
    %roomEdges.setAwake( false );
    %roomEdges.setActive( false );
    %roomEdges.setSceneLayer(2);
    %roomEdges.setSceneGroup( Utility.getCollisionGroup("") );
    %this.getScene().add( %roomEdges ); 
    
    %this.addArenaBoundaries( $roomWidth, $roomHeight );
	
    //%this.setUpdateCallback(true);
	
	%this.dropPickupChance = 15;
	
	//Populate room
	%this.player = %this.spawnPlayer(0, 0);		//add player before Enemies!
	
	%this.roomChromosomes = "";
	
	// Enemy Info
	%this.EnemyCount = 0;
	%this.toolVarietyCount = 7;		//number of different tools available, length of local chromosomes
	
	//-RoomDamageTracking---
	//      (damage dealt by player)
	%this.roomShooterDamage = 0;
	%this.roomShooterShotsFired = 0;
	%this.roomBladeDamage = 0;
	%this.roomBladeAttackNums = 0;

	%this.addRoomFont(-$roomWidth/2 + 1, $roomHeight/2 - 0.5);
	
	$roomStartLag = 1;		//delay before READY text appears
	//%this.getScene().setScenePause(true);
	//%this.getScene().schedule($roomStartLag, "setScenePause", true);
	%this.schedule($roomStartLag, "addREADYFont", 0, 0);
	
	%this.addBackgroundMusic();
	
	if(%this.currLevel == 1)		//starting chromosome for first level, every game
	{
		%this.currChromosome = "0 0 0 0 0 0 1" SPC
					  "0 0 0 0 0 0 1";
	}
	
	//gather spawn points
	%enemyUnitCount = mFloor(getWordCount(%this.currChromosome)/%this.toolVarietyCount);	//number of enemy units
	for(%i = 0; %i < %enemyUnitCount; %i++)
	{
		%this.spawnPoints[%i] = %this.findSpawnLocation();		//record a random spawn location (not on top of player)
	}
}

//-----------------------------------------------------------------------------
//start single backgroun track on repeat
function Arena::addBackgroundMusic(%this)
{
	echo("arena: audio");		
	
	%musicAsset = "GameAssets:roomMusic";
	
	$musicHandle = alxPlay(%musicAsset);	
	
	%this.schedule(alxGetAudioLength(%musicAsset), "addBackgroundMusic");
}

//-----------------------------------------------------------------------------
//display current level/room number
function Arena::addRoomFont(%this, %x, %y)
{
	%text = "Room:" @ %this.currLevel;
	//draw font 4 times (slightly offset) for pseudo boldness
	%this.addRoomNumFont(%x, %y, "3 3", %text, "Left", "1 1 0");
	%this.addRoomNumFont(%x + 0.1, %y, "3 3", %text, "Left", "1 1 0");
	%this.addRoomNumFont(%x + 0.1, %y + 0.1, "3 3", %text, "Left", "1 1 0");
	%this.addRoomNumFont(%x, %y + 0.1, "3 3", %text, "Left", "1 1 0");
}

//-----------------------------------------------------------------------------
//draw specified text in specified color
//TODO: function should probably be renamed to be more generic (drawText(...), or addTextOverlay(...))
function Arena::addRoomNumFont(%this, %x, %y, %size, %text, %align, %colorBlend)
{
	%font = new ImageFont();
	%font.Image = "GameAssets:font";
	%font.Text = %text;
	%font.FontSize = %size;
	%font.setPosition(%x, %y);
	%font.TextAlignment = %align;
	%font.setBlendColor(%colorBlend);
	%this.getScene().add( %font ); 
	
	return %font;
}

//-----------------------------------------------------------------------------

function Arena::addREADYFont(%this, %x, %y)
{
	%text = "READY?";
	%lifeSpan = 1500;
	
	//draw font 4 times (slightly offset) for pseudo boldness
	//TODO: READY/SET/GO 4 (each) font draws draws could be done through a function
	%this.addRoomNumFont(%x, %y, "5 5", %text, "Center", "1 0 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x + 0.1, %y, "5 5", %text, "Center", "1 0 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x + 0.1, %y + 0.1, "5 5", %text, "Center", "1 0 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x, %y + 0.1, %text, "5 5", "Center", "1 0 0").schedule(%lifeSpan, "safeDelete");
	
	//-spawn shadow dusts (effect for enemy spawn)---------------
	//TODO: should probably be moved to a separate function. Call from buildArena()
	%enemyUnitCount = mFloor(getWordCount(%this.currChromosome)/%this.toolVarietyCount);
	for(%i = 0; %i < %enemyUnitCount; %i++)
	{
		%shadowDust = new CompositeSprite()			//create new dust object
		{
			class = "shadowDust";
			myArena = %this;
			lifeSpan = (%lifeSpan*2)/1000;
		};
		
		%this.getScene().add( %shadowDust );		//add dust object to scene
		
		%shadowDust.setPosition(%this.spawnPoints[%i]);	//move dust to coorisponding enemy spawn loc.
	}
	
	//-schedule next text to appear-------------------
	%this.schedule(%lifeSpan, "addSETFont", %x, %y);	
}

//-----------------------------------------------------------------------------

function Arena::addSETFont(%this, %x, %y)
{
	%text = "SET";
	%lifeSpan = 1500;
	//TODO: READY/SET/GO 4 (each) font draws draws could be done through a function
	%this.addRoomNumFont(%x, %y, "6 6", %text, "Center", "1 1 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x + 0.1, %y, "6 6", %text, "Center", "1 1 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x + 0.1, %y + 0.1, "6 6", %text, "Center", "1 1 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x, %y + 0.1, %text, "6 6", "Center", "1 1 0").schedule(%lifeSpan, "safeDelete");
	
	%this.schedule(%lifeSpan, "addGOFont", %x, %y);
	%this.schedule(%lifeSpan, "processRoomChromosomes");
}

//-----------------------------------------------------------------------------

function Arena::addGOFont(%this, %x, %y)
{
	%text = "FIGHT!";
	%lifeSpan = 1500;
	//TODO: READY/SET/GO 4 (each) font draws draws could be done through a function
	%this.addRoomNumFont(%x, %y, "7 7", %text, "Center", "0 1 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x + 0.1, %y, "7 7", %text, "Center", "0 1 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x + 0.1, %y + 0.1, "7 7", %text, "Center", "0 1 0").schedule(%lifeSpan, "safeDelete");
	%this.addRoomNumFont(%x, %y + 0.1, %text, "7 7", "Center", "0 1 0").schedule(%lifeSpan, "safeDelete");	
}


//-----------------------------------------------------------------------------
// add boundaries on all sides of the Arena

function Arena::addArenaBoundaries(%this, %width, %height)
{

    // Calculate a width and height to use for the bounds.
    %wrapWidth = %width * 1.0;
    %wrapHeight = %height * 1.0;	//1.1

    %this.getScene().add( %this.createOneArenaBoundary( "left",   -%wrapWidth/2 SPC 0,  5 SPC %wrapHeight) );
    %this.getScene().add( %this.createOneArenaBoundary( "right",  %wrapWidth/2 SPC 0,   5 SPC %wrapHeight) );
    %this.getScene().add( %this.createOneArenaBoundary( "top",    0 SPC -%wrapHeight/2, %wrapWidth SPC 5 ) );
    %this.getScene().add( %this.createOneArenaBoundary( "bottom", 0 SPC %wrapHeight/2,  %wrapWidth SPC 5 ) );
}

//-----------------------------------------------------------------------------
// create boundary at specified location
function Arena::createOneArenaBoundary(%this, %side, %position, %size)
{
    %boundary = new SceneObject() { class = "ArenaBoundary"; };
    
    %boundary.setSize( %size );
    %boundary.side = %side;
    %boundary.setPosition( %position );
    %boundary.setSceneLayer( 1 );
    %boundary.createPolygonBoxCollisionShape( %size );
	
    // the objects that collide with us should handle any callbacks.
    // remember to set those scene objects to collide with scene group 15 (which is our group)!
    %boundary.setSceneGroup( Utility.getCollisionGroup("Wall") );
    %boundary.setCollisionCallback(false);
    %boundary.setBodyType( "static" );
    return %boundary;
}

//-----------------------------------------------------------------------------
// add a Player object to the Arena at specified location

function Arena::spawnPlayer(%this, %xPos, %yPos)
{
	%mainPlayer = new CompositeSprite()
	{
		class = "Player";
		myArena = %this;
	};
	
    %this.getScene().add( %mainPlayer );
	
	%mainPlayer.initialize();
	
	%mainPlayer.setPosition(%xPos, %yPos);

	return %mainPlayer;
} 

//-----------------------------------------------------------------------------
//break up Chromosome string and start spawning enemies according to sub-strings of the Chromosome
///ordering: armor/parry/acid/tar/blade/shooter/blob

function Arena::processRoomChromosomes(%this)
{
	echo("Chromosome:" SPC %chromosome);
	%chromosome = %this.currChromosome;
	
	for(%i = 0; %i < mFloor(getWordCount(%chromosome)/%this.toolVarietyCount); %i++)	//"for 0 through number of enemies"
	{
	
		%subChromosome = getWords(%chromosome, %i*%this.toolVarietyCount, (%this.toolVarietyCount - 1) + %i*%this.toolVarietyCount);	//get current group (7) of "words" (tool count/number)
		
		echo("  sub" SPC %subChromosome);
		
		echo("Arena.Arena: spawn enemy unit" SPC %i);

		%spawnLoc = %this.spawnPoints[%i];
		%this.spawnEnemyUnit(%subChromosome, getWord(%spawnLoc, 0), getWord(%spawnLoc, 1));	//spawn enemy at specified spawn location

		echo("Arena.Arena: spawned enemy unit successfuly" SPC %i);
		
		//collect subChromsomes back into a line-by-line list for post-room processing (no NewLine on last!!!)
		if(%i >= (getWordCount(%chromosome)/%this.toolVarietyCount) - 1)
		{
			%this.roomChromosomes = %this.roomChromosomes @ %subChromosome;
		}
		else
		{
			%this.roomChromosomes = %this.roomChromosomes @ %subChromosome NL "";
		}
	}
	
	echo("Chromosome done processing!");
}

//-----------------------------------------------------------------------------
//spawn enemy at specified spawn location

function Arena::spawnEnemyUnit(%this, %localChromosome, %xPos, %yPos)
{
	%newEnemy = new CompositeSprite()
	{
		class = "EnemyUnit";
		myChromosome = %localChromosome;
		myArena = %this;
		mainTarget = %this.player;
	};
	
    %this.getScene().add( %newEnemy );
	
	echo("Arena.Arena: initializing enemy:");
	%newEnemy.initialize();			//manual constructor function
	
	%newEnemy.setPosition(%xPos, %yPos);

	return %newEnemy;
} 

//-----------------------------------------------------------------------------
//returns random location in Arena that leaves the middle clear for player start

function Arena::findSpawnLocation(%this)
{
	%position = "0 0";
	//$roomWidth/3), getRandom(-$roomHeight
	
	%zoneFrac = 5;
	%noSpawnZon_pointA = -$roomWidth/%zoneFrac SPC $roomHeight/%zoneFrac;
	%noSpawnZon_pointB = $roomWidth/%zoneFrac SPC -$roomHeight/%zoneFrac;
	
	%widthBorder = $roomWidth/8;
	%heightBorder = $roomHeight/8;
	
	%quadrant = getRandom(0, 4);
	
	if(%quadrant < 1)
	{
		%position = getRandom(-$roomWidth/2+%widthBorder, getWord(%noSpawnZon_pointB, 0)) SPC getRandom(getWord(%noSpawnZon_pointA, 1), $roomHeight/2 - %heightBorder);
	}
	else if(%quadrant < 2)
	{
		%position = getRandom(getWord(%noSpawnZon_pointB, 0), $roomWidth/2 - %widthBorder) SPC getRandom(getWord(%noSpawnZon_pointB, 1), $roomHeight/2 - %heightBorder);
	}
	else if(%quadrant < 3)
	{
		%position = getRandom(getWord(%noSpawnZon_pointA, 0), $roomWidth/2 - %widthBorder) SPC getRandom(-$roomHeight/2 + %heightBorder, getWord(%noSpawnZon_pointB, 1));
	}
	else
	{
		%position = getRandom(-$roomWidth/2 + %widthBorder, getWord(%noSpawnZon_pointA, 0)) SPC getRandom(-$roomHeight/2 + %heightBorder, getWord(%noSpawnZon_pointA, 1));
	}

	return %position;
} 

//-----------------------------------------------------------------------------
//tell Manager that the player died. Pass enemy chromosome who killed player to display on defeat screen

function Arena::playerDied(%this, %murderer )
{
	%this.myManager.playerDies(%murderer.myChromosome, %murderer.maxBodySize);
} 

//-----------------------------------------------------------------------------

function Arena::finishRoom(%this)
{
	%this.myManager.endCurrentLevel();
} 