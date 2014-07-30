//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Animated Button Script Class
//-----------------------------------------------------------------------------
//
// This class inherits from ButtonBase to create a button that can animate
// on mouse events.
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Animated Button - Example Use
//-----------------------------------------------------------------------------

//%exampleAnimBtn = new t2dSceneObject() {
      //
      //scenegraph = %Ascenegraph;
      //class = AnimatedButton;
      //superClass = ButtonBase;
      //
      //position = "0 0";
      //size = "12 9";
      //
      //baseImage = myButtonImageMap;
      //overlayImage = myButtonOverlayImageMap;
      //
      //fontName = "Arial";
      //fontColor = "1 1 1 1";
      //fontOffset = "0 0";
      //fontSize = "6";
      //text = "Example!";
      //
      //command = "echo(\"Button Clicked!\");";
//};
   


function AnimatedButton::onAdd(%this)
{
   
   Parent::onAdd(%this);

   // Store original (base) size, for returning to after animating stuff
   %this.baseSize = %this.size;

   //click sounds defaults
   /*if( %this.soundMouseOver $= "" )
      %this.soundMouseOver = soundMenuAccept;

   if( %this.soundMouseClick $= "" )
      %this.soundMouseClick = soundMenuClick;
   */
}

function AnimatedButton::onRemove(%this)
{
   Parent::onRemove(%this);
}

function AnimatedButton::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   
       
   if( %this.noAnimate )
   {
      eval( %this.command );
      return;
   }
   if( %this.disabled != true )
   {
      if( %this.soundMouseClick !$= "" && isObject(%this.soundMouseClick) )
         alxPlaySound( %this.soundMouseClick, 0.95 );

      if( %this.immediateEval == true )
      {
         AnimationManager.playAnimation( %this, "buttonWiggleAnim:position", $ANIM_MODE_REL );
         schedule( 50, 0, "eval", %this.command );
      }
      else
         AnimationManager.playAnimation( %this, "buttonWiggleAnim:rotation", $ANIM_MODE_REL, "eval(" @ %this @ ".command);");
   }   
}

function AnimatedButton::onMouseEnter(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.disabled != true && !%this.noAnimate )
   {
      if( %this.onMouseEnter !$= "" )
         eval(%this.onMouseEnter );
      if( %this.soundMouseOver !$= ""  && isObject(%this.soundMouseOver) )
         alxPlaySound(%this.soundMouseOver, 0.85 );
      %this.isHovering = true;
      if( isObject(%this.effect) && !%this.noEffect)
      {
         %this.effect.stopEffect();
         %this.effect.playEffect();
      }

      
      AnimationManager.playAnimation( %this, "buttonOverAnim:size", $ANIM_MODE_REL, %this @ ".isHovering=false;" );
   }
}

function AnimatedButton::onMouseLeave(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( /*%this.disabled != true && */!%this.noAnimate )
   {

      if( %this.onMouseLeave !$= "" )
         eval(%this.onMouseLeave );

      if( %this.isHovering == true )
      {
         AnimationManager.stopAnimations( %this );
         %this.isHovering = false;
      }
      %keyFrame = "200:" @ %this.baseSize @ ":0:100";
      AnimationManager.playAnimationFrame(%this, %keyFrame, "size", $ANIM_MODE_ABS );
   }
}


function AnimatedButton::setSize( %this, %size )
{
   Parent::setSize( %this, %size );
   %this.base.setSize( %size );
   %this.overlay.setSize( %size );
}

function AnimatedButton::setSizeX( %this, %size )
{
   Parent::setSizeX( %this, %size );
   %this.base.setSizeX( %size );
   %this.overlay.setSizeX( %size );
}

function AnimatedButton::setSizeY( %this, %size )
{
   Parent::setSizeY( %this, %size );
   %this.base.setSizeY( %size );
   %this.overlay.setSizeY( %size );
}