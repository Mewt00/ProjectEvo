//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// @class SolitaireStack
/// @brief Solitaire-specific CardStack object (inherits from CardStack)
///
/// A SolitaireStack is a solitaire-specific CardStack object, inheriting
/// all the methods from that class, and extending it to implement the 
/// solitaire-specific functionality we need for the game.
///
/// @field stackType Set to type if it has one:  alternating or ace
/// @field hSkew Each graphical card in the stack is horiz spaced out by this amount
/// @field vSkew Each graphical card in the stack is vert spaced out by this amount

/// (SimID this, t2dSceneGraph scenegraph)
/// Create t2dStaticSprites for each card in a stack.
///
/// @param this The SolitaireStack object
/// @param scenegraph The t2dSceneGraph object to create the sprites in.
///
function SolitaireStack::associateCardsWithImages(%this, %scenegraph)
{
   for (%i=0; %i<%this.getNumCards(); %i++)
   {
      %card = %this.getCard(%i);
      for (%s=0; %s<getWordCount($Card::Suits); %s++)
      {
         if (getWord($Card::Suits, %s) $= %card.getSuit()) {break;}
      }
      for (%v=0; %v<getWordCount($Card::Values); %v++)
      {
         if (getWord($Card::Values, %v) $= %card.getValue()) {break;}
      }
   
      %card.sprite = new t2dStaticSprite()
      {
         scenegraph = %scenegraph;
         backImageMap = cardback_smlImageMap;
         frontImageMap = playingcardsImageMap;
         frontFrame = %s*getWordCount($Card::values) + %v;
         imageMap = cardback_smlImageMap;
         size = $CardSprite::SIZE_X SPC $CardSprite::SIZE_Y;
         layer = 15;
         class = "CardSprite";
         card = %card;
      };
      %stackOriginX = %this.getPositionX() - %this.getSizeX()/2 + 
                      %card.sprite.getSizeX()/2;
      %stackOriginY = %this.getPositionY() - %this.getSizeY()/2 + 
                      %card.sprite.getSizeY()/2;

      %positionX = %stackOriginX + %card.owner.hSkew * %card.index;
      %positionY = %stackOriginY + %card.owner.vSkew * %card.index;
      %card.sprite.setPosition(%positionX, %positionY);

      %card.callbackActive = true;
      %card.onStatusUpdate();
   }
}


/// (SimID this)
/// Test to see if the user can do a default "manipulation" to this stack.
///
/// @param this The SolitaireStack object
///
function SolitaireStack::legalToManipulate(%this)
{
   // The only stack that can do anything is the "deck" stack
   // which can either deal out three cards to the "discard"
   // or to flip the discard pile back over to the deck after
   // the user has gone all the way through.
   if ((%this.getID() == deck.getID()) &&
       (mouse.getNumCards() == 0))
   {
      return true;
   }
   else
   {
      return false;
   }
}

/// (SimID this)
/// Do a default "manipulation" to this stack.
///
/// @param this The SolitaireStack object
///
function SolitaireStack::manipulate(%this)
{
   if (deck.getNumCards() == 0)
   {
      // Out of cards, flip the discard pile back over to be the new deck.
      while(discard.getNumCards() > 0)
      {
         %card = discard.takeTop();
         deck.add(%card);
         %card.setFacing("down");
      }      
   }
   else
   {
      // Use the "updateDelay" feature to get the fwip-fwip-fwip three card deal.
      %dealDelay = 100;
      %numCardsToTransfer = t2dGetMin(deck.getNumCards(), 3);
      for (%i=0; %i<%numCardsToTransfer; %i++)
      {
         %card = deck.lookTop();
         %card.setUpdateDelay(%dealDelay);
         %card.setFacing("up");
         deck.transferTo(discard, %card);
         schedule(%dealDelay, 0, alxPlay, whoosh);
         %dealDelay += 100; 
      }
   }
}
