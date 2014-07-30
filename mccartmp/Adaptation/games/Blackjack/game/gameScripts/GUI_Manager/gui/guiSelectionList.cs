//-----------------------------------------------------------------------------
// GarageGames Billiards
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------


//-----------------------------------------------------------------------------
// T2D Gui Selection List Class - Example Use
//-----------------------------------------------------------------------------
//
// While this class may be used in this form, it is intended to be a pure virtual
// class that you derive your own specifics classes from.  It creates the basic
// functionality needed by a next/prev selection list, but has no item information
// display functionality.  
//
// See GuiSelectionListText for a real-world example
//
//-----------------------------------------------------------------------------
//
//%MySelectionList = new t2dSceneObject() 
//{
//   scenegraph              = %mySceneGraph;
//   class                   = SelectionList;
//
//   //-----------------------------------------------------------------------------
//   // Next/Previous Button Appearance ( Shared Attributes )
//   //-----------------------------------------------------------------------------
//
//   // The size of the prev/next buttons
//   buttonSize              = "5 5";
//   // The base image (background) to be used on the Next/Prev item buttons
//   buttonBaseImage         = myButtonBaseImageMap;
//   // The font to be used on prev/next buttons (if any text is associated with them)
//   buttonFontName          = $Font::Small;
//   // The font color to be used on prev/next buttons (if any text is associated with them)
//   buttonFontColor         = "1 1 1 1";
//   // The font mount position to be used on prev/next buttons (if any text is associated with them)
//   buttonFontOffset        = "0 0";
//   // The font size to be used on prev/next buttons (if any text is associated with them)
//   buttonFontSize          = "16";
//
//   //-----------------------------------------------------------------------------
//   // Previous Button Appearance 
//   //-----------------------------------------------------------------------------
//
//   // The text to be placed on the 'previous item' button.  ("" means no text)
//   prevButtonText          = "<<"; 
//   // The overlay image to be placed on top of the buttonBaseImage
//   // ( << graphic or the like - generally with no button text )
//   prevButtonOverlayImage  = myPrevButtonImageMap;
//   // Position on the base to mount the previous button ( "-0.9 0" Default )
//   prevButtonMountPos      = "-0.9 0";
//   // The cell of the imagemap to use (defaults to 0)
//   prevButtonImageIndex    = 0;
//
//   //-----------------------------------------------------------------------------
//   // Next Button Appearance
//   //-----------------------------------------------------------------------------
//
//   // The text to be placed on the 'next item' button.  ("" means no text)
//   nextButtonText          = ">>"; 
//   // The overlay image to be placed on top of the buttonBaseImage
//   // ( >> graphic or the like - generally with no button text )
//   nextButtonOverlayImage  = myNextButtonImageMap;
//   // Position on the base to mount the previous button ( "0.9 0" Default )
//   nextButtonMountPos      = "0.9 0";
//   // The cell of the imagemap to use (defaults to 0)
//   nextButtonImageIndex    = 0;
//
//};


function SelectionList::onAdd(%this)
{
   
   %scenegraph = %this.getSceneGraph();

   // Set current entry to none.
   %this.currentEntry = -1;

   %this.base = %this;

   %this.setLayer(%this.layer);
 
   // Create the Previous Item Button
   %this.btnPrev = new t2dSceneObject() 
   { 
      scenegraph     = %sceneGraph;
      
      class          = SpriteButton;
      superClass     = ButtonBase;      

      text           = %this.prevButtonText;
      overlayImage   = %this.prevButtonOverlayImage;
      imageIndex     = %this.prevButtonImageIndex;
      hoverImageIndex= %this.prevButtonHoverImageIndex;
      clickImageIndex= %this.prevButtonClickImageIndex;
      size           = %this.buttonSize;
      baseImage      = %this.buttonBaseImage;
      fontName       = %this.buttonFontName;
      fontColor      = %this.buttonFontColor;
      fontOffset     = %this.buttonFontOffset;
      fontSize       = %this.buttonFontSize;
      command        = %this SPC ".prevItem();";
      noAnimate      = %this.noAnimate;
      immediateEval  = true;
      layer          = %this.layer;
   };

   %this.btnPrev.setLayer(%this.layer);
   %this.btnPrev.mount( %this, %this.prevButtonMountPos );
   

   // Create the Next Item Button
   %this.btnNext = new t2dSceneObject() 
   { 
      scenegraph     = %sceneGraph;
      
      class          = SpriteButton;
      superClass     = ButtonBase;      

      text           = %this.nextButtonText;
      overlayImage   = %this.nextButtonOverlayImage;
      imageIndex     = %this.nextButtonImageIndex;
      hoverImageIndex= %this.nextButtonHoverImageIndex;
      clickImageIndex= %this.nextButtonClickImageIndex;
      size           = %this.buttonSize;
      baseImage      = %this.buttonBaseImage;
      fontName       = %this.buttonFontName;
      fontColor      = %this.buttonFontColor;
      fontOffset     = %this.buttonFontOffset;
      fontSize       = %this.buttonFontSize;
      command        = %this SPC ".nextItem();";
      noAnimate      = %this.noAnimate;
      immediateEval  = true;
      layer          = %this.layer;
   };

   // Layer/Group/Mount Next Button
//   %this.btnNext.setGraphGroup(2);
   %this.btnNext.setLayer(%this.layer);
   %this.btnNext.mount( %this, %this.nextButtonMountPos );


   // Create the list
   %this.list = new SimSet();
   
}

