
//-----------------------------------------------------------------------------
//Copyright (c) 2015 RABBAT STUDIOS
//-----------------------------------------------------------------------------

function MainMenu::create( %this )
{    
	echo("MainMenu create");
    MainMenu.add( TamlRead("./gui/MenuDialog.gui.taml") );
    //Canvas.pushDialog(MenuDialog);

    // Adding another Dialog file.
    MainMenu.add( TamlRead("./gui/OptionsDialog.gui.taml") );
	
	//echo("Title music play: ");
	//$titleMusicHandle = alxPlay("GameAssets:mainMenuMusic");
	//echo("Title music play: " @ alxIsPlaying($titleMusicHandle));
	//echo("handle: " @ $titleMusicHandle);
	%this.openMainMenu();
	//echo("Title music play: " @ alxIsPlaying($titleMusicHandle));
	//echo("handle: " @ $titleMusicHandle);
}

function MainMenu::openMainMenu( %this)
{
	Canvas.pushDialog(MenuDialog);
	//if(!alxIsPlaying($titleMusicHandle))
	$titleMusicHandle = alxPlay("GameAssets:mainMenuMusic");
}

function MainMenu::destroy( %this )
{
	echo("MainMenu destroy");
}

// Adding command for NewGameButton.
function NewGameButton::onClick(%this)
{
    echo("NewGameButton click");
    //Canvas.popDialog(OptionsDialog);
	ModuleDatabase.unloadGroup("game");
	ModuleDatabase.loadGroup("game");
	alxStop($titleMusicHandle);
	//RoomManager::initialize();
	echo("End of NewGameButton click");
}

// Adding command for NewGameButton.
function OptionsButton::onClick(%this)
{
    Canvas.popDialog(MenuDialog);
    Canvas.pushDialog(OptionsDialog);
}

// Adding command for QuitButton.
function QuitButton::onClick(%this)
{
    quit();
}

// Adding command for StartGameButton.
function BackButton::onClick(%this)
{
    Canvas.popDialog(OptionsDialog);
	Canvas.pushDialog(MenuDialog);
}
