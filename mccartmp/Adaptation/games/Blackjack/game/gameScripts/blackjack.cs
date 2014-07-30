$chipStack::pattern[0] = "0 0";
$chipStack::pattern[1] = "0 0";
$chipStack::pattern[2] = "0 -0.5" SPC "0 0.5";
$chipStack::pattern[3] = "0 -0.433" SPC "-0.5 0.433" SPC "0.5 0.433";
$chipStack::pattern[4] = "-0.5 -0.5" SPC "0.5 -0.5" SPC "0.5 0.5" SPC "-0.5 0.5";
$chipStack::pattern[5] = "-0.5 -0.45" SPC "0.5 -0.45" SPC "-0.5 0.5" SPC "0.5 0.5" SPC "-1.35 0";
$chipStack::pattern[6] = "-0.5 -0.45" SPC "0.5 -0.45" SPC "-0.5 0.5" SPC "0.5 0.5" SPC "-1.5 -0.45" SPC "-1.5 0.45";
$chipStack::pattern[7] = "-0.5 -0.45" SPC "0.5 -0.45" SPC "-0.5 0.5" SPC "0.5 0.5" SPC "-1.5 -0.45" SPC "-1.5 0.45" SPC "-2.5 0";
$chipStack::pattern[8] = "-0.5 -0.45" SPC "0.5 -0.45" SPC "-0.5 0.5" SPC "0.5 0.5" SPC "-1.5 -0.45" SPC "-1.5 0.45" SPC "-2.5 -0.45" SPC "-2.5 0.45";
$chipStack::pattern[9] = "-0.5 -0.45" SPC "0.5 -0.45" SPC "-0.5 0.5" SPC "0.5 0.5" SPC "-1.5 -0.45" SPC "-1.5 0.45" SPC "-2.5 -0.45" SPC "-2.5 0.45" SPC "-3.5 0";
$chipStack::pattern[10] = "-0.5 -0.45" SPC "0.5 -0.45" SPC "-0.5 0.5" SPC "0.5 0.5" SPC "-1.5 -0.45" SPC "-1.5 0.45" SPC "-2.5 -0.45" SPC "-2.5 0.45" SPC "-3.5 -0.45" SPC "-3.5 0.45";


function blackJackSceneGraph::onUpdateScene(%this)
{
   //This should be run by the animationscenegraph in GUI_MAIN.cs, but that seems not to work right now ;)
   if(isObject(AnimationManager))
   {
      AnimationManager.update( %elapsedTime );
   }
   
   for (%i=0; %i<$numTestObjects; %i++)
   {
      if (isObject($testObject[%i]))
      {
         %distance = t2dVectorDistance(0 SPC 0, $testObject[%i].getPosition());
         //$testObject[%i].moveTo(0 SPC 0, %distance);
         //$testObject[%i].setImpulseForce(t2dVectorScale($testObject[%i].getPosition(), -1));
      }
   }
 
   // tick the FSM.
   if (!isObject(%this.tableControl)) {return;} 
   
   %prevState = %this.tableControl.curState;
   %this.tableControl.update();
   %curState = %this.tableControl.curState;

   doubleDownButton.enable = false;
   splitButton.enable = false;

   if ((%curState $= "PLACEBETS") && (%this.tableControl.playerHands.getCount() <= 2))
   {
      if(sceneWindow2D.getScenegraph().getWallet() > 0)
      {
         betButton.setEnabled(true);
         quickBetButton.setEnabled(true);
      }
      else
      {
         betButton.setEnabled(false);
         quickBetButton.setEnabled(false);
      }
      
      if( !betMenu.up )
         changeBetButton.setEnabled(true);
      else
         changeBetButton.setEnabled(false);
   }
   else
   {
      betButton.setEnabled(false);
      quickBetButton.setEnabled(false);
      changeBetButton.setEnabled(false);
   }
   
   if ((%curState $= "PLACEBETS") && (%this.tableControl.playerHands.getCount() >= 1))
   {
      dealButton.setEnabled(true);
   }
   else
   {
      dealButton.setEnabled(false);
   }
   
   if (%curState $= "GETUSERINPUT")
   {
      hitButton.setEnabled(true);
      standButton.setEnabled(true);
   }
   else
   {
      hitButton.setEnabled(false);
      standButton.setEnabled(false);
   }
   
   if ((%prevState !$= %curState) || (%this.tableControl.playerHands.getCount() != %oldCount))
   {
      // the state of the GUI impacts the buttons.
      SceneWindow2D.checkGuiButtons(sceneWindow2D.getMousePosition());
   }
   %oldCount = %this.tableControl.playerHands.getCount();
}



// Blackjack Finite State Machine (FSM)

function blackjackFSM::onAdd(%this)
{
   %this.curState = "IDLE";
   %this.initialBetsPlaced = "false";
   %this.playerHands = new SimSet();
   %this.activeHand = 0;
   dealerScoreImage.setFrame(24);
   dealerScoreImage.setVisible(true);
   dealerScoreText.setVisible(false);
   
   SceneWindow2D.getScenegraph().resetWallet();
}

