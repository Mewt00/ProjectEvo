//-----------------------------------------------------------------------------
// Room manager, sets up canvas and arena
//-----------------------------------------------------------------------------


//globals
//$roomWidth = 1280;
//$roomHeight = 960;
$roomWidth = 160;
$roomHeight = 120;
$pixelToWorldRatio = $roomWidth/1600;

//---------------------------------------------------------------------

function RoomManager::create( %this )
{   	
	setRandomSeed(getRealTime());
	
	//OpenALInitDriver();							//for audio
	

    new SceneWindow(mainWindow)
	{
		//useWindowMouseEvents = "1";
	};
	
    mainWindow.profile = new GuiControlProfile();
    Canvas.setContent(mainWindow);
	
	//new ScriptObject(InputManager);
	//mainWindow.addInputListener(InputManager);

    //mainWindow.setCameraPosition( 0, 0 );
	//echo("main scene" @ isObject(mainScene));
	//mainScene.layerSortMode0 = "Newest"; doesn't exist yet?
	
    mainWindow.setCameraSize( $roomWidth, $roomHeight );
    //mainWindow.setCameraSize( $roomWidth*1.2, $roomHeight*1.2 );	//zoomed out cam
	
    // load some scripts and variables
    //exec("./scripts/arena.cs");
    exec("./titleScreenGUI.cs");
    exec("./roomCompleteGUI.cs");
    exec("./roomDefeatGUI.cs");
    exec("./scripts/behaviors/menus/GlobalControls.cs");
	
	enableXInput();
		$enableDirectInput = true;
		activateDirectInput();
	
	echo("Xinput:");
	echoInputState();
	
    GlobalActionMap.bindObj("keyboard", "Escape", "exitGame", %this);
	//GlobalActionMap.bindObj("keyboard", "M", $pref::Video::fullscreen ^= !$pref::Video::fullScreen, %this);
	
	%this.goToTitleScreen( );
	
	
	//Lasting Variables
	%this.CurrentLevel = 0;
	%this.CurrentChromosome = 0;
	
	
	echo("RoomManager.main: Creating GeneticAlgorithm instance");
	$genAlg = new GeneticAlgorithm();
}
    
//-----------------------------------------------------------------------------

//function XInput::connect (%this) {
//	echo("Connect hit");
//}

//-----------------------------------------------------------------------------
  
function RoomManager::goToTitleScreen( %this )
{
	%this.CurrentLevel = 0;
	%this.addTitleMusic();

    new Scene(mainScene)
	{
		//class="defualtWindow";
	};
    mainWindow.setScene(mainScene);
	
	%gui_titleScreen = new SceneObject()
	{
		class = "TitleScreenGUI";
		myManager = %this;
	};
		 
	%gui_titleScreen.openTitleScreen(mainScene);
}

//-----------------------------------------------------------------------------

function RoomManager::addTitleMusic(%this)
{
	%musicAsset = "GameAssets:mainMenuMusic";
	
	$musicHandle = alxPlay(%musicAsset);	
	
	//%this.schedule(alxGetAudioLength(%musicAsset), "addTitleMusic");
}
    
//-----------------------------------------------------------------------------
  
function RoomManager::startNextLevel( %this )
{
	%this.CurrentLevel++;
	
	%gameArena = new SceneObject()
	{
		class = "Arena";
		myManager = %this;
		currLevel = %this.CurrentLevel;
		currChromosome = %this.nextChromosome;
	};
	
	%arenaScene = new Scene();
	//%arenaScene.setDebugOn("collision");
	%arenaScene.layerSortMode0 = "Newest";
	%arenaScene.add(%gameArena);
	%gameArena.buildArena( );
	
	mainWindow.setScene( %arenaScene );
	
	%this.currentArena = %gameArena;
}

//-----------------------------------------------------------------------------

function RoomManager::endCurrentLevel( %this )
{
	echo("RoomManager.main: Room finished!");
	
	%this.writeRoomSummationFile();	

	//%this.currentArena.player.clearBehaviors();
	//%this.currentArena.getScene().remove(%this.currentArena.player);
	
	//TODO: Fix the clearing of the Player object
	%this.currentArena.getScene().schedule(320, "clear");  
	%this.schedule(320, "goToRoomCompleteScreen");  
}

//-----------------------------------------------------------------------------
//Writes a file that is to be decoded by the evolution engine

