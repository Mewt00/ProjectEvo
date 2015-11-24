
//-----------------------------------------------------------------------------
//Copyright (c) 2015 RABBAT STUDIOS
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
    
    ModuleDatabase.loadExplicit("MainMenu");
	ModuleDatabase.loadExplicit("Console");
	echo("Appcore create done");
}

//-----------------------------------------------------------------------------

function AppCore::destroy( %this )
{
	
	OpenALShutdownDriver();
	ModuleDatabase.unloadExplicit("MainMenu");
	ModuleDatabase.unloadExplicit("Console");

}