function blackjackFSM::update(%this)
{
   switch$(%this.curState)
   {
      case "IDLE":
         %this.nextState = "IDLE";
         
      case "PLACEBETS":
         if (%this.initialBetsPlaced)
         {
            if (%this.playerHands.getCount() == 0)
            {
               error("FSM error:  bets placed but no active hands?!");
               %this.nextState = "IDLE";
            }
            else
            {
               // Woohoo, all bets are placed, it's time to deal some cards!
               %dealDelay = 100;
               for (%i=0; %i<%this.playerHands.getCount(); %i++)
               {
                  %handObject = %this.playerHands.getObject(%i);
                  for (%cards=0; %cards<2; %cards++)
                  {
                     %card = shoe.lookTop();
                     %card.setUpdateDelay(%dealDelay);
                     %card.setFacing("up");
                     shoe.transferTo(%handObject, %card);
                     
                     %dealDelay += 100;
                  }
                  %score = %handObject.blackjackScore();
                  
                  if(%score > 21)
                  {
                     %handObject.scoreSprite.schedule(%dealDelay, setFrame, %score);
                     %handObject.scoreSprite.setVisible(true);
                     %handObject.scoreText.setVisible(false);
                  } else
                  {
                     schedule(%dealDelay, %handObject.scoreText, "eval", %handObject.scoreText.getID() @ ".text = \"" @ %score @ "\";");
                     %handObject.scoreText.setVisible(true);
                     %handObject.scoreSprite.setVisible(false);
                  }
               }
               
               %dealerHole = shoe.lookTop();
               %dealerHole.setUpdateDelay(%dealDelay);
               shoe.transferTo(dealerHand, %dealerHole);
               %dealDelay += 100;
               
               %dealerUp = shoe.lookTop();
               %dealerUp.setUpdateDelay(%dealDelay);
               %dealerUp.setFacing("up");
               shoe.transferTo(dealerHand, %dealerUp);
               %dealDelay += 100;

               // Figure out which hand is the active one.
               while((%this.playerHands.getObject(%this.activeHand).blackjackScore() >= 21) &&
                     (%this.activeHand < %this.playerHands.getCount()))
               {
                  // Auto-stand on obvious ones.
                  %standActive = true;
                  break;
               }

               if(%standActive)
               {
                  %this.standActiveHand();
               } else
               {
                  %pointerPos = t2dVectorAdd(%this.playerHands.getObject(%this.activeHand).getPosition(),
                                          0 SPC -$card::sizeY);
                  %distance = t2dVectorDistance(activePointer.getPosition(), %pointerPos);
                  activePointer.repositioned = false;
                  activePointer.schedule(%dealDelay, moveTo, %pointerPos, %distance/0.2, true, true);
               }

               %this.dealIsDone = false;
               %this.schedule(%dealDelay, setDealIsDone);
               
               %this.nextState = "WAITFORDEAL";
            }
         }
         else
         {
            //%this.applyNewBetList(%this.calculateBetList());
            %this.nextState = "PLACEBETS"; 
         }
         
      case "WAITFORDEAL":
         if (%this.dealIsDone)
         {
            // de-"highlight" the inactive cards.
            for (%i=0; %i<%this.playerHands.getCount(); %i++)
            {      
               %handObject = %this.playerHands.getObject(%i);
               for (%j=0; %j<%handObject.getNumCards(); %j++)
               {
                  if (%i == %this.activeHand)
                  {
                     %handObject.getCard(%j).sprite.setBlendColor(1 SPC 1 SPC 1 SPC 1);
                  }
                  else
                  {
                     %handObject.getCard(%j).sprite.setBlendColor(0.5 SPC 0.5 SPC 0.5 SPC 1);
                  }
               }
            }

            %this.nextState = "GETUSERINPUT";
         }
         else
         {
            %this.nextState = "WAITFORDEAL";
         }

      case "GETUSERINPUT":
         %userIsDone = true;
         for (%i=0; %i<%this.playerHands.getCount(); %i++)
         {
            %handObject = %this.playerHands.getObject(%i);
            if (%handObject.done == false)
            {
               %userIsDone = false;
               break;
            }
         }
         
         if (%userIsDone)
         {
            %this.nextState = "WAITFORDEALERSTURN";
         }
         else
         {
            %this.nextState = "GETUSERINPUT";
         }
         
      case "WAITFORDEALERSTURN":
         if (activePointer.repositioned)
         {
            dealerHand.getCard(0).setFacing("up");
            %this.nextState = "DEALERSTURN";
         }
         else
         {
            %this.nextState = "WAITFORDEALERSTURN";
         }

      case "DEALERSTURN":
         %score = dealerHand.blackjackScore();
         
         if(%score > 21)
         {
            dealerScoreImage.setFrame(%score);
            dealerScoreText.setVisible(false);
            dealerScoreImage.setVisible(true);            
         } else
         {
            dealerScoreText.text = %score;
            dealerScoreText.setVisible(true);
            dealerScoreImage.setVisible(false);
         }
         
         if (%score >= 17)
         {
            // dealer must stand.
            %this.nextState = "PAYOFFBETS";
         }
         else
         {
            // dealer must hit.
            %card = shoe.lookTop();
            %card.setFacing("up");
            shoe.transferTo(dealerHand, %card);
            
            %this.waiting = true;
            %this.schedule(1000*$card::defaultMinTime, wakeUp);
            %this.nextState = "DEALERPLAYINGDELAY";
         }
         
      case "DEALERPLAYINGDELAY":
         if (%this.waiting)
         {
            %this.nextState = "DEALERPLAYINGDELAY";
         }
         else
         {
            %this.nextState = "DEALERSTURN";
         }
         
      case "PAYOFFBETS":
         // Figure out winnings.
         %delay = 200;
         
         %amount = 0;
         for (%i=0; %i<%this.playerHands.getCount(); %i++)
         {
            %handObject = %this.playerHands.getObject(%i);
            if (dealerHand.blackjackScore() == 23)
            {
               // dealer blackjack
               %winner = "house";
               %amount += -%handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, -%handObject.betBox.bet);
            }
            else if ((%handObject.getNumCards() >= 6) && (%handObject.blackjackScore() <= 20))
            {
               // player has 6 cards with less than 20.
               %winner = "player";
               %amount += %handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, %handObject.betBox.bet);
            }
            else if (%handObject.blackjackScore() == 22)
            {
               // player bust.
               %winner = "house";
               %amount += -%handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, -%handObject.betBox.bet);
            }
            else if (%handObject.blackjackScore() == 23)
            {
               // player blackjack.  Pay 2:1.
               %winner = "player";
               %amount += 2*%handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, 2*%handObject.betBox.bet);
            }
            else if (dealerHand.blackjackScore() == 22)
            {
               // dealer bust.
               %winner = "player";
               %amount += %handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, %handObject.betBox.bet);
            }
            else if (dealerHand.blackjackScore() >= %handObject.blackjackScore())
            {
               // dealer has better hand.
               %winner = "house";
               %amount += -%handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, -%handObject.betBox.bet);
            }
            else
            {
               %winner = "player";
               %amount += %handObject.betBox.bet;
               %this.schedule(%delay, updateCredit, %handObject.betBox.bet);
            }
            
            if (%winner $= "house")
            {
               %chipDest = 0 SPC -120;
            }
            else
            {
               %chipDest = 0 SPC 120;
            }
            
            for (%j=0; %j<%handObject.betBox.chipSet.getCount(); %j++)
            {
               %chipStack = %handObject.betBox.chipSet.getObject(%j);
               %chipStack.chipStackSetPosition(%chipDest, %delay);
            }
                        
            %delay += 200;
         }
         
         %this.waiting = true;
         
         %time = t2dGetMax(%delay, 1500);
         
         %this.schedule(%time, wakeUp);
         %this.schedule(%time, "popUpCredits", %amount);
         %this.nextState = "WAITFORCHIPSTOCHANGEHANDS";
         
      case "WAITFORCHIPSTOCHANGEHANDS":
         if (%this.waiting)
         {
            %this.nextState = "WAITFORCHIPSTOCHANGEHANDS";
         }
         else
         {
            %bailout = 0;
            while(%this.playerHands.getCount() > 0)
            {
               %hand = %this.playerHands.getCount()-1;
               %handObject = %this.playerHands.getObject(%hand);
               %handObject.transferTo(discard, 0, RESTOFSTACK);
               %this.playerHands.remove(%handObject);
               %numStacks = %handObject.betBox.chipSet.getCount();
               for (%i=0; %i<%numStacks; %i++)
               {
                  %handObject.betBox.chipSet.getObject(0).delete();
               }
               if (%handObject.betBox.chipSet.getCount() != 0)
               {
                  error("Problem deleting chipstacks...");
               }
               %handObject.betBox.fadeOut(25, 0.1);
               %handObject.scoreSprite.fadeOut(25, 0.1);
               %handObject.safeDelete();
               %bailout++;
               if (%bailout > 100) {error("Huh:" SPC %bailout);  break;}
            }
            dealerHand.transferTo(discard, 0, RESTOFSTACK);
            dealerScoreImage.setFrame(24);
            dealerScoreImage.setVisible(true);
            dealerScoreText.setVisible(false);
            
            SceneWindow2D.getScenegraph().resetWallet();
            blackjackFSM_Singleton.applyNewBetList(blackjackFSM_Singleton.calculateBetList());
      
            %this.activeHand = 0;
            
            %this.waiting = true;
            %this.schedule(1000, wakeUp);
            %this.nextState = "WAITFORTABLETOCLEAR";
         }
         
      case "WAITFORTABLETOCLEAR":
         if (%this.waiting)
         {
            %this.nextState = "WAITFORTABLETOCLEAR";
         }
         else if (shoe.getNumCards() < 20)
         {
            %this.initialBetsPlaced = false;
            %this.nextState = "RESHUFFLE";
            
            if( credit.amount <= 0 )
               showLoanButton();
         }
         else
         {
            %this.initialBetsPlaced = false;
            %this.nextState = "PLACEBETS";
            
            if( credit.amount <= 0 )
               showLoanButton();
         }
         
      case "RESHUFFLE":
         shoe.transferTo(discard, 0, RESTOFSTACK);
         if (discard.getNumCards() != 52*4)
         {
            error("we don't have to right number of total cards!" SPC discard.getNumCards());
            SceneWindow2D.danger();
         }
         discard.shuffle();
         for (%i=0; %i<discard.getNumCards(); %i++)
         {
            discard.getCard(%i).setFacing("down");
            discard.getCard(%i).setUpdateDelay(1000*$card::defaultMinTime);
         }
         discard.transferTo(shoe, 0, RESTOFSTACK);
         
         %this.waiting = true;
         %this.schedule(100 * 2 * $card::defaultMinTime, wakeUp);
         
         %this.nextState = "WAITINGFORSHOERESTOCK";
         
      case "WAITINGFORSHOERESTOCK":
         if (%this.waiting)
         {
            %this.nextState = "WAITINGFORSHOERESTOCK";
         }
         else
         {
            %this.nextState = "PLACEBETS";
         }         
         
      default:
         error("Unknown FSM state:" SPC %this.curState);
   }

   if (%this.nextState !$= %this.curState)
   {
      //echo("blackjackFSM:" SPC %this.curState SPC "->" SPC %this.nextState);
   }

   %this.prevState = %this.curState;
   %this.curState = %this.nextState;   
}

