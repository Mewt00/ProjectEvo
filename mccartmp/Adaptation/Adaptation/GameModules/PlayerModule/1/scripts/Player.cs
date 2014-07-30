//-----------------------------------------------------------------------------
// PlayerModule: player class and functions
//-----------------------------------------------------------------------------

function Player::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function Player::initialize(%this)
{
	exec("./playerBullet/PlayerBullet.cs");
	
	%this.setSceneGroup(5);			//0: Player sceneGroup
	%this.setSceneLayer(5);
	%this.fixedAngle = true;
	
	%this.sizeRatio = 0.8;
	
	%this.walkSpeed = 100;			// <-- f*cked (is there a max limit on linearVelocity?? of 130?)
	%this.health = 100;
	%this.setPosition(0, 25);
	
	%this.setupSprite();
	%this.setupControls();
	
    %this.createPolygonBoxCollisionShape(%this.getWidth(), %this.getHeight());
    %this.setCollisionShapeIsSensor(0, true);
    %this.setCollisionGroups( "10 15" );
	%this.setCollisionCallback(true);
}

//-----------------------------------------------------------------------------

function Player::setupSprite( %this )
{
	%this.addSprite("0 0");
	%this.setSpriteAnimation("GameAssets:playerbaseAnim", 0);
	%this.setSpriteName("BodyAnim");
	%this.setSpriteSize(159*%this.sizeRatio, 210*%this.sizeRatio);
}

//-----------------------------------------------------------------------------

function Player::setupControls( %this )
{
	exec("./behaviors/controls/PlayerMovementControls.cs");
	exec("./behaviors/controls/alignToJoystick.cs");
	
 	%controls = PlayerMovementControlsBehavior.createInstance();
	%controls.walkSpeed = %this.walkSpeed;
	%controls.upKey = "keyboard W";
	%controls.leftKey = "keyboard A";
	%controls.downKey = "keyboard S";
	%controls.rightKey = "keyboard D";
	%this.addBehavior(%controls);
	
/* 	%xBoxControl = AlignToJoystickBehavior.createInstance();
	%xBoxControl.xAxis = "joystick0 xaxis";
	%xBoxControl.yAxis = "joystick0 yaxis";
	%xBoxControl.rotationOffset = 90;
	%this.addBehavior(%xBoxControl); */
	
	
	exec("./behaviors/controls/faceMouse.cs");
	
	%faceMse = FaceMouseBehavior.createInstance();
	%faceMse.object = mainPlayer;
	%faceMse.rotationOffset = 180;
	%this.addBehavior(%faceMse);
}

//-----------------------------------------------------------------------------

function Player::destroy( %this )
{
}
