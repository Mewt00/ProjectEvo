
function TitleScreen::onAdd(%this)
{
}

function TitleScreen::openTitleScreen(%this, %scene)
{
	// Background
    %background = new Sprite();
    %background.setBodyType( "static" );
    %background.setImage( "GameAssets:menu_titleScreenBkgrd" );
    %background.setSize( $roomWidth, $roomHeight );
    %background.setCollisionSuppress();
    %background.setAwake( false );
    %background.setActive( false );
    %background.setSceneLayer(30);
    %scene.add( %background );
	
	exec("./scripts/behaviors/menus/MenuControls.cs");
	
	%controls = MenuControlBehavior.createInstance();
	%controls.enterKey = "keyboard enter";
	%this.addBehavior(%controls);
}