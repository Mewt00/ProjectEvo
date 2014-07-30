//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// @class SolitaireCard
/// @brief Solitaire-specific Card object (inherits from Card)
///
/// A SolitaireCard is a solitaire-specific Card object, inheriting
/// all the methods from that class, and extending it to implement the 
/// solitaire-specific functionality we need for the game.

/// (SimID this)
/// This function is called whenever something significant happens to a card.
/// We use this function to update the graphical objects to reflect the change,
/// for example by moving to the right place in the right stack, or flipping
/// from the "front" to the "back" image map when the card's facing changes.
///
/// @param this The SolitaireCard object
///
function SolitaireCard::onStatusUpdate(%this)
{
   // Sometimes we don't want the update to take place right away.
   // This defers it until later.
   if (%this.delaying && !%this.deferScheduled)
   {
      %this.schedule(%this.delayTime, deferredUpdate);
      %this.deferScheduled = true;
      return;
   }
   else if (%this.delaying)
   {
      return;
   }

   if (!isObject(%this.owner))
   {
      error(%this SPC "card doesn't have an owner!" SPC %this.owner);
   }
   
   %stackOriginX = %this.owner.getPositionX() - %this.owner.getSizeX()/2 + 
                   %this.sprite.getSizeX()/2;
   %stackOriginY = %this.owner.getPositionY() - %this.owner.getSizeY()/2 + 
                   %this.sprite.getSizeY()/2;
   
   %positionX = %stackOriginX + %this.owner.hSkew * %this.index;
   %positionY = %stackOriginY + %this.owner.vSkew * %this.index;
   if ((%positionX != %this.sprite.getPositionX()) ||
       (%positionY != %this.sprite.getPositionY()))
   {
      %distance = t2dVectorDistance(%this.sprite.getPosition(), 
                                       %positionX SPC %positionY);
      %minSpeed = (%this.owner.cardMinSpeed > 0) ? (%this.owner.cardMinSpeed) : ($CardSprite::DefaultMinSpeed);
      %maxTime =  (%this.owner.cardMaxTime > 0)  ? (%this.owner.cardMaxTime) :  ($CardSprite::DefaultMaxTime);

      %speed = t2dGetMax(%minSpeed, %distance/%maxTime); // get there in third second or less.

      %this.sprite.moveTo(%positionX, %positionY, %speed, true, true);  // set up callback to support movement in "motionLayer"
   }
   if (%this.facing $= "up")
   {
      %this.sprite.imageMap = %this.sprite.frontImageMap;
      %this.sprite.frame = %this.sprite.frontFrame;
   }
   else
   {
      %this.sprite.frame = 0;
      %this.sprite.imageMap = %this.sprite.backImageMap;
      
   }
   %this.sprite.setSortPointY(%this.index*$CardSprite::SortSizeMultiplier);
   if (%this.sprite.getAtRest())
   {
      %this.sprite.setLayer(%this.owner.getLayer());
   }
   else
   {
      %this.sprite.setLayer($CardSprite::MotionLayer);
   }
}

/// (SimID this)
/// This allows the "onStatusUpdate" effect to be delayed from when the
/// actual update happened in script to the object.
///
/// @param this The SolitaireCard object
///
function SolitaireCard::setUpdateDelay(%this, %delay)
{
   %this.delaying = true;
   %this.delayTime = %delay;
}


/// (SimID this)
/// A helper function to implement deferred updates.
///
/// @param this The SolitaireCard object
///
function SolitaireCard::deferredUpdate(%this)
{
   %this.delaying = false;
   %this.deferScheduled = false;
   %this.onStatusUpdate();
}

/// (SimID this)
/// Test to see if the user can pick a card up (according to the
/// rules of solitaire), to potentially place it on another stack.
///
/// @param this The SolitaireCard object
///
function SolitaireCard::legalToPickUp(%this)
{
   if (%this.owner.stackType $= "alternating")
   {
      if (%this.facing $= "down")
      {
         // can't pick up face down cards!
         return false;
      }         
      else if (%this.index == %this.owner.getNumCards()-1)
      {
         // Top card in stack is always legal to pick up.
         return true;
      }
      else if (%this.index == 0)
      {
         // It's the only card here -- sure, pick it up.
         return true;
      }
      else if (%this.owner.getCard(%this.index-1).facing $= "down")
      {
         // It's on top of a face down card, so you can pick it up.
         return true;
      }
      else
      {
         // anything else is illegal.
         return false;
      }
   }
   else if ((%this.owner.getID() == discard.getID()) &&
            (%this.index == discard.getNumCards()-1))
   {
      // You can pick up the top card from the "discard" pile.
      return true;
   }
   else
   {
      return false;
   }
}

/// (SimID this)
/// Test to see if the user can (according to the rules of solitaire)
/// put the card he's holding down on a particular stack.
///
/// @param this The SolitaireCard object
///
function SolitaireCard::legalToDrop(%this, %target)
{
   if (%target.stackType $= "alternating")
   {
      if ((%target.getNumCards() == 0) &&
          (%this.getValue() == 13))
      {
         // king can start a new column.
         return true;
      }
      else if (%target.getNumCards() == 0)
      {
         // if it isn't a king, can't start a new column.
         return false;
      }
      
      %topSuit = %target.lookTop().getSuit();
      %topValue = %target.lookTop().getValue();
      
      %thisSuit = %this.getSuit();
      %thisValue = %this.getValue();
      
      if ((((%thisSuit $= "h") || (%thisSuit $= "d")) &&
           ((%topSuit $= "s") || (%topSuit $= "c"))) ||
          (((%thisSuit $= "s") || (%thisSuit $= "c")) &&
           ((%topSuit $= "h") || (%topSuit $= "d"))))
      {
         // if it's opposite color...
         if (%thisValue == (%topValue - 1))
         {
            // and one step down in value, then it's legal to put it there.
            return true;
         }
      }
      // No other legal destinations.
      return false;
   }
   else if (%target.stackType $= "ace")
   {
      if (%target.getNumCards() == 0)
      {
         if (%this.getValue() == 1)
         {
            // Ace has a value of 1, ace is legal starter for the ace stacks.
            return true;
         }
         else
         {
            // other cards can't go on empty ace pile
            return false;
         }
      }
      else
      {
         if ((%target.lookTop().getSuit() $= %this.getSuit()) &&
             (%target.lookTop().getValue()+1 == %this.getValue()))
         {
            // matching suit and next higher value.
            return true;
         }
         else
         {
            // nothing else is legal.
            return false;
         }
      }
   }      
}

/// (SimID this)
/// Test to see if the user can (according to the rules of solitaire) flip
/// a card over.
///
/// @param this The SolitaireCard object
///
function SolitaireCard::legalToFlip(%this)
{
   if (%this.owner.stackType $= "alternating")
   {
      if ((%this.facing $= "down") &&
          (%this.index == %this.owner.getNumCards()-1))
      {
         // It's currently face down, and on the top of the stack.
         return true;
      }
   }
   // Otherwise, no, you can't flip it.
   return false;
}
