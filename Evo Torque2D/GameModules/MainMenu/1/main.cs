

function MainMenu::create( %this )
{    
    //MainMenu.add( TamlRead("./gui/MenuDialog.gui.taml") );
    //Canvas.pushDialog(MenuDialog);

    // Adding another Dialog file.
    //MainMenu.add( TamlRead("./gui/OptionsDialog.gui.taml") );
	
	
}

function MainMenu::destroy( %this )
{
}

// Adding command for NewGameButton.
function NewGameButton::onClick(%this)
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
function StartGameButton::onClick(%this)
{
    // This group doesn't exist, just here for example.
    //We could load up a group this way to start the game.
    ModuleDatabase.loadGroup("gameStartGroup");
    Canvas.popDialog(OptionsDialog);
}

// Adding command for MainMenuButton.
function MainMenuButton::onClick(%this)
{
    Canvas.popDialog(OptionsDialog);
    Canvas.pushDialog(MenuDialog);
}