function blackjackFSM::calculateBetList(%this)
{
   //echo("calcing....");
   %highest = SceneWindow2D.getSceneGraph().getWallet();
   
   echo("wallet has: "@%highest);
   
   if(%highest <= 0)
   {
      //%highest = 50;
   }
   
   %lowest = 50;
   %interval = mFloor( (%highest)/7 );
   
   %current = betSelectionList.GetSel().value;
   
   if(%this.lastUsedBet > %current)
      %current = %this.lastUsedBet;
   
   
   
   %betList = "";
   
   if( %current < %lowest )
   {      %addCandidate = %current;
         %nextCandidate = %current;
   }
   else
   {
      %addCandidate = %lowest;
      %nextCandidate = %lowest;
   }
   
   if(%current > %highest)
      %current = -1000;
   
   //echo("%highest: "@%highest@" %lowest: "@%lowest@" %interval: "@%interval@" %current: "@%current);
   
   //If the player doesn't even have enough money for the low limit, they can only bet it all
   if( %highest <= %lowest )
   {
      //echo(">>>>>>>>>>>>>>>>>>>>>Returning betList: <"@%highest@">");
      return %highest; // @ "";
   }
   
   //We'll keep increasing %addCandidate until it reaches %highest, adding values as we go      
   while( %nextCandidate < %highest )
   {
      %addCandidate = %nextCandidate;
      %nextCandidate = roundNicely(%addCandidate + %interval);
      
      //we need to make sure the current bet value is in the list at the correct location
      if( %addCandidate < %current && %nextCandidate > %current )
         %betList = %betList SPC %addCandidate SPC %current;
      else if( %addCandidate < %highest )
      {
         %betList = %betList SPC %addCandidate;
      }      
      //echo("Betlist is now: <"@%betList@">");
   }
   
   //don't allow betting of 0 dollars
   //strreplace(%betList, " 0", "");
   //strreplace(%betList, " 0 ", " ");
   
   %betList = ltrim( %betList SPC %highest );
   //echo("Returning betList: <"@%betList@">");
   return %betList;
   
   
}

