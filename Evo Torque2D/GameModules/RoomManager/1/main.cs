//-----------------------------------------------------------------------------
// Room manager, sets up canvas and arena
//-----------------------------------------------------------------------------

//globals
//Ideal resolution - 1366 x 768
//  				 16:9
$roomWidth = 160;
$roomHeight = 90;
//How many pixels in one world unit(m)
$worldUnitsToPixels = 15;
$pixelsToWorldUnits = 1/$worldUnitsToPixels;
//$edgeWidth = 30 * $pixelsToWorldUnits;
//TODO Make it a ScriptObject?
//---------------------------------------------------------------------

function RoomManager::create( %this )
{   	
	//echo("handle: " @ $titleMusicHandle);
	
    // load some scripts and variables
    //exec("./scripts/arena.cs");
    //exec("./roomCompleteGUI.cs");
    exec("./roomDefeatGUI.cs");
	exec("./scripts/behaviors/controls/InGameMenuControls.cs");
	RoomManager.add( TamlRead("./gui/DefeatDialog.gui.taml") );
	
	enableXInput();
		$enableDirectInput = true;
		activateDirectInput();
	
	echo("Xinput:");
	echoInputState();
	
	//new ActionMap(inGameMenuActionMap);//? Why is this made here
	echo("exitMenu object? " @ isObject(%this.exitMenu));
	%this.menuControls = InGameMenuControlsBehavior.createInstance();
	echo("menuControls : " @ isObject(%this.menuControls));
	%this.exitMenu = new ScriptObject();
	echo("exitMenu object? " @ isObject(%this.exitMenu));
	%this.exitMenu.addBehavior(%this.menuControls);
	
	%this.initialize();

}

//---------------------------------------------------------------------

function RoomManager::initialize( %this)
{
	//echo("Object?:" @ isObject(%this));
	setRandomSeed(getRealTime());
	
	//Set up Arena Scene Window
	new SceneWindow(mainWindow){};
	mainWindow.profile = GuiDefaultProfile;
	Canvas.setContent(mainWindow);
	
	mainWindow.setCameraSize( $roomWidth, $roomHeight );
	
	// load some scripts and variables
    //exec("./scripts/arena.cs");
    //exec("./roomDefeatGUI.cs");

	
	//Lasting Variables
	%this.CurrentLevel = 0;
	%this.CurrentChromosome = 0;
		
	echo("RoomManager.main: Creating GeneticAlgorithm instance");
	//TODO Possible wasted memory, genAlg never destroyed but new GeneticAlgorithm object is assigned
	$genAlg = new GeneticAlgorithm();
	
	%this.startArena();
	
}
    
//-----------------------------------------------------------------------------
  
function RoomManager::startArena( %this )
{
	%this.CurrentLevel++;
	%this.currentArena = new SceneObject()
	{
		class = "Arena";
		myManager = %this;
		currLevel = %this.CurrentLevel;
		currChromosome = 
			"0 0 0 0 0 0 1" SPC "0 0 0 0 0 0 1";//%this.nextChromosome; //TODO next v current?
	};
	
	%arenaScene = new Scene();
	//%arenaScene.setDebugOn("collision");
	%arenaScene.layerSortMode0 = "Newest";
	%arenaScene.add(%this.currentArena);
	%this.currentArena.buildArena();
	
	mainWindow.setScene( %arenaScene );
	
}	
	
//-----------------------------------------------------------------------------
  
function RoomManager::startNextLevel( %this )
{
	echo("Start next level entered");
	%this.CurrentLevel++;
	
	//%this.runNextRoomGenAlg();
	
	%this.currentArena.currLevel = %this.CurrentLevel;
	//%this.currentArena.currChromosome = %this.nextChromosome;
	
	%this.currentArena.nextArenaWave();
	//%this.nextChromosome = %this.currentArena.currChromosome;
}

//-----------------------------------------------------------------------------

