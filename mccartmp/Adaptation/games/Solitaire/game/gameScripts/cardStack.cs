//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// @class CardStack
/// @brief Container object that holds playing cards
///
/// A CardStack is an object that holds cards (a deck, a pile, and a hand are
/// all kinds of cardstacks).  The "holds" field can be used to initialize
/// it.  Manipulating cardstacks is the primary operation in any card game.
/// Manipulation of card stacks frequently causes the onStatusUpdate callback
/// of cards to be called, which is the primary method of providing user 
/// feedback (e.g. moving cards to different locations based on their movement
/// between stacks.
///
/// @field string holds A torquescript list of Card objects to initialize the object with.
/// @field SimSet container The thing that actually holds the cards.

function CardStack::onAdd(%this)
{
   // Custom onAdd constructor.

   %this.container = new SimSet() {};
   for (%i=0; %i<getWordCount(%this.holds); %i++)
   {
      %this.container.add(getWord(%this.holds, %i));
   }
   %this.holds = "";
}

function CardStack::onRemove(%this)
{
   // Custom onRemove desctructor

   // This used to be a while loop, but sometimes it was getting stuck on exit
   // and infinitely spamming the console with warnings, and eating all of memory.
   %numObjects = %this.container.getCount();
   for (%i=0; %i<%numObjects; %i++)
   {
      %this.container.getObject(0).delete();
   }
   if (%this.container.getCount() != 0)
   {
      error(%this SPC "CardStack onRemove error!");
   }
}

/// (SimID this)
/// Get the number of cards in the stack.
///
/// @param this The CardStack object
///
function CardStack::getNumCards(%this)
{
   if (isObject(%this.container))
   {
      return %this.container.getCount();
   }
   else
   {
      return 0;
   }
}

/// (SimID this, int index)
/// Get the Card object at position %%index in the stack
///
/// @param this The CardStack object
/// @param index The element to access.
///
function CardStack::getCard(%this, %index)
{
   return %this.container.getObject(%index);
}

/// (SimID this)
/// A debug method to print the contents of the stack to the console.
///
/// @param this The CardStack object
///
function CardStack::list(%this)
{
   %debugString = "Stack contains:";
   for (%i=0; %i<%this.container.getCount(); %i++)
   {
      %debugString = %debugString SPC %this.container.getObject(%i).getCard();
   }
   echo(%debugString);
}

/// (SimID this)
/// Reorders the deck in a random fashion.
///
/// @param this The CardStack object
///
function CardStack::shuffle(%this)
{
   %numCards = %this.container.getCount();
   for (%i=0; %i<%numCards*10; %i++)
   {
      %card = mFloor(getRandom(0, %numCards-1));
      %this.container.bringToFront(%this.container.getObject(%card));
   }
   for (%i=0; %i<%this.container.getCount(); %i++)
   {
      %this.container.getObject(%i).index = %i;
      if (%this.container.getObject(%i).callbackActive)
      {
         %this.container.getObject(%i).onStatusUpdate();
      }
   }
}

/// (SimID this, string cardClass)
/// Initializes a cardstack to be a full deck of cards
///
/// @param this The CardStack object
/// @param cardClass The class to use for the created card objects, defaults to Card
///
function CardStack::initializeFullDeck(%this, %cardClass)
{
   // first, clear out anything stale.
   while(%this.container.getCount() > 0)
   {
      %this.container.getObject(0).delete();
   }

   // now, iterate through all suits and values to create a full deck.
   echo("Suits:" SPC $Card::Suits);
   echo("Values:" SPC $Card::Values);
   for (%s=0; %s<getWordCount($Card::Suits); %s++)
   {
      for (%v=0; %v<getWordCount($Card::Values); %v++)
      {
         %card = new ScriptObject()
         {
            // if %cardClass is undefined, use card as the class and no superClass
            // otherwise, use the specified class, and card as the superClass
            class = (%cardClass $= "" ? "card" : %cardClass);
            superClass = (%cardClass $= "" ? "" : "card");
            owner = %this;
            suit = getWord($card::suits, %s);
            value = getWord($card::values, %v);
            facing = "down";
            callbackActive = false;
         };
         %this.container.add(%card);
      }
   }
}

/// (SimID this)
/// Returns the Card object at the top of the stack.
///
/// @param this The CardStack object
///
function CardStack::lookTop(%this)
{
   %card = %this.container.getObject(%this.container.getCount()-1);
   return %card;
}

/// (SimID this, CardStack to, Card card, string transferType)
/// Transfers cards from this stack to another
///
/// @param this The CardStack Object
/// @param to The destination CardStack Object
/// @param card either a card index or a Card object to start the transfer with
/// @param transferType String that specifies how to transfer -- SINGLE or RESTOFSTACK, single is default
///
function CardStack::transferTo(%this, %to, %card, %transferType)
{
   if (%this.getID() == %to.getID())
   {
      error("Can't transfer to and from the same object!" SPC %this SPC %to);
      return;
   }
      
   if (%transferType $= "")
   {
      %transferType = "SINGLE";
   }
   
   if (isObject(%card))
   {
      if (%this.container.isMember(%card))
      {
         for (%i=0; %i<%this.container.getCount(); %i++)
         {
            if (%this.container.getObject(%i) == %card)
            {
               %index = %i;
               break;
            }
         }
      }
      else
      {
         error(%card SPC "is not a member of" SPC %this SPC ", can't transfer!");
         return;
      }
   }
   else
   {
      %index = %card;
      if (%index >= %this.container.getCount())
      {
         error(%index SPC "is outside bounds of stack");
         return;
      }
   }
   
   
   if (%transferType $= "SINGLE")
   {
      %card = %this.container.getObject(%index);
      %this.container.remove(%card);
      for (%i=%index; %i<%this.container.getCount(); %i++)
      {
         %this.container.getObject(%i).index = %i;
         if (%this.container.getObject(%i).callbackActive)
         {
            %this.container.getObject(%i).onStatusUpdate();
         }
      }
      %to.add(%card);
   }
   else if (%transferType $= "RESTOFSTACK")
   {
      while(%this.container.getCount()-1 >= %index)
      {
         %card = %this.container.getObject(%index);
         %this.container.remove(%card);
         %to.add(%card);
      }
   }
   else
   {
      error("Don't know what to do with transfer type" SPC $transferType);
      return;
   }
}

         
/// (SimID this)
/// Removes the top Card object from the stack and returns it.
///
/// @param this The CardStack object
///
function CardStack::takeTop(%this)
{
   %card = %this.container.getObject(%this.container.getCount()-1);
   if (!isObject(%card))
   {
      error(%card SPC "is not a card object!");
   }
   %card.owner = "";
   %this.container.remove(%card);
   for (%i=0; %i<%this.container.getCount(); %i++)
   {
      %otherCard = %this.container.getObject(%i);
      %otherCard.index = %i;
      if (%otherCard.callbackActive)
      {
         %otherCard.onStatusUpdate();
      }
   }
   return %card;
}

/// (SimID this, Card card)
/// Add a card to the stack
///
/// @param this The CardStack object
/// @param card The card to add to the stack
function CardStack::add(%this, %card)
{
   %card.owner = %this;
   %card.index = %this.container.getCount();
   %this.container.add(%card);
   if (%card.callbackActive)
   {
      %card.onStatusUpdate();
   }
}