function blackjackFSM::applyNewBetList(%this, %betList)
{
   //echo("Applying new betList");
   %origValue = betSelectionList.getSel().value;
   
   echo("lastSelListVal: "@%origValue@", lastUsedBet: "@%this.lastUsedBet);
   
   if(%this.lastUsedBet > %origValue )
      %origValue = %this.lastUsedBet;
   
   betSelectionList.clear();
   
   while(%betList !$= "")
   {
      %first = firstWord(%betList);
      betSelectionList.add( "$" @ %first, %first);
      %betList = restWords(%betList);      
   }
   
   if( ! betSelectionList.setSelByValue(%origValue) )
      betSelectionList.setSelToLast();
      
}

function roundNicely(%value)
{
   if(%value < 100)
      return %value;
   else if (%value < 1000)
      return (mCeil( %value/100 ) * 100);
   else 
      return (mCeil( %value/1000 ) * 1000);
}


            
function blackjackFSM::setDealIsDone(%this)
{
   %this.dealIsDone = true;
}

function blackjackFSM::wakeUp(%this)
{
   %this.waiting = false;
}

function blackjackFSM::registerHand(%this, %hand)
{
   %this.playerHands.add(%hand);
}

function blackjackFSM::hitActiveHand(%this)
{
   %hand = %this.playerHands.getObject(%this.activeHand);
   %hand.hitMe();
   %score = %hand.blackjackScore();
   
   if(%score > 21)
   {
      %hand.scoreSprite.setFrame(%score);
      %hand.scoreSprite.setVisible(true);
      %hand.scoreText.setVisible(false);
   } else
   {
      %hand.scoreText.text = %score;
      %hand.scoreText.setVisible(true);
      %hand.scoreSprite.setVisible(false);
   }

   // if we bust or win, we obviously don't want more cards.
   if (%score >= 21)
   {
      %this.standActiveHand();
   }
}

function blackjackFSM::standActiveHand(%this)
{
   %this.playerHands.getObject(%this.activeHand).done = true;
   %this.activeHand++;

   while((%this.activeHand < %this.playerHands.getCount()) &&
         (%this.playerHands.getObject(%this.activeHand).blackjackScore() >= 21))
   {
      // Auto-stand on obvious ones.
      %this.playerHands.getObject(%this.activeHand).done = true;
      %this.activeHand++;
   }

   if (%this.activeHand < %this.playerHands.getCount())
   {
      // de-"highlight" the inactive cards.
      for (%i=0; %i<%this.playerHands.getCount(); %i++)
      {      
         %handObject = %this.playerHands.getObject(%i);
         for (%j=0; %j<%handObject.getNumCards(); %j++)
         {
            if (%i == %this.activeHand)
            {
               %handObject.getCard(%j).sprite.setBlendColor(1 SPC 1 SPC 1 SPC 1);
            }
            else
            {
               %handObject.getCard(%j).sprite.setBlendColor(0.5 SPC 0.5 SPC 0.5 SPC 1);
            }
         }
      }
      %pointerPos = t2dVectorAdd(%this.playerHands.getObject(%this.activeHand).getPosition(),
                                 0 SPC -$card::sizeY);
      %distance = t2dVectorDistance(activePointer.getPosition(), %pointerPos);
      activePointer.repositioned = false;
      activePointer.moveTo(%pointerPos, %distance/0.2, true, true);
   }
   else
   {
      // we're all done with player input, no more concept of "inactive", so hightlight all.
      for (%i=0; %i<%this.playerHands.getCount(); %i++)
      {      
         %handObject = %this.playerHands.getObject(%i);
         for (%j=0; %j<%handObject.getNumCards(); %j++)
         {
            %handObject.getCard(%j).sprite.setBlendColor(1 SPC 1 SPC 1 SPC 1);
         }
      }
      %pointerPos = 0 SPC -100;
      %distance = t2dVectorDistance(activePointer.getPosition(), %pointerPos);
      activePointer.repositioned = false;
      
      %pointerPosX = activePointer.getPositionX();
      %pointerPosY = activePointer.getPositionY();

      
      if((%pointerPosX == getWord(%pointerPos, 0)) && (%pointerPosY == getWord(%pointerPos, 1)))
      {
         activePointer.repositioned = true;
      } else
      {
         activePointer.moveTo(%pointerPos, %distance/0.3, true, true);
      }
   }
}