function RoomManager::endCurrentLevel( %this )
{
	echo("RoomManager.main: Room finished!");
	
	%this.writeRoomSummationFile();	
	
	$genAlg.run();

	/*//%this.currentArena.player.clearBehaviors();
	//%this.currentArena.getScene().remove(%this.currentArena.player);
	
	%this.currentArena.getScene().schedule(320, "clear");  
	%this.schedule(320, "goToRoomCompleteScreen"); */ 
	
	//%this.nextChromosome = %this.currentArena.currChromosome;
	
	//TODO Lag/Jumpiness around spawn
	//%this.nextChromosome = %this.runNextRoomGenAlg();
	//%this.runNextRoomGenAlg();
	//%this.schedule(32, "runNextRoomGenAlg");
	%this.schedule(320, "startNextLevel");
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
	//$genAlg.run();
	%chromosome = $genAlg.retrieveChromosome();			//call C++ code!
	
	echo("RoomManager.main: GeneticAlgorithm. run successful!");
	
	%this.nextChromosome = %chromosome;
	
	//return %chromosome;
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
  
function RoomManager::goToRoomDefeatScreen( %this, %furthestLevel, %killerChromosome, %killBodyRadius )
{	
	//%this.nextChromosome = %this.runNextRoomGenAlg();

	echo("Main goToDefeat" SPC %killerChromosome);
	
	
	
	%defeatRoomScene = new Scene();
	%defeatRoomScene.layerSortMode0 = "Newest";
	mainWindow.setScene( %defeatRoomScene );
	
	%defeatInfoScreen = new SceneObject()
	{
		class = "RoomDefeatGUI";
		myManager = %this;
		lastLevel = %furthestLevel;
		killerChromosome = %killerChromosome;
		killBodyRadius = %killBodyRadius;
	};
		 
	%defeatInfoScreen.openScreen(%defeatRoomScene);
	
	Canvas.pushDialog(DefeatDialog);
}   

//-----------------------------------------------------------------------------

function ContinueButton::onClick(%this)
{
    echo("Continue click");
	//mainWindow.delete();
	alxStopAll();
	Canvas.pushDialog(MenuDialog);
	$titleMusicHandle = alxPlay("GameAssets:mainMenuMusic");
	echo("End of ContinueButton click");
}

//-----------------------------------------------------------------------------

function RoomManager::exitGame(%this, %val)
{
	if(%val == 1)
	{
		/*//echo("RoomManager.exitGame()");
		//mainWindow.delete();
		//alxStop($roomMusicHandle);
		//Canvas.popDialog(mainWindow);
		//%this.currentArena.getScene().schedule(320, "clear");  
		//Canvas.removeContent(mainWindow);
		//mainWindow.schedule(320, "clear");
		//Canvas.pushDialog("MenuDialog");
		//MainMenu::openMainMenu();
		//Canvas.pushDialog(MenuDialog);
		//echo("handle: " @ $titleMusicHandle);
		//if(!alxIsPlaying($titleMusicHandle))
		//$titleMusicHandle = alxPlay("GameAssets:mainMenuMusic");
		//echo("Unload now");
		//ModuleDatabase.schedule(32, "unloadGroup", "game");*/
	}
}

//-----------------------------------------------------------------------------

function RoomManager::destroy(%this)
{
	echo("ingame object?: " @ isObject(inGameMenuActionMap));
	echo("exitMenu object? " @ isObject(%this.exitMenu));
	echo("exitMenu object? " @ isObject(%this.menuControls));
	echo("before behavior removed by clearBehaviors");
	%this.exitMenu.clearBehaviors();
	%this.exitMenu.delete();
	echo("ingame object?: " @ isObject(inGameMenuActionMap));
	echo("exitMenu object? " @ isObject(%this.exitMenu));
	if(isObject(%this.currentArena))
		%this.currentArena.getScene().clear();
	//%this.currentArena.safeDelete();
	//echo("ingame object?: " @ isObject(inGameMenuActionMap));
	echo("Reached RoomManager destroy");
}

	