function SelectionList::setBlendColor( %this, %color )
{
   %this.btnPrev.setBlendColor( %color );
   %this.btnNext.setBlendColor( %color );
   Parent::setBlendColor( %this, %color );
}

function SelectionList::onRemove(%this)
{
   // Remove the Prev Button
   if(isObject(%this.btnPrev))
      %this.btnPrev.safeDelete();
   // Remove the Next Button
   if(isObject(%this.btnNext))
      %this.btnNext.safeDelete();
      
   // Remove the Item List
   if( isObject(%this.list) )
   {
      %count = %this.list.getCount();
      while( %this.list.getCount() )
         %this.list.getObject(0).delete();
      %this.list.delete();
   }
}

//-----------------------------------------------------------------------------
// Selection List Callbacks
//-----------------------------------------------------------------------------
function SelectionList::OnSelChange( %this, %oldSelection, %newSelection )
{
   %newValue = %newSelection.value;
   %newText = %newSelection.text;
   
   //echo("Sel changed from ["@%oldSelection.text@","@%oldSelection.value@"] to ["@%newSelection.text@","@%newSelection.value@"]");
   
   if( %this.command !$= "" )
      eval( %this.command );
}


//-----------------------------------------------------------------------------
// Action Methods
//-----------------------------------------------------------------------------
function SelectionList::clear( %this )
{
   if( isObject(%this.list) )
   {
      // Delete all the list entries and clear the SimSet 
      while( %this.list.getCount() > 0 )
      {
         if( isObject( %this.list.getObject( 0 ) ) )
            %this.list.getObject( 0 ).delete();
      }
      
      // Delete the list
      %this.list.delete();
   }

   %this.list = new SimSet();
   %this.currentEntry = -1;
}

function SelectionList::add( %this, %text, %value )
{
   // Should update?
   if( %this.list.getCount() == 0 )
      %shouldUpdate = true;
   else
      %shouldUpdate = false;

   // Construct the new entry
   %entry = new ScriptObject()
   {
      class = SelectionListEntry;
      text = %text;
      value = %value;
   };

   // Add the entry to our list
   %this.list.add( %entry );

   // Update the users view
   //if( %shouldUpdate )
   //{
      %this.OnSelChange( %entry, %entry );
      %this.updateDisplay();
   //}
}

function SelectionList::NextItem( %this )
{
   %this.updateDisplay();
   
   // Sanity!
   if( !isObject( %this.list) || %this.list.getCount() <= 1 )
      return;

   // No entry? Default to first.
   if( %this.currentEntry == -1 )
   {
      echo("Resetting list to beginning");
      %this.SetSel( %this.list.getObject( 0 ) );
      return;
   }

   for( %i = 0; %i < %this.list.getCount(); %i++ )
   {
      %object = %this.list.getObject( %i );
      if( %object == %this.currentEntry )
      {
         // Loop around?
         if( %i + 1 == %this.list.getCount() )
            %this.SetSel( %this.list.getObject( 0 ) );
         else
            %this.SetSel( %this.list.getObject( %i + 1 ) );

         return;
      }
   }

   // If we get here, something has gone terribly wrong
   // So we'll just default to the first entry.
   %this.SetSel( %this.list.getObject( 0 ) );
}