function blackjackFSM::updateCredit(%this, %amount)
{
   echo("Credit is" SPC credit.amount SPC "and adjust by" SPC %amount);
   credit.amount += %amount;
   credit.text = "$" @ credit.amount;
   echo("Credit is now" SPC credit.amount SPC "which is" SPC credit.text);
   
   sceneWindow2D.getScenegraph().resetWallet();
}

function blackjackFSM::popUpCredits(%this, %amount)
{
   %popUpText = new t2dTextObject()
   {
      scenegraph = SceneWindow2D.getScenegraph();
      class = PopupText;
      superClass = textObjectClass;
      layer = 5;
      position = "-135 95";
      
      delayTime = "1000";
      
      font = $Font::DefaultFont;
      textAlign = "Left";
      lineHeight = "16";
      autoSize = "1";
      fontSizes = "20";
      textColor = "1 1 1";
      hideOverlap = "0";
      wordWrap = "0";
      hideOverflow = "0";
      aspectRatio = "1";
      lineSpacing = "0";
      characterSpacing = "0";  
   };
   
   if(%amount > 0)
   {
      %popUpText.text = "You WON $" @ %amount;
      %popUpText.textColor = "0.25 1 0.25";    
      SceneWindow2D.fireOffFireworks(1);  
   } else if(%amount < 0)
   {
      %popUpText.text = "You LOST $" @ mAbs(%amount);
      %popUpText.textColor = "1 0.2 0.2";      
   } else
   {
      %popUpText.text = "You broke EVEN.";
   }   
}

function pointer::onPositionTarget(%this)
{
   %this.repositioned = true;
}










function clearHands()
{
   while(playerHand.getNumCards() > 0)
   {
      discard.add(playerHand.takeTop());
   }
   while(dealerHand.getNumCards() > 0)
   {
      discard.add(dealerHand.takeTop());
   }
}



function dealBlackjack()
{
   %dealerHole = shoe.takeTop();
   dealerHand.add(%dealerHole);

   schedule(250, 0, hit, dealerHand);
   schedule(500, 0, hit, playerHand);
   schedule(750, 0, hit, playerHand);
}

function hit(%stack)
{
   %stack.hitMe();
}

function cardStack::hitMe(%this)
{
   // we're going to add a new card.  We need to adjust the skew so it fits.
   if (%this.getNumCards() > 0)
   {   
      %newHskew = (%this.getSizeX() - $card::sizeX) / (%this.getNumCards()); 
      //echo("Must fit" SPC %this.getNumCards()+1 SPC "cards in sizeX" SPC %this.getSizeX() SPC
      //     "so we'll change the hSkew from" SPC %this.hSkew SPC "to" SPC %newHskew);
      %this.hSkew = %newHskew;
   }
   
   %hitCard = shoe.takeTop();
   %this.add(%hitCard);
   %hitCard.setFacing("up");

   for (%i=0; %i<%this.getNumCards(); %i++)
   {
      %this.getCard(%i).onStatusUpdate();
   }
}

function cardStack::blackjackScore(%this)
{
   %score = 0;
   %numAces = 0;
   for (%i=0; %i<%this.getNumCards(); %i++)
   {
      %card = %this.getCard(%i);
      if (%card.getValue() == 1)
      {
         %numAces++;
      }
      
      %score += t2dGetMin(10, %card.getValue());
   }
   
   for (%i=0; %i<%numAces; %i++)
   {
      if (%score <= 11)
      {
         %score += 10;
      }
   }
   
   if (%score >= 22)
   {
      // bust!  Make this bounded.
      %score = 22;
   }
   else if ((%this.getNumCards() == 2) && (%score == 21))
   {
      // detect and report special "blackjack" score.
      %score = 23;
   }
   else if (%this.getNumCards() == 0)
   {
      // "blank" score for no cards.
      %score = 24;
   }
   
   return %score;
}

