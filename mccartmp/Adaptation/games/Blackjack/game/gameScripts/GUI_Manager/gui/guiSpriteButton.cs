//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Sprite Button Script Class
//-----------------------------------------------------------------------------
//
// This class inherits button properties from ButtonBase and implements
// a simple button that changes frames based on mouse events.
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// T2D Sprite Button - Example Use
//-----------------------------------------------------------------------------


//%exSpriteButton = new t2dSceneObject() {
   //scenegraph   = %exScenegraph;
   //class        = SpriteButton;
   //superClass   = ButtonBase;
   //position     = "0 0";
   //size         = "4 4";
   //text           = "";
   //overlayImage   = exArrowIconsImageMap;
   //imageIndex     = 0;
   //hoverImageIndex= 1;
   //clickImageIndex= 2;
   //baseImage      = "";
   //noAnimate      = false;
   //immediateEval  = true;
   //layer          = 9;
   //
   //command        = "echo(\"Sprite Button clicked!\")";
//};


function SpriteButton::onAdd(%this)
{
   Parent::onAdd(%this);

   //sound defaults
   //if( %this.soundMouseOver $= "" )
   //   %this.soundMouseOver = soundMenuAccept;
   //
   //if( %this.soundMouseClick $= "" )
   //   %this.soundMouseClick = soundMenuClick;

}

function SpriteButton::onRemove(%this)
{
   Parent::onRemove(%this);
}

function SpriteButton::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.disabled != true )
   {
      // Set back to default image
      %this.overlay.setImageMap( %this.overlayImage, ( ( %this.imageindex !$= "" ) ? %this.imageIndex : 0 ) );

      // Eval Command, if any.
      if( %this.command !$= "" )
         eval( %this.command );
   }   
}

function SpriteButton::onMouseDown(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.disabled != true )
   {
      // Play click sound, if any.
      if( %this.soundMouseClick !$= "" && isObject(%this.soundMouseClick) )
         alxPlaySound( %this.soundMouseClick, 0.95 );

      // Set back to default image
      %this.overlay.setImageMap( %this.overlayImage, ( ( %this.clickImageindex !$= "" ) ? %this.clickImageindex : 0 ) );
   }   
}


function SpriteButton::onMouseEnter(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.disabled != true )
   {
      // Eval onMouseEnter command, if any.
      if( %this.onMouseEnter !$= "" )
         eval(%this.onMouseEnter );

      // Play on mouse over sound, if any.
      if( %this.soundMouseOver !$= ""  && isObject(%this.soundMouseOver) )
         alxPlaySound(%this.soundMouseOver, 0.85 );

      // Set hover image, if any.
      %this.overlay.setImageMap( %this.overlayImage, ( ( %this.hoverImageIndex !$= "" ) ? %this.hoverImageIndex : 0 ) );
   }
}

function SpriteButton::onMouseLeave(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.disabled != true )
   {
      // Eval onMouseLeave command, if any.
      if( %this.onMouseLeave !$= "" )
         eval(%this.onMouseLeave );

      // Set to normal state
      %this.overlay.setImageMap( %this.overlayImage, ( ( %this.imageindex !$= "" ) ? %this.imageIndex : 0 ) );
   }
}


function SpriteButton::setSize( %this, %size )
{
   Parent::setSize( %this, %size );
   %this.base.setSize( %size );
   %this.overlay.setSize( %size );
}

function SpriteButton::setSizeX( %this, %size )
{
   Parent::setSizeX( %this, %size );
   %this.base.setSizeX( %size );
   %this.overlay.setSizeX( %size );
}

function SpriteButton::setSizeY( %this, %size )
{
   Parent::setSizeY( %this, %size );
   %this.base.setSizeY( %size );
   %this.overlay.setSizeY( %size );
}