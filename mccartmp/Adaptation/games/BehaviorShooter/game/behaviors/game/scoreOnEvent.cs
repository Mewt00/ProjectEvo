//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ScoreOnEventBehavior))
{
   %template = new BehaviorTemplate(ScoreOnEventBehavior);
   
   %template.friendlyName = "Score on Event";
   %template.behaviorType = "Game";
   %template.description  = "Affects a scoreBoard text object.";

   %template.addBehaviorField(scoreboard, "Scoreboard text object", object, "", t2dSceneObject);
   %template.addBehaviorField(value, "Point value", int, 10);
   
   %template.addBehaviorField(scoreOnRemove, "Award points when object is deleted", bool, "1");
   %template.addBehaviorField(scoreOnCollision, "Award points when object collides", bool, "0");
   %template.addBehaviorField(scoreOnWorldLimit, "Award poitns when object hits a world limit", bool, "0");
}

function ScoreOnEventBehavior::onBehaviorAdd(%this)
{
   if (%this.scoreOnCollision)
      %this.owner.collisionCallback = true;
   
   if (%this.scoreOnWorldLimit)
      %this.owner.worldLimitCallback = true;
}


function ScoreOnEventBehavior::onCollision(%this, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts)
{
   if (%this.scoreOnCollision)
   {
      %scoreboardBehavior = %this.scoreboard.getBehavior("ScoreBoardBehavior");  
      if (isObject(%scoreboardBehavior))
         %scoreboardBehavior.incrementScore( %this.value );
   }
}

function ScoreOnEventBehavior::onRemove(%this)
{
   if (%this.scoreOnRemove)
   {
      %scoreboardBehavior = %this.scoreboard.getBehavior("ScoreBoardBehavior");  
      if (isObject(%scoreboardBehavior))
         %scoreboardBehavior.incrementScore( %this.value );
   }
}

function ScoreOnEventBehavior::onWorldLimit(%this, %mode, %side)
{
   if (%this.scoreOnWorldLimit)
   {
      %scoreboardBehavior = %this.scoreboard.getBehavior("ScoreBoardBehavior");  
      if (isObject(%scoreboardBehavior))
         %scoreboardBehavior.incrementScore( %this.value );
   }
}
