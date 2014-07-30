// A cardStack is an object that holds cards (a deck, a pile, and a hand are
// all kinds of cardstacks).  The "holds" parameter can be used to initialize
// it.

function cardStack::onAdd(%this)
{
   %this.container = new SimSet() {};
   for (%i=0; %i<getWordCount(%this.holds); %i++)
   {
      %this.container.add(getWord(%this.holds, %i));
   }
   %this.holds = "";
}

function cardStack::onRemove(%this)
{
   // This used to be a while loop, but sometimes it was getting stuck on exit
   // and infinitely spamming the console with warnings, and eating all of memory.
   %numObjects = %this.container.getCount();
   for (%i=0; %i<%numObjects; %i++)
   {
      %this.container.getObject(0).delete();
   }
   if (%this.container.getCount() != 0)
   {
      error(%this SPC "cardStack onRemove error!");
   }
}

function cardStack::getNumCards(%this)
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

function cardStack::getCard(%this, %index)
{
   return %this.container.getObject(%index);
}

function cardStack::list(%this)
{
   %debugString = "Stack contains:";
   for (%i=0; %i<%this.container.getCount(); %i++)
   {
      %debugString = %debugString SPC %this.container.getObject(%i).getCard();
   }
   //echo(%debugString);
}

function cardStack::shuffle(%this)
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

function cardStack::initializeFullDeck(%this)
{
   // first, clear out anything stale.
   while(%this.container.getCount() > 0)
   {
      %this.container.getObject(0).delete();
   }

   // now, iterate through all suits and values to create a full deck.
   //echo("Suits:" SPC $card::suits);
   //echo("Values:" SPC $card::values);
   for (%s=0; %s<getWordCount($card::suits); %s++)
   {
      for (%v=0; %v<getWordCount($card::values); %v++)
      {
         %card = new ScriptObject()
         {
            class = card;
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

function cardStack::lookTop(%this)
{
   %card = %this.container.getObject(%this.container.getCount()-1);
   return %card;
}

function cardStack::transferTo(%this, %to, %card, %transferType)
{
   if (%this.getID() == %to.getID())
   {
      error("Can't transfer to and from the same object!" SPC %this SPC %to);
      SceneWindow2D.danger();
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
         SceneWindow2D.danger();
         return;
      }
   }
   else
   {
      %index = %card;
      if (%index >= %this.container.getCount())
      {
         error(%index SPC "is outside bounds of stack");
         SceneWindow2D.danger();
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
      //%card.index = %to.getNumCards();
      %to.add(%card);
   }
   else if (%transferType $= "RESTOFSTACK")
   {
      while(%this.container.getCount()-1 >= %index)
      {
         %card = %this.container.getObject(%index);
         //echo("Gotta transfer" SPC %card);
         %this.container.remove(%card);
         //echo("Removing from" SPC %index SPC "and" SPC %this.container.getCount() SPC
         //     "items in set");
         //%this.container.listObjects();
         //%card.index = %to.getNumCards();
         %to.add(%card);
         //%card.onStatusUpdate();
      }
   }
   else
   {
      error("Don't know what to do with transfer type" SPC $transferType);
      SceneWindow2D.danger();
      return;
   }
}

         

function cardStack::takeTop(%this)
{
   %card = %this.container.getObject(%this.container.getCount()-1);
   if (!isObject(%card))
   {
      SceneWindow2D.danger();
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

function cardStack::add(%this, %card)
{
   %card.owner = %this;
   %card.index = %this.container.getCount();
   %this.container.add(%card);
   if (%card.callbackActive)
   {
      %card.onStatusUpdate();
   }
}
