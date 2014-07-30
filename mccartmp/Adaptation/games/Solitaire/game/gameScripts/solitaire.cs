//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

/// Set up the cards on the table to play solitaire.

// Sets up the game board to the "start" state of all the cards in the deck.
function Solitaire::resetDeck()
{
   %checkStacks = "aces aceh acec aced stack0 stack1 stack2 stack3 stack4 stack5 stack6 discard";
   for (%i=0; %i<getWordCount(%checkStacks); %i++)
   {
      %stack = getWord(%checkStacks, %i);
      if (%stack.getNumCards() > 0)
      {
         // move every card in the stack back to the deck.
         %stack.transferTo(deck, 0, RESTOFSTACK);
      }
   }
   
   for (%i=0; %i<deck.getNumCards(); %i++)
   {
      deck.getCard(%i).setFacing("down");
   }
}

// Deal out the cards onto the table.
function Solitaire::dealSolitaire()
{
   // Handle people who do lots of deals in succession...
   if ($SolitaireGUI::EnableInputEvent != 0)
   {
      cancel($SolitaireGUI::EnableInputEvent);
   }
   
   Solitaire::resetDeck();
   if (deck.getNumCards() < 52)
   {
      deck.initializeFullDeck("SolitaireCard");
      deck.associateCardsWithImages(sceneWindow2D.getSceneGraph());
   }

   deck.shuffle();

   %dealDelay = 100 + 1000 * $CardSprite::DefaultMaxTime;

   for (%i=0; %i<7; %i++)
   {
      %card = deck.lookTop();
      %card.setUpdateDelay(%dealDelay);
      %card.setFacing("up");
      deck.transferTo("stack"@%i, %card);
      schedule(%dealDelay, 0, alxPlay, whoosh);
      %dealDelay += 100; 
      for (%j=%i+1; %j<7; %j++)
      {
         %card = deck.lookTop();
         %card.setUpdateDelay(%dealDelay);
         %card.setFacing("down");
         deck.transferTo("stack"@%j, %card);
         schedule(%dealDelay, 0, alxPlay, whoosh);
         %dealDelay += 100; 
      }
   }

   $SolitaireGUI::VictorySignalled = false;
   $SolitaireGUI::AllowGameInput = false;
   $SolitaireGUI::EnableInputEvent = schedule(%dealDelay, 0, eval, "SolitaireGui::enableGameInput();");
}

// The user wants to "deal".  We need handle the case of the user who goes nuts 
// and presses the "deal" button a zillion times in a row, though.
function Solitaire::pressDealButton()
{
   if ($SolitaireGUI::AllowGameInput)
   {
      Solitaire::dealSolitaire();
   }
   else if ($SolitaireGUI::NumDeferredDeals > 0)
   {
      // we only want to queue up one deal.
      return;
   }
   else
   {
      $SolitaireGUI::NumDeferredDeals++;
      if ($SolitaireGUI::DeferredDealEvent == 0) 
      {
         %timeLeft = getEventTimeLeft($SolitaireGUI::EnableInputEvent);
         $SolitaireGUI::DeferredDealEvent = schedule($CardSprite::DefaultMaxTime + %timeLeft, 0, eval, "Solitaire::doDeferredDeal();");
      }
   }
}

// We can only do a new deal from a known good state, so this lets us do one 
// "later", after we reach that known good state.
function Solitaire::doDeferredDeal()
{
   $SolitaireGUI::NumDeferredDeals--;
   $SolitaireGUI::DeferredDealEvent = 0;
   
   if ($SolitaireGUI::NumDeferredDeals < 0)
   {
      error("doDeferredDeal called, but no outstanding deferred deals!");
   }
   
   Solitaire::dealSolitaire();
   
   if ($SolitaireGUI::NumDeferredDeals > 0)
   {
      %timeLeft = getEventTimeLeft($SolitaireGUI::EnableInputEvent);
      $SolitaireGUI::DeferredDealEvent = schedule($CardSprite::DefaultMaxTime + %timeLeft, 0, eval, "Solitaire::doDeferredDeal();");
   }
}


// Victory detector.
function Solitaire::checkForWin()
{
   if ($SolitaireGUI::DebugSignalWin) {
      return true;
   }
   
   // You win if you get all 13 cards of each suit into the ace stacks.
   if ((aceh.getNumCards() >= 13) &&
       (aced.getNumCards() >= 13) &&
       (aces.getNumCards() >= 13) &&
       (acec.getNumCards() >= 13))
   {
      return true;
   }
   else
   {
      return false;
   }
}

// Every frame, check to see if the player won the game.
function SolitaireSceneGraph::onUpdateScene(%this)
{
   if (!$SolitaireGUI::VictorySignalled)
   {
      if (Solitaire::checkForWin())
      {
         if ($SolitaireGUI::ScheduledPopUpEvent == 0)
         {
            $SolitaireGUI::ScheduledPopUpEvent = schedule($CardSprite::DefaultMaxTime*1000, 0, eval, "SolitaireGui::popUpGui();");
         }
      }
   }
}