function RoomManager::writeRoomSummationFile( %this )
{
	%plyrRangeCount = %this.currentArena.player.rangedCount;
	%plyrMeleeCount = %this.currentArena.player.meleeCount;
	%plyrBlockCount = %this.currentArena.player.blockCount;
	%plyrDashCount = %this.currentArena.player.dashCount;
	
	
	echo("RoomManager.main: Results:");
	echo(%plyrRangeCount);
	echo(%plyrMeleeCount);
	echo(%plyrBlockCount);
	echo(%plyrDashCount);
	echo("");
	
	%totalCount = %plyrRangeCount + %plyrMeleeCount + %plyrBlockCount + %plyrDashCount;
					
	if(%totalCount > 0)
	{	
		%plyrRangeCount = %plyrRangeCount/%totalCount;
		%plyrMeleeCount = %plyrMeleeCount/%totalCount;
		%plyrBlockCount = %plyrBlockCount/%totalCount;
		%plyrDashCount = %plyrDashCount/%totalCount;
	}
	
	echo("Result Percents:");
	echo(%plyrRangeCount);
	echo(%plyrMeleeCount);
	echo(%plyrBlockCount);
	echo(%plyrDashCount);
	echo(" total:" SPC %totalCount);
	
	echo("RoomManager.main: chromosome" SPC (%this.currentArena.roomChromosomes));
	
	%file = new FileObject();
	
	
	if(%file.openForWrite("utilities/ga_input.txt"))
	{
		echo("RoomManager.main: write file opened");
	
		if(%this.currentArena.roomShooterShotsFired > 0)
			%averageRangedDamage = (%this.currentArena.roomShooterDamage/%this.currentArena.roomShooterShotsFired);
		else
			%averageRangedDamage = 0;
		
		if(%this.currentArena.roomBladeAttackNums > 0)
			%averageMeleeDamage = (%this.currentArena.roomBladeDamage/%this.currentArena.roomBladeAttackNums);
		else
			%averageMeleeDamage = 0;
			
		echo("Room Avg. Melee:" SPC %averageMeleeDamage);
		echo("Room Avg. Range:" SPC %averageRangedDamage);
		
		%file.writeLine((5 + %this.CurrentLevel*2) @ "");
		%file.writeLine("");
		%file.writeLine(%plyrRangeCount @ "");
		%file.writeLine(%plyrMeleeCount @ "");
		%file.writeLine(%plyrBlockCount @ "");
		%file.writeLine(%plyrDashCount @ "");
		%file.writeLine(%averageMeleeDamage);					//enemy dps melee
		%file.writeLine(%averageRangedDamage);					//enemy dps range
		%file.writeLine("");
		%file.writeLine("");
		%file.writeLine(%this.currentArena.roomChromosomes);	//enemy subChromosomes (1/line)
		
		echo(%this.currentArena.roomChromosomes);
		echo("RoomManager.main: Summation file Written");
	}
	else
	{
		error("RoomManager.main: Summation file is not open for writing");
	}
	%file.close();
	echo("RoomManager.main: write file closed");
	
}   
 
//-----------------------------------------------------------------------------
  
function RoomManager::runNextRoomGenAlg( %this )
{
	echo("RoomManager.main: GeneticAlgorithm.run()");
	%chromosome = $genAlg.run();			//call C++ code!
	
	echo("RoomManager.main: GeneticAlgorithm. run successful!");
	
	return %chromosome;
}
 
//-----------------------------------------------------------------------------
  
function RoomManager::goToRoomCompleteScreen( %this )
{	
	%this.nextChromosome = %this.runNextRoomGenAlg();

	%completeRoomScene = new Scene();
	%completeRoomScene.layerSortMode0 = "Newest";
	mainWindow.setScene( %completeRoomScene );
	
	%gui_roomCompleteScreen = new SceneObject()
	{
		class = "RoomCompleteGUI";
		myManager = %this;
	};
		 
	%gui_roomCompleteScreen.openScreen(%completeRoomScene);
}   

//-----------------------------------------------------------------------------

function RoomManager::endRoomTitleScreen( %this )
{	
	alxStopAll();
	%this.startNextLevel();
}
 
//-----------------------------------------------------------------------------
  
function RoomManager::endRoomCompleteScreen( %this )
{	
	%this.startNextLevel();
}
 
//-----------------------------------------------------------------------------
  
function RoomManager::goToRoomDefeatScreen( %this, %furthestLevel, %killerChromosome, %killBodyRadius )
{	
	//%this.nextChromosome = %this.runNextRoomGenAlg();

	echo("Main goToDefeat" SPC %killerChromosome);
	
	%defeatRoomScene = new Scene();
	%defeatRoomScene.layerSortMode0 = "Newest";
	mainWindow.setScene( %defeatRoomScene );
	
	%gui_roomDefeatScreen = new SceneObject()
	{
		class = "RoomDefeatGUI";
		myManager = %this;
		lastLevel = %furthestLevel;
		killerChromosome = %killerChromosome;
		killBodyRadius = %killBodyRadius;
	};
		 
	%gui_roomDefeatScreen.openScreen(%defeatRoomScene);
}   

//-----------------------------------------------------------------------------

function RoomManager::playerDies( %this, %killerChromosome, %killBodyRadius )
{	
	alxStopAll();
	%this.currentArena.getScene().schedule(320, "clear");  
	%this.schedule(320, "goToRoomDefeatScreen", %this.CurrentLevel, %killerChromosome, %killBodyRadius);  
	//%this.goToRoomDefeatScreen(%killerChromosome, %killBodyRadius);
	%this.CurrentLevel = 0;
}

//-----------------------------------------------------------------------------

function RoomManager::exitGame(%this, %val)
{
	if(%val == 1)
	{
		echo("RoomManager.exitGame()");
		quit();
	}
}

//-----------------------------------------------------------------------------

function RoomManager::destroy(%this)
{
	echo("rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrReached RoomManager destroy and AL Driver");
	OpenALShutdownDriver();
	echo("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
}

	