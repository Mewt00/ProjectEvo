//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// Functions that implement the mouse input for the game.  Since picking up
/// and putting down cards is done with the mouse, a lot of game logic lives 
/// in here.

function sceneWindow2D::onMouseDragged(%this, %modifier, %worldPosition, %clicks)
{
   %this.onMouseMove(%modifier, %worldPosition, %clicks);
}

function sceneWindow2D::onMouseMove(%this, %modifier, %worldPosition, %clicks)
{
   // If the user is holding cards, we need to tell the cards that are being
   // held that they need to move with the mouse.
   if (mouse.getNumCards() > 0)
   {
      mouse.setPosition(%worldPosition);
      for (%i=0; %i<mouse.getNumCards(); %i++)
      {
         mouse.getCard(%i).onStatusUpdate();
      }
   }

   // check to see if the mouse is over any buttons.
   SolitaireGui::checkGuiButtons(%worldPosition);   
}



function sceneWindow2D::onMouseDown(%this, %modifier, %worldPosition, %clicks)
{
   // The "mouse" stack is a SolitaireStack that holds "picked up" cards.
   mouse.setPosition(%worldPosition);
   mouse.buttonDown = true;
   SolitaireGui::checkGuiButtons(%worldPosition);   
   
   if ($SolitaireGUI::GuiActive)
   {
      // GUI is up, don't do any game functions.
      return;
   }

   if (!$SolitaireGUI::AllowGameInput)
   {
      // Mouse is disabled, because we're dealing out the cards.
      return;
   }

   // Alright, bookkeeping done, let's actually do some real work!
   %objects = %this.getSceneGraph().pickPoint(%worldPosition);
   %cardObjects = MouseInput::filterAndSort(%objects, CardSprite);
   for (%i=0; %i<getWordCount(%cardObjects); %i++)
   {
      %object = getWord(%cardObjects, %i);
      %card = %object.card;
      // Let's see if it's legal to pick up.
      if (%card.legalToPickUp())
      {
         mouse.pickedUpFrom = %card.owner;
         %card.owner.transferTo(mouse, %card, RESTOFSTACK);

         // double-click: try to move directly to ace pile.
         if (%clicks == 2)
         {
            if (mouse.getNumCards() == 1)
            {
               for (%i=0; %i<4; %i++)
               {
                  %target = getWord("aces acec aced aceh", %i);
                  if (mouse.getCard(0).legalToDrop(%target))
                  {
                     mouse.transferTo(%target, 0, RESTOFSTACK);
                     alxPlay(whoosh);
                     break;
                  }
               }
            }
         }
                     
         break;
      }
      else if (%card.legalToFlip())
      {
         %card.toggleFacing();
         break;
      }
   }
   
   %stackObjects = MouseInput::filterAndSort(%objects, SolitaireStack);
   for (%i=0; %i<getWordCount(%stackObjects); %i++)
   {
      %object = getWord(%stackObjects, %i);
   
      if (%object.legalToManipulate())
      {
         %object.manipulate();
         break;
      }
   }   
}
      
function sceneWindow2D::onMouseUp(%this, %modifier, %worldPosition, %clicks)
{
   mouse.setPosition(%worldPosition);
   mouse.buttonDown = false;
   SolitaireGui::checkGuiButtons(%worldPosition);   

   if (SolitaireGui::activateGuiButton(%worldPosition) > 0)
   {
      return;
   }

   if ($SolitaireGUI::GuiActive)
   {
      // GUI is up, don't do any game functions.
      return;
   }

   if (!$SolitaireGUI::AllowGameInput)
   {
      // Mouse is disabled, because we're dealing out the cards.
      return;
   }


   // Now do the actual function!
   if (mouse.getNumCards() > 0)
   {
      %dropTargets = %this.getSceneGraph().pickPoint(%worldPosition);
      for (%i=0; %i<getWordCount(%dropTargets); %i++)
      {
         %target = getWord(%dropTargets, %i);
         if ((isObject(%target)) && (%target.class $= "SolitaireStack"))
         {
            if (%target.getID() == mouse.getID())
            {
               continue;
            }
            
            if (mouse.getCard(0).legalToDrop(%target))
            {
               mouse.transferTo(%target, 0, RESTOFSTACK);
               break;
            }
            else
            {
               mouse.transferTo(mouse.PickedUpFrom, 0, RESTOFSTACK);
               break;
            }               
         }
      }
   }
   // Gave it a chance to deposit in a new place.  If didn't work, go back home.
   if (mouse.getNumCards() > 0)
   {
      mouse.transferTo(mouse.PickedUpFrom, 0, RESTOFSTACK);
   }
}
         

function sceneWindow2D::onMouseLeave(%this, %modifier, %worldPosition, %clicks)
{
   // treat it like a virtual onMouseUp.
   %this.onMouseUp(%modifier, %worldPosition, %clicks);
}
   
function sceneWindow2D::onRightMouseUp(%this, %modifier, %worldPosition, %clicks)
{
   SolitaireGui::toggleGui();
}

// A helper function that filters a list of objects for only objects of
// a certain class, and then sorts those objects based on their Y sort
// points.  This is useful for picking up the "top" card when card sprite
// objects overlap.
function MouseInput::filterAndSort(%objects, %class)
{
   %sortArrayLength = 0;
   
   // first, filter for only the sets we care about.
   for (%i=0; %i<getWordCount(%objects); %i++)
   {
      %object = getWord(%objects, %i);
      if (isObject(%object) && (%object.class $= %class))
      {
         %sortArray[%sortArrayLength] = %object;
         %sortArrayLength++;
      }
   }

   // empty arrays and one element arrays are already intrinsically sorted.
   if (%sortArrayLength == 0)
   {
      return "";
   }
   else if (%sortArrayLength == 1)
   {
      return %sortArray[0];
   }
   else
   {
      // Do an insertion sort.
      for (%sortedUpTo=1; %sortedUpTo<%sortArrayLength; %sortedUpTo++)
      {
         %compare = %sortedUpTo-1;
         while(%sortArray[%compare].getSortPointY() < %sortArray[%sortedUpTo].getSortPointY())
         {
            %compare--;
            if (%compare < 0) {break;}
         }
         %compare++; // while went "one too far", so back up one.
         
         // found where to insert it, shift everything to the right out of the way.
         %temp = %sortArray[%sortedUpTo];
         for (%i=%sortedUpTo; %i>%compare; %i--)
         {
            %sortArray[%i] = %sortArray[%i-1];
         }
         %sortArray[%compare] = %temp;
      }
   }
   
   // pack the sorted array into a torque list.
   %retString = %sortArray[0];
   for (%i=1; %i<%sortArrayLength; %i++)
   {
      %retString = %retString SPC %sortArray[%i];
   }
   return %retString;
}


