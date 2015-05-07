
//-----------------------------------------------------------------------------
//Copyright (c) 2015 Chubby Bunny, LLC
//-----------------------------------------------------------------------------


// Set log mode.
setLogMode(6);

// Set profiler.
//profilerEnable( true );

// Controls whether the execution or script files or compiled DSOs are echoed to the console or not.
// Being able to turn this off means far less spam in the console during typical development.
setScriptExecEcho( false );

// Controls whether all script execution is traced (echoed) to the console or not.
trace( false );

// Sets whether to ignore compiled TorqueScript files (DSOs) or not.
$Scripts::ignoreDSOs = true;

// The name of the company. Used to form the path to save preferences. Defaults to GarageGames
// if not specified.
// The name of the game. Used to form the path to save preferences. Defaults to C++ engine define TORQUE_GAME_NAME
// if not specified.
// Appending version string to avoid conflicts with existing versions and other versions.
setCompanyAndProduct("Chubby Bunny", "ProjectEvo" );

// Set module database information echo.
ModuleDatabase.EchoInfo = false;

// Set asset database information echo.
AssetDatabase.EchoInfo = false;

// Set the asset manager to ignore any auto-unload assets.
// This cases assets to stay in memory unless assets are purged.
AssetDatabase.IgnoreAutoUnload = true;

// Scan modules.
ModuleDatabase.scanModules( "GameModules" );

// Load AppCore module.
ModuleDatabase.LoadExplicit( "AppCore" );
//ModuleDatabase.LoadGroup( "game" );

//-----------------------------------------------------------------------------

function onExit()
{
    // Unload the AppCore module.
    ModuleDatabase.unloadExplicit( "AppCore" );
	//ModuleDatabase.unloadGroup( "game" );
}

//-----------------------------------------------------------------------------

//function androidBackButton(%val)
//{
//	if (%val) {
//		//Add code here for other options the back button can do like going back a screen.  the quit should happen at your main menu.

//		quit();
//	}

//}