function SelectionList::PreviousItem( %this )
{
   // Stub so that either works, making peoples ease of use go up
   %this.Prev();
}

function SelectionList::PrevItem( %this )
{
   // Sanity!
   if( !isObject( %this.list) || %this.list.getCount() <= 1 )
      return;

   // No entry? Default to first.
   if( %this.currentEntry == -1 )
   {
      %this.SetSel( %this.list.getObject( 0 ) );
      return;
   }

   for( %i = 0; %i < %this.list.getCount(); %i++ )
   {
      %object = %this.list.getObject( %i );
      if( %object == %this.currentEntry )
      {
         // Loop around?
         if( %i - 1 < 0 )
            %this.SetSel( %this.list.getObject( %this.list.getCount() - 1  ) );
         else
            %this.SetSel( %this.list.getObject( %i - 1 ) );

         return;
      }
   }

   // If we get here, something has gone terribly wrong
   // So we'll just default to the last entry.
   %this.SetSel( %this.list.getObject( %this.list.getCount() - 1 ) );
}


function SelectionList::SetSel( %this, %entry )
{
   // Sanity
   if( !isObject(%this.list) || !isObject( %entry ) )
      return;
   
   //store old entry to pass to onSelChange
   %oldEntry = %this.currentEntry;
   
   // Store new entry
   // Done before onSelChange so that currentEntry is updated before onSelChange so that the command is useful
   %this.currentEntry = %entry;
   
   // Notify user of selection change
   %this.OnSelChange( %oldEntry, %entry );
   
   
   // Update Display
   %this.updateDisplay();
   
   
}

//Chooses a selection based on the first occurence of that value in the list
function SelectionList::setSelByValue( %this, %value )
{
   %this.updateDisplay();
   
   // Sanity!
   if( !isObject( %this.list) || %this.list.getCount() <= 1 )
      return false;

   for( %i = 0; %i < %this.list.getCount(); %i++ )
   {
      %object = %this.list.getObject( %i );
      if( %object.value == %value )
      {
         %this.SetSel( %this.list.getObject( %i ) );

         return true;
      }
   }

   // If we get here, then there isn't an object with that value
   // So we'll just default to the first entry.
   %this.SetSel( %this.list.getObject( 0 ) );
   
   //return false because we weren't able to find that value in the list
   return false;
}

//Sets the selection to the last object
function SelectionList::setSelToLast( %this )
{
   %this.updateDisplay();
   
   // Sanity!
   if( !isObject( %this.list) || %this.list.getCount() <= 1 )
      return false;

   %this.SetSel( %this.list.getObject( %this.list.getCount() - 1 ) );
   return true;
}


function SelectionList::GetSel( %this )
{
   return %this.currentEntry;
}

function SelectionList::RemoveById( %this, %id )
{
   // Sanity
   if( !isObject(%this.list) )
      return -1;

   // Remove
   if( %this.list.getCount() <= %index )
      %this.list.remove( %id );
}

function SelectionList::UpdateDisplay( %this )
{
   if( %this.currentEntry == -1 )
   {
      if( isObject(%this.list) && %this.list.getCount() != 0 )
         %this.currentEntry = %this.list.getObject( 0 );
      else
         return;
   }
}

//-----------------------------------------------------------------------------
// Item Retrieval Methods
//-----------------------------------------------------------------------------
function SelectionList::GetItemByIndex( %this, %index )
{
   if( !isObject(%this.list) )
      return -1;

   if( %this.list.getCount() <= %index )
      return %this.list.getObject(%index);

   // None found!
   return -1;

}

function SelectionList::GetItemByText( %this, %text )
{
   if( !isObject(%this.list) )
      return -1;

   for(%i = 0; %i < %this.list.getCount(); %i++)
      if( %this.list.getObject(%i).text $= %text )
         return %this.list.getObject(%i);

   // None found!
   return -1;
}

function SelectionList::GetItemByValue( %this, %value )
{
   for(%i = 0; %i < %this.list.getCount(); %i++)
      if( %this.list.getObject(%i).value $= %value || %this.list.getObject(%i).value == %value)
         return %this.list.getObject(%i);

   // None found!
   return -1;

}
