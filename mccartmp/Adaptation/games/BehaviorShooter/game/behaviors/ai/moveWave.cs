//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

$MoveWaveBehavior::sinTable = "0 0.2588 0.5 0.7071 0.866 0.9659 1 0.9659 0.866 0.7071 0.5 0.2588 0 -0.2588 -0.5 -0.7071 -0.866 -0.9659 -1 -0.9659 -0.866 -0.7071 -0.5 -0.2588";
$MoveWaveBehavior::sinOffset = 0;

if (!isObject(MoveWaveBehavior))
{
   %template = new BehaviorTemplate(MoveWaveBehavior);
   
   %template.friendlyName = "Move in a Horizontal Wave";
   %template.behaviorType = "AI";
   %template.description  = "Makes an object move in a wave";

   %dir = "left" TAB "right";
   %template.addBehaviorField(direction, "of motion", enum, "left", %dir);
   %template.addBehaviorField(speed, "(world units per second)", float, 10);
   %template.addBehaviorField(useWaveOffset, "Phase shift the starting position", bool, "0");
   %template.addBehaviorField(waveOffset, "Phase shift increment size (in 15 deg)", int, "0");

}

function MoveWaveBehavior::onAddToScene(%this)
{
   %this.timeStep = 100;
   
   if( %this.useWaveOffset )
   {
      %this.sinStep = $MoveWaveBehavior::sinOffset;
      $MoveWaveBehavior::sinOffset += %this.waveOffset;
      if( $MoveWaveBehavior::sinOffset > getWordCount( $MoveWaveBehavior::sinTable ) )
         $MoveWaveBehavior::sinOffset = 0;
   }
   else
      %this.sinStep = 0;
   
   %this.scheduleNext();
}

function MoveWaveBehavior::scheduleNext(%this)
{
   %this.updateSchedule = %this.schedule( %this.timeStep, "updateMove");   
}

function MoveWaveBehavior::updateMove(%this)
{   
   %dir = -1;
   if( %this.direction $= "right" )
      %dir = 1;
   
   %nextPos = t2dVectorAdd(%this.owner.position, %dir * %this.speed * %this.timeStep / 1000 SPC getWord($MoveWaveBehavior::sinTable, %this.sinStep));
   
   %this.sinStep++;
   if( %this.sinStep >= getWordCount($MoveWaveBehavior::sinTable) )
      %this.sinStep = 0;
   
   %this.owner.moveTo( getWord(%nextPos,0), getWord(%nextPos,1), %this.speed, false, false, false, 0.1);
   %this.scheduleNext();
   
}