function blackJackSceneGraph::spawnHand(%this)
{
   %hand = new t2dSceneObject()
   {
      class = cardStack;
      scenegraph = %this;
      size = 1.9*$card::sizeX SPC $card::sizeY;
      hSkew = 0.9*$card::sizeX;
      layer = 15;
      done = false;
      position = 0 SPC 120;
   };
   %this.tableControl.registerHand(%hand);

   %hand.betBox = new t2dStaticSprite()
   {
      class = chipArea;
      scenegraph = %this;
      size = $card::sizeX SPC $card::sizeY;
      imageMap = betBoxImageMap;
      layer = 15;
      visible = "0";
   };
   %betBoxOffsetX = -1 + %hand.betBox.getSizeX()/%hand.getSizeX();
   %hand.betBox.mount(%hand, %betBoxOffsetX, 2.1, 20, true, false, true, false);
   
   %betAmount = %this.getBetAmount();
   
   %hand.betBox.populateBet(%betAmount);
   
   %this.updateWallet(%betAmount);
   
   blackjackFSM_Singleton.lastUsedBet = %betAmount;
   
   %hand.scoreSprite = new t2dStaticSprite()
   {
      scenegraph = %this;
      imageMap = scoreImageMap;
      frame = 24;
      size = 0.6 * $card::sizeX SPC 0.6 * $card::sizeX;
      
   };
   
   %hand.scoreText = new t2dTextObject()
   {
      scenegraph = %this;
      class = textObjectClass;
      
      position = %hand.getPosition();
      layer = 15;
      
      font = $Font::DefaultFont;
      textAlign = "Left";
      lineHeight = "12";
      autoSize = "1";
      fontSizes = "20";
      textColor = "1 1 1";
      hideOverlap = "0";
      wordWrap = "0";
      hideOverflow = "0";
      aspectRatio = "1";
      lineSpacing = "0";
      characterSpacing = "0";  
   };

   //%scoreOffsetX = (2*%hand.betBox.getSizeX() + %hand.scoreSprite.getSizeX())/(%hand.getSizeX()) - 1;
   //%scoreOffsetY = (%hand.getSizeY() + %hand.scoreSprite.getSizeX())/(%hand.getSizeY());
   //%hand.scoreSprite.mount(%hand, %scoreOffsetX, %scoreOffsetY);   
   %scoreOffsetX = 1.3 + %hand.scoreSprite.getSizeX()/%hand.betBox.getSizeX();
   %scoreOffsetY = -1 + %hand.scoreSprite.getSizeY()/%hand.betBox.getSizeY();
   %hand.scoreSprite.mount(%hand.betBox, %scoreOffsetX, %scoreOffsetY, 0, true, false, true, false);
   %hand.scoreText.mount(%hand.betBox, %scoreOffsetX, %scoreOffsetY, 0, true, false, true, false);


   // position the hands.
   %totalNumHands = %this.tableControl.playerHands.getCount();

   if (0.95*handArea.getSizeX()/%totalNumHands < %hand.getSizeX())
   {
      %newSize = 0.95*handArea.getSizeX()/%totalNumHands;
      for (%i=0; %i<%totalNumHands; %i++)
      {
         %handObject = %this.tableControl.playerHands.getObject(%i);
         %handObject.setSizeX(%newSize);
         %handObject.hskew = (%newSize - $card::sizeX);
         %betBoxOffsetX = -1 + %handObject.betBox.getSizeX()/%handObject.getSizeX();
         %handObject.betBox.mount(%handObject, %betBoxOffsetX, 2.1, 20, true, false);
      }
   }

   %rightX = handArea.getPositionX() + handArea.getSizeX()/2;
   for (%i=0; %i<%totalNumHands; %i++)
   {
      %hand = %this.tableControl.playerHands.getObject(%i);
      %handSkew = handArea.getSizeX()/(%totalNumHands+1);
      
      if ((%totalNumHands > 0) && (%handSkew < %hand.getSizeX()))
      {
         %handSkew = (handArea.getSizeX()-%hand.getSizeX())/(%totalNumHands-1);
         %x = %rightX - (%hand.getSizex()/2) - %i * %handSkew;
      }
      else
      {
         %x = %rightX - (%i+1)*%handSkew;
      }
      
      %y = handArea.getPositionY();

      %distance = t2dVectorDistance(%hand.getPosition(), %x SPC %y);
      %hand.moveTo(%x, %y, %distance/0.1);
      
      %betBoxDestPosition = (%x - 9) SPC (%y + (%hand.getHeight() * 1.08));
      %handDestPosition = %x SPC %y;
      %vectorSub = t2dVectorSub(%betBoxDestPosition, %handDestPosition);
      %newPosition = t2dVectorAdd(%x SPC %y, %vectorSub);
      
      %hand.betBox.repositionTo(%newPosition);
   }  
   
   // now lets create the bet amount text
   %textObj = new t2dTextObject()
   {
      scenegraph = %this;
      class = textObjectClass;
      position = %hand.getPosition();
      layer = 15;
      
      font = $Font::DefaultFont;
      text = "$" @ %betAmount;  
      textAlign = "Left";
      lineHeight = "9";
      autoSize = "1";
      fontSizes = "20";
      textColor = "0.25 1 0.25";
      hideOverlap = "0";
      wordWrap = "0";
      hideOverflow = "0";
      aspectRatio = "1";
      lineSpacing = "0";
      characterSpacing = "0";       
   };
      
   %textObj.mount(%hand.betBox, 0, 1.3);    
}

      
/*
function playerHand::onAdd(%this)
{
   %this.hand = new t2dSceneObject()
   {
      class = cardStack;
      scenegraph = %this.scenegraph;
      size = 2*$card::sizeX SPC $card::sizeY;
      hSkew = 15;
      layer = %this.layer;
      done = false;
   };
   %this.hand.mount(%this, 0, 0);
   $tableControl.registerHand(%this.hand);

   %this.hand.betBox = new t2dStaticSprite()
   {
      scenegraph = %this.scenegraph;
      size = $card::sizeX SPC $card::sizeY;
      imageMap = betBoxImageMap;
      layer = %this.layer;
   };
   %this.hand.betBox.mount(%this.hand, 0, 2.1);

   %this.hand.scoreSprite = new t2dStaticSprite()
   {
      scenegraph = %this.scenegraph;
      imageMap = scoreImageMap;
      frame = 24;
      size = 20 SPC 20;
      layer = %this.layer;
   };
   %scoreOffsetX = (getWord(%this.size, 0) + %this.hand.scoreSprite.getSizeX())/(getWord(%this.size, 0));
   %this.hand.scoreSprite.mount(%this.hand, %scoreOffsetX, 0);
}
*/


