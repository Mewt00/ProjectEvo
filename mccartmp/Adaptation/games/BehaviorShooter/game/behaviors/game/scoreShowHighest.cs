//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(HighScoreBehavior))
{
   %template = new BehaviorTemplate(HighScoreBehavior);
   
   %template.friendlyName = "Scoreboard TopScore";
   %template.behaviorType = "Game";
   %template.description  = "Makes a text object display the highest score from a scoreboard. ($pref::highScore)";

   %template.addBehaviorField(prefix, "Highscore's prefix", string, "Best: ");
   %template.addBehaviorField(suffix, "Highscore's suffix", string, "");
}

function HighScoreBehavior::onBehaviorAdd(%this)
{   
   %this.updateText();
}

function HighScoreBehavior::onLevelLoaded(%this)
{
   //if($pref::highScore $= "")
    //  $pref::highScore = 0;
}

function HighScoreBehavior::updateText(%this)
{
   %this.owner.text = %this.prefix @ $pref::highScore @ %this.suffix;
}
