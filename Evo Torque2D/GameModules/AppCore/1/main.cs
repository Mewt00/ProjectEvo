
//-----------------------------------------------------------------------------
//Copyright (c) 2015 Chubby Bunny, LLC
//-----------------------------------------------------------------------------


function AppCore::create( %this )
{
    // Load system scripts
    //exec("./scripts/constants.cs");
    exec("./scripts/defaultPreferences.cs");
    exec("./scripts/canvas.cs");
    exec("./scripts/openal.cs");
    
    // Initialize the canvas
    initializeCanvas("Project Evo");
    
    // Set the canvas color
    Canvas.BackgroundColor = "DarkMagenta";
    Canvas.UseBackgroundColor = true;
    
    // Initialize audio
    initializeOpenAL();
    
    ModuleDatabase.loadGroup("game");
}

//-----------------------------------------------------------------------------

function AppCore::destroy( %this )
{
	/*TODO unload Modules order
	echo("rrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrrr");
	ModuleDatabase.unloadGroup("game");
	echo("After");
	*/

}

