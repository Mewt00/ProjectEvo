
function RoomDefeatGUI::onAdd(%this)
{
}

function RoomDefeatGUI::openScreen(%this, %scene)
{	
	echo("RoomManager.RoomDefeatGUI: openScreen()");

	// Background
    %background = new Sprite();
    %background.setBodyType( "static" );
    %background.setImage( "GameAssets:menu_roomDefeatBkgrd" );
    %background.setSize( $roomWidth, $roomHeight );
    %background.setCollisionSuppress();
    %background.setAwake( false );
    %background.setActive( false );
    %background.setSceneLayer(30);
    %scene.add( %background );
	%this.myScene = %scene;
	
	exec("./scripts/behaviors/menus/RoomDefeatControls.cs");
	
	%controls = RoomDefeatControls.createInstance();
	%controls.enterKey = "keyboard enter";
	%this.addBehavior(%controls);
	
	%this.addRoomFont(-$roomWidth*0.2375, $roomHeight*0.0996, %this.lastLevel);
	%this.addKillerEnemy(-$roomWidth*0.2375, -$roomHeight*0.163);
}

//-----------------------------------------------------------------------------

function RoomDefeatGUI::addRoomFont(%this, %x, %y, %text)
{
	%this.addRoomNumFont(%x, %y, %text);
	%this.addRoomNumFont(%x + 0.1, %y, %text);
	%this.addRoomNumFont(%x + 0.1, %y + 0.1, %text);
	%this.addRoomNumFont(%x, %y + 0.1, %text);
}

//-----------------------------------------------------------------------------

function RoomDefeatGUI::addRoomNumFont(%this, %x, %y, %text)
{
	%font = new ImageFont();
	%font.Image = "GameAssets:font";
	%font.Text = %text;
	%font.FontSize = "4 4";
	%font.setBlendColor(1, 0, 0);
	%font.setPosition(%x, %y);
	%font.TextAlignment = "Left";
	%this.myScene.add( %font ); 
}

//-----------------------------------------------------------------------------

function RoomDefeatGUI::addKillerEnemy(%this, %x, %y)
{
	%this.spawnEnemyUnit(%this.killerChromosome, %x + (1+%this.killBodyRadius)*64*$pixelToWorldRatio, %y);
	%this.myScene.add( %font ); 
}

//-----------------------------------------------------------------------------

function RoomDefeatGUI::spawnEnemyUnit(%this, %localChromosome, %xPos, %yPos)
{
	echo("RoomManager.RoomDefeatGUI: spawn:" SPC %localChromosome);

    // add a Player object to the Arena
	%newEnemy = new CompositeSprite()
	{
		class = "EnemyUnit";
		myChromosome = %localChromosome;
		noBehaviors = 1;
	};
	
    %this.myScene.add( %newEnemy );
	%newEnemy.initialize();
	%newEnemy.setPosition(%xPos, %yPos);
	
	%newEnemy.myHealthbar.safeDelete();
	%newEnemy.clearBehaviors();
	%newEnemy.setAngle(0);
	//%newEnemy.setEnabled(false);
	//%newEnemy.setAwake(false);
	//%newEnemy.setActive(false);
	
	
} 
		
//-----------------------------------------------------------------------------

function RoomDefeatGUI::deleteThis(%this)
{
	echo("RoomManager.RoomDefeatGUI: deleteThis()");
	%this.clearBehaviors();
	%this.safeDelete();
}