function playerHand::onRemove(%this)
{
   %this.hand.delete();

   %this.betBox.delete();

   //%this.hitControl.delete();
   //%this.buttonHit.delete();
   
   //%this.standControl.delete();
   //%this.buttonStand.delete();
   
   //%this.splitControl.delete();
   //%this.buttonSplit.delete();
   
   //%this.doubleDownControl.delete();
   //%this.buttonDoubleDown.delete();

   %this.hand.scoreSprite.delete();
   %this.hand.scoreText.safeDelete();
}


function playerHand::hit(%this)
{
   // this hit function DOES NOT get called
   %this.hand.hitMe();
   %score = %this.hand.blackjackScore();
   if (%this.hand.getNumCards() < 2)
   {
      // hasn't finished dealing yet.
      %this.scoreSprite.setFrame(24);
      %this.scoreSprite.setVisible(true);
   }
   else if ((%this.hand.getNumCards() == 2) && (%score == 21))
   {
      // blackjack!
      %this.scoreSprite.setFrame(23);
      %this.scoreSprite.setVisible(true);
   }
   else if (%score > 21)
   {
      // bust!
      %this.scoreSprite.setFrame(22);
      %this.scoreSprite.setVisible(true);
   }
   else
   {
      error(%score);
      %this.scoreText.text = %score;
      %this.scoreSprite.setVisible(false);
      %this.scoreText.setVisible(true);
   }
}


function cardStack::chipStackSetPosition(%this, %worldPosition, %delay)
{
   %mx = getWord(%worldPosition, 0);
   %my = getWord(%worldPosition, 1);
   %this.hSkew = %mx/150;
   %this.vskew = -(75 - %my)/100;
   %this.setPosition(%worldPosition);
   
   for (%i=0; %i<%this.getNumCards(); %i++)
   {
      if (%delay > 0)
      {
         %this.getCard(%i).setUpdateDelay(%delay);
      }
      %this.getCard(%i).onStatusUpdate();
   }
}




function chipArea::addChipStack(%this, %pattern, %imageMap, %numChips, %delay)
{
   %stack = new t2dSceneObject()
   {
      class = "cardStack";
      scenegraph = sceneWindow2D.getSceneGraph();
      hSkew = -0.1;
      vSkew = -1;
      minSpeed = 10;
      minTime = 0.1;
      layer = %this.layer;
      position = 0 SPC 120;
      size = $card::sizeY/2 SPC $card::sizeY/2;
   };
         
   for (%i=0; %i<%numChips; %i++)
   {
      %card = new ScriptObject()
      {
         class = card;
         owner = %this;
         suit = getWord($card::suits, 0);
         value = getWord($card::values, 0);
         facing = "down";
         callbackActive = false;
      };
      %card.sprite = new t2dStaticSprite()
      {
         scenegraph = sceneWindow2D.getSceneGraph();
         imageMap = %imageMap;
         backImageMap = %imageMap;
         frontImageMap = %imageMap;
         frontFrame = 0;
         class = "cardSprite";
         card = %card;
         layer = 15;
         size = %stack.getSize();
      };    
      
      %card.owner = %stack;
      %card.index = %stack.getNumCards();
      %stack.add(%card);
      
      
      %stackOriginX = %stack.getPositionX() - %stack.getSizeX()/2 + 
                      %card.sprite.getSizeX()/2;
      %stackOriginY = %stack.getPositionY() - %stack.getSizeY()/2 + 
                      %card.sprite.getSizeY()/2;

      %positionX = %stackOriginX + %card.owner.hSkew * %card.index;
      %positionY = %stackOriginY + %card.owner.vSkew * %card.index;
      %card.sprite.setPosition(%positionX, %positionY);

      %card.callbackActive = true;
      %card.onStatusUpdate();
      
   }

   //echo("     Num chips in stack=" SPC %stack.getNumCards());
   

   if (!isObject(%this.chipSet))
   {
      %this.chipSet = new SimSet();
   }
   %index = %this.chipSet.getCount();
   %this.chipSet.add(%stack);

   %stackPosition = getWords($chipStack::pattern[%pattern], %index*2, %index*2 + 1);
   %stackPosition = t2dVectorScale(%stackPosition, %stack.getSizeX());
   %stackPosition = t2dVectorAdd(%this.getPosition(), %stackPosition);
   %stack.chipStackSetPosition(%stackPosition, %delay);
}

function chipArea::repositionTo(%this, %pos)
{
   %x = getWord(%pos, 0);
   %y = getWord(%pos, 1);
   
   %pattern = %this.chipSet.getCount();
   for (%index=0; %index<%pattern; %index++)
   {
      %stack = %this.chipSet.getObject(%index);
      %stackPosition = getWords($chipStack::pattern[%pattern], %index*2, %index*2 + 1);
      %stackPosition = t2dVectorScale(%stackPosition, %stack.getSizeX());
      %stackPosition = t2dVectorAdd(%x SPC %y, %stackPosition);
      %stack.chipStackSetPosition(%stackPosition);
   }
}

