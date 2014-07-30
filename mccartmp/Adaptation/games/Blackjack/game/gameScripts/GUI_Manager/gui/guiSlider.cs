//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//
//-----------------------------------------------------------------------------
// Slider Script Class
//-----------------------------------------------------------------------------
//
// This class implements a slider style script class that may be used for GUI
// elements that are animatable in T2D.
//
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// T2D Gui Slider - Example Use
//-----------------------------------------------------------------------------

//%MySlider = new t2dSceneObject() 
//{
//   scenegraph              = %mySceneGraph;
//   class                   = Slider;
//   
//   position                = "0 0";
//   size                    = "10 5";
//
//   value                   = "0.3";
//
//   //-----------------------------------------------------------------------------
//   // Image Display Attributes
//   //-----------------------------------------------------------------------------
//   
//   // Image map to use for check/unchecked states
//   baseImage                = myCheckBoxSprite;
//
//   //-----------------------------------------------------------------------------
//   // Thumb Image Attributes
//   //-----------------------------------------------------------------------------
//   
//   // Image for the sliding part itself
//   thumbImage            = sliderThumbImageMap;
//   // The size of the slidable part
//   thumbSize          = "1 3";
//};


//-----------------------------------------------------------------------------
// Action Methods
//-----------------------------------------------------------------------------
function Slider::setValue( %this, %value )
{
   // Sanity Check
   if( %value < 0 || %value > 1 ) 
   {
      error("Slider::SetValue - Value out of range (" SPC %value SPC ") - Value must be between 0 and 1");
      return;
   }

   // Ok the value is valid, but if it's the same, 
   // we don't update anything or notify the user of change.
   if( %this.value == %value )
      return;

   // Set new value
   %oldValue = %this.value;
   %this.value = %value;

   // Update View
   %this.updateView();

   // User Callback
   %this.OnValueChange( %this.value, %oldValue );
}

function Slider::getValue( %this )
{
   return %this.value;
}

function Slider::updateView( %this )
{
   %this.sliderThumb.mount( %this, %this.getThumbMount() );
}

//-----------------------------------------------------------------------------
// State Callbacks
//-----------------------------------------------------------------------------
function Slider::OnValueChange( %this, %newValue, %oldValue )
{
   //note that the command can access %newValue and %oldValue
   if( %this.command !$= "")
      eval(%this.command);
}

//-----------------------------------------------------------------------------
// Construction/Destruction
//-----------------------------------------------------------------------------
function Slider::onAdd(%this)
{
   
   if( %this.size !$= "" ) 
      %this.setSize( %this.size );
    
   // Create the slider base image
   %this.sliderBase = new t2dStaticSprite() { scenegraph  = %this.getSceneGraph(); class = "SliderBaseMouseEvents"; };
   %this.sliderBase.setSize( %this.getSize() );
   %this.sliderBase.setImageMap( %this.baseImage, 0 );
   %this.sliderBase.mount( %this, "0 0" );
   %this.sliderBase.sliderObj = %this; 
   
   // Allow our slider base to receive mouse events
   %this.sliderBase.setUseMouseEvents(true);

   // Set value before we create the thumb so that we can position it properly.
   %this.value = (%this.value !$= "") ? %this.value : "0.5";

   // Create the slider base image
   %this.sliderThumb = new t2dStaticSprite() { scenegraph  = %this.getSceneGraph(); class = "SliderThumbMouseEvents"; };
   %this.sliderThumb.setSize( %this.thumbSize );
   %this.sliderThumb.setImageMap( %this.thumbImage, 0 );
   %this.sliderThumb.mount( %this, %this.getThumbMount() );
   %this.sliderThumb.sliderObj = %this; 
   
   // Allow our slider base to receive mouse events
   %this.sliderThumb.setUseMouseEvents(true);

   // Default to not depressed state
   %this.depressed = false;
}

function Slider::getThumbMount( %this )
{
   // If we've got no value, return leftmost (0 value)
   if ( %this.value $= "" )
      return "-1 0";

   // Get position and size cenetered in scene   
   %posX    = %this.getPositionX();       // Get Position X
   %sizeX   = %this.getSizeX();           // Get Size X

   // Calculate as a stand 'rect' style
   %posX    = %posX - ( %sizeX / 2.0 );   // Calculate Leftmost scene point
   %extX    = %posX + %sizeX;             // Calculate Rightmost scene point

   // Before we take the width inverse and finish calculations we have to have a positive point 
   // or else we will end up doubling to a twice negative number and mount will bork!
   // Gah this is stupid :(
   if( %posX < 0 )
   {
      %extX += ( %posX * -1 );
      %posX = 0;
   }

   // We need to return this point in -1 to 1 so we can mount the thumb, so do some stupid math
   %widthInverse = 1.0 / ( %extX - %posX );

   // Value PosX
   %valPosX = %this.value * ( %extX - %posX );

   // Return our -1 to 1 value
   return ( %valPosX * %widthInverse * 2.0 - 1.0 ) SPC "0";

}


function Slider::getMouseValue( %this, %worldPosition )
{
   // Get position and size cenetered in scene   
   %posX    = %this.getPositionX();       // Get Position X
   %sizeX   = %this.getSizeX();           // Get Size X

   // Calculate as a standard 'rect' style
   %posX    = %posX - ( %sizeX / 2.0 );   // Calculate Leftmost scene point
   %extX    = %posX + %sizeX;             // Calculate Rightmost scene point

   %valPosX = %worldPosition - %posX;

   %absX = mAbs( %extX - %posX );

   // Return our 0 to 1 value
   return ( %valPosX / %absX );

}


function Slider::onRemove(%this)
{
   if( isObject( %this.sliderThumb ) )
      %this.sliderThumb.safeDelete();

   if( isObject( %this.sliderBase ) )
      %this.sliderBase.safeDelete();
}

//-----------------------------------------------------------------------------
// Mouse Events (Slider Base)
//-----------------------------------------------------------------------------
function SliderBaseMouseEvents::onMouseDown(%this, %modifier, %worldPosition, %mouseClicks)
{
   %this.sliderObj.depressed = true;

   %newValue = %this.sliderObj.getMouseValue( %worldPosition );
   
   if( %newValue > 0.0 && %newValue < 1.0 )
      %this.sliderObj.setValue ( %newValue );
}

function SliderBaseMouseEvents::onMouseUp(%this, %modifier, %worldPosition, %mouseClicks)
{
   %this.sliderObj.depressed = false;
   
}


function SliderBaseMouseEvents::onMouseDragged(%this, %modifier, %worldPosition, %mouseClicks)
{
   if( %this.sliderObj.depressed )
   {
      %newValue = %this.sliderObj.getMouseValue( %worldPosition ); 
      if( %newValue > 0.0 && %newValue < 1.0 )
         %this.sliderObj.setValue ( %newValue );
   }
}



//-----------------------------------------------------------------------------
// Mouse Events (Slider Thumb)
//-----------------------------------------------------------------------------
//function SliderThumbMouseEvents::onMouseEnter(%this, %modifier, %worldPosition, %mouseClicks)
//{
//   %this.setImageMap( %this.sliderObj.thumbImage, %this.sliderObj.thumbHoverImageIndex );
//}
//
//function SliderThumbMouseEvents::onMouseLeave(%this, %modifier, %worldPosition, %mouseClicks)
//{
//   %this.setImageMap( %this.sliderObj.thumbImage, %this.sliderObj.thumbImageIndex );
//}
