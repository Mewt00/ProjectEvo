//-----------------------------------------------------------------------------
// DamageSplashScreen: red blood splatter around screen edges
//-----------------------------------------------------------------------------

function DamageSplashScreen::onAdd( %this )
{
	%this.initialize();
}

//-----------------------------------------------------------------------------

function DamageSplashScreen::initialize(%this)
{
	%this.setupSprite();
	%this.setUpdateCallback(true);
	%this.setActive( false );
	%this.setCollisionSuppress();
	%this.setSceneGroup( Utility.getCollisionGroup("") );
	
	%this.lifeSpan = %this.initDamage*15 + 100;		//length of blood splat based on damages taken
	%this.dmgSplashSchedule = schedule(%this.lifeSpan, 0, "DamageSplashScreen::damageSplashRemove", %this);
}
//-----------------------------------------------------------------------------

function DamageSplashScreen::setupSprite( %this )
{
	%this.setImage( "GameAssets:damageScreenSplash" );
	%this.setSize( $roomWidth, $roomHeight );
	%this.setSceneLayer(2);
}

//-----------------------------------------------------------------------------
//Fade with time

function DamageSplashScreen::onUpdate(%this)
{
	%this.setBlendAlpha(getEventTimeLeft(%this.dmgSplashSchedule) / %this.lifeSpan);
}

//-----------------------------------------------------------------------------

function DamageSplashScreen::damageSplashRemove( %this )
{
	%this.safeDelete();
}

//-----------------------------------------------------------------------------

function DamageSplashScreen::onRemove( %this )
{
}
