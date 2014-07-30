//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// @class Card
/// @brief Create a playing card object
///
/// The Card object is the fundamental building block of the card game.
///
/// @field bool callbackActive if set to true the function onStatusUpdate when card status changes
/// @field int value the value of the card from ace=1 to king=13
/// @field string suit the suit of the card usually one character
/// @field string facing is the card face up or face down

/// Card Default Legal Suits
$Card::Suits = "h d c s";

/// Card Default Legal Values
$Card::Values = "1 2 3 4 5 6 7 8 9 10 11 12 13";

function Card::onAdd(%this)
{
   // custom onAdd constructor

   if (%this.callbackActive)
   {
      %this.onStatusUpdate();
   }
}

function Card::onRemove(%this)
{
   // custom onRemove desctructor
}

/// (SimID this)
/// Accessor function that returns the value/suit pair
///
/// @param this The Card object
///
function Card::getCard(%this)
{
   return %this.value @ %this.suit;
}

/// (SimID this)
/// Accessor function that returns the value of the card
///
/// @param this The Card object
///
function Card::getValue(%this)
{
   return %this.value;
}

/// (SimID this)
/// Accessor function that returns the suit of the card
///
/// @param this The Card object
///
function Card::getSuit(%this)
{
   return %this.suit;
}

/// (SimID this)
/// Accessor function that returns the facing of the card
///
/// @param this The Card object
///
function Card::getFacing(%this)
{
   return %this.facing;
}

/// (SimID this, string facing)
/// Sets the card's facing (to "up" or "down")
///
/// @param this The Card object
/// @param facing What facing to set the card to.
///
function Card::setFacing(%this, %facing)
{
   %this.facing = %facing;
   if (%this.callbackActive)
   {
      %this.onStatusUpdate();
   }
}

/// (SimID this)
/// Sets the card's facing to the opposite of whatever it is now
///
/// @param this The Card object
///
function Card::toggleFacing(%this)
{
   if (%this.facing $= "up")
   {
      %this.facing = "down";
   }
   else if (%this.facing $= "down")
   {
      %this.facing = "up";
   }
   else
   {
      error("Bad facing!");
   }
   if (%this.callbackActive)
   {
      %this.onStatusUpdate();
   }
}
