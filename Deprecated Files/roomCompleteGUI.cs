
function RoomCompleteGUI::onAdd(%this)
{
}

function RoomCompleteGUI::openScreen(%this, %scene)
{	
	echo("RoomManager.roomCompleteGUI: openScreen()");

	// Background
    %background = new Sprite();
    %background.setBodyType( "static" );
    %background.setImage( "GameAssets:menu_roomCompleteBkgrd" );
    %background.setSize( $roomWidth, $roomHeight );
    %background.setCollisionSuppress();
    %background.setAwake( false );
    %background.setActive( false );
    %background.setSceneLayer(30);
    %scene.add( %background );
	
	exec("./scripts/behaviors/menus/RoomCompleteControls.cs");
	
	%controls = RoomCompleteControls.createInstance();
	%controls.enterKey = "keyboard enter";
	%this.addBehavior(%controls);
}

function RoomCompleteGUI::deleteThis(%this)
{
	echo("RoomManager.roomCompleteGUI: deleteThis()");
	%this.clearBehaviors();
	%this.safeDelete();
}