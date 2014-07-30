//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ScoreBoardBehavior))
{
   %template = new BehaviorTemplate(ScoreBoardBehavior);
   
   %template.friendlyName = "Scoreboard Text";
   %template.behaviorType = "Game";
   %template.description  = "Makes a text object act as a scoreboard.";

   %template.addBehaviorField(prefix, "Text's prefix", string, "Score: ");
   %template.addBehaviorField(suffix, "Text's suffix", string, "");
   %template.addBehaviorField(initVal, "Initial value", int, 0);
   %template.addBehaviorField(save, "Save highscore to $pref::highScore", bool, "0");
   %template.addBehaviorField(highScoreboard, "optional Highscore text object", object, "", t2dSceneObject);
}

function ScoreBoardBehavior::onBehaviorAdd(%this)
{
   //displayScore is the currently shown score
   //score is the score it's counting to
   %this.displayScore = %this.initVal;
   %this.score = %this.initVal;
   
   %this.updateText();      
}

function ScoreBoardBehavior::onLevelLoaded(%this)
{
   //if($pref::highScore $= "")
   //  $pref::highScore = 1;
}

function ScoreBoardBehavior::updateText(%this)
{
   %this.owner.text = %this.prefix @ %this.displayScore @ %this.suffix;
   
   if( %this.score < %this.displayScore )
      %this.displayScore--;
   else if( %this.score > %this.displayScore )
      %this.displayScore++;
   else
   {
      if( %this.score > $pref::highScore )
      {
         $pref::highScore = %this.score;
         %this.updateHighest();
      }
      return;
   }
   
   %this.updateSchedule = %this.schedule(75, "updateText");
}

//Sets the score instantly instead of counting to it
function ScoreBoardBehavior::setScore(%this, %newScore)
{
   %this.displayScore = %newScore;
   %this.score = %newScore;
   %this.updateText();
}

//Counts the current score to the newScore
function ScoreBoardBehavior::incrementScore(%this, %newScore)
{
   %this.score += %newScore;
   %this.updateText();
}

//updates the highscore board if there is one
function ScoreBoardBehavior::updateHighest(%this)
{
   if( isObject(%this.highScoreBoard) )
      %this.highScoreBoard.updateText();
}