function chipArea::populateBet(%this, %amount)
{
   %this.bet = %amount;

   %maxStacks = 6;
   %maxChipsInStack = 10;
   %maxChips = %maxChipsInStack * %maxStacks;
   
   %totalAmount = credit.amount;
   
   %percentOfTotal = (%amount / %totalAmount);
   
   %numChips = mCeil(%maxChips * %percentOfTotal);
   %numStacks = mCeil(%percentOfTotal * %maxStacks);
   
   echo("%percentOfTotal = " @ %percentOfTotal);
   echo("totalAmount = " @ %totalAmount);
   echo("amount = " @ %amount);
   echo("numchips = " @ %numChips);
   echo("numStacks = " @ %numStacks);
   
   %chipCheck[0] = 100;
   %chipCheck[1] = 25;
   %chipCheck[2] = 5;
   %chipCheck[3] = 1;
   
   %chipCheckCount = 4;
   
   for(%i=0;%i<%chipCheckCount;%i++)
      %chipNum[%i] = 0;
   
   %chipCount = 0;
   
   if(mfloor(%amount / %chipCheck[0]) > %numChips)
   {
      // will create too many chips, we will only show a certain value
      %testAmount = %numChips * %chipCheck[0];
      echo("oldAmount = " @ %amount);
      echo("new amount = " @ %testAmount);
   } else
   {
      %testAmount = %amount;
   }
   
   %displayAmount = %testAmount;

   for(%i=0;%i<%chipCheckCount;%i++)
   {
      %chipToCheck = %chipCheck[%i];
      %chipNum[%i] = mFloor(%testAmount / %chipToCheck);
      %amountAltered = (%chipNum[%i] * %chipToCheck);
      %testAmount -= %amountAltered;
      %chipCount += %chipNum[%i];
      
      if(%testAmount <= 0)
      {
         if(%chipCount < %numChips)
         {
            if(%i > 0)
            {
               %skipForward = false;
               %newI = %i-1;
               while(%chipNum[%newI] == 0)
               {
                  %newI--;
                  if(%newI < 0)
                  {
                     %skipForward = true;
                     break;
                  }
               }
               
               if(%skipForward)
                  continue;
               
               // every chip check besides the first
               %chipNum[%newI]--;
               %testAmount += %chipCheck[%newI] + %amountAltered;
               %chipCount -= 1 + %chipNum[%i];
               %i = %newI;
            }
         } else
         {
            break;            
         }
      }
   }
   
   %total = 0;
   %stackCount = 0;
   for(%i=0;%i<%chipCheckCount;%i++)
   {
     %chipNum = %chipNum[%i];
     
     %total += (%chipCheck[%i] * %chipNum);
     
     %addCount = mCeil(%chipNum / 10);
     %testTotal = %chipNum;
     
     for(%j=0;%j<%addCount;%j++)
     {
        if(%testTotal > 10)
        {
           %chipsToAdd = 10;
        } else
        {
           %chipsToAdd = %testTotal;           
        }
        
        %testTotal -= %chipsToAdd;
        
        echo("adding stack numStacks = " @ %numStacks @ " chipType = " @ %chipCheck[%i] @ " chipsToAdd " @ %chipsToAdd);
        %chipType[%stackCount] = %chipCheck[%i];
        %chipAmount[%stackCount] = %chipsToAdd;
        %stackCount++;
     }
   }
   
   echo("total number of stacks = " @ %stackCount);
   
   for(%i=0;%i<%stackCount;%i++)
   {
      %chipType = %chipType[%i];
      %chipAmount = %chipAmount[%i];

      %this.addChipStack(%stackCount, "chip" @ %chipType @ "dollarImageMap", %chipAmount);      
   }
   
   if(%total != %displayAmount)
   {
      error("Totals don't match! (chip calculations)");
      error("total = " @ %total @ " displayAmount = " @ %displayAmount);
   }
   
   %delay = 100;
   /*if (%min1s > 0)
   {
      %this.addChipStack(%numStacks, chip1dollarImageMap, %min1s);
   }
   if (%min5s > 0)
   {
      %this.addChipStack(%numStacks, chip5dollarImageMap, %min5s);
   }
   if (%min25s > 0)
   {
      %this.addChipStack(%numStacks, chip25dollarImageMap, %min25s);
   }
   if (%min100s > 0)
   {
      %this.addChipStack(%numStacks, chip100dollarImageMap, %min100s);
   }*/

   //echo("Need" SPC %numStacks SPC "stacks for a bet of" SPC %amount SPC "can be done with" SPC %min100s SPC "$100chips" SPC
   //     %min25s SPC "$25chips" SPC %min5s SPC "$5chips" SPC %min1s SPC "$1chips");

   return %numStacks;
}

function blackJackSceneGraph::setBetAmount(%this, %amount)
{
   %this.betAmount = %amount;
}
   
function blackJackSceneGraph::getBetAmount(%this)
{
   if(%this.betAmount $= "")
      %this.betAmount = 347;
   
   return %this.betAmount;
}

function textObjectClass::onAdd(%this)
{
   %camArea = sceneWindow2D.getCurrentCameraArea();
   %camHeight = mAbs( getWord( %camArea, 1) - getWord( %camArea, 3) );
   %pixelHeight = getWord( sceneWindow2D.getWindowExtents(), 3);
   %this.addAutoFontSize( %pixelHeight , %camHeight, %this.lineHeight);   
}

function blackJackSceneGraph::getWallet(%this)
{
   return %this.wallet;   
}

function blackJackSceneGraph::updateWallet(%this, %amount)
{
   %this.wallet -= %amount; 
}

function blackJackSceneGraph::resetWallet(%this)
{
   %this.wallet = credit.amount;   
}