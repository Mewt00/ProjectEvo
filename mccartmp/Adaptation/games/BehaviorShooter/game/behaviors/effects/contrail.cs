//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(ContrailBehavior))
{
   %template = new BehaviorTemplate(ContrailBehavior);
   
   %template.friendlyName = "Contrail";
   %template.behaviorType = "Effects";
   %template.description  = "This should be added to a particle effect. Plays the effect when the object it is mounted to is moving";
}

function ContrailBehavior::onBehaviorAdd(%this)
{
   %this.owner.enableUpdateCallback();
   %this.playing = false;
}

function ContrailBehavior::onUpdate(%this)
{
   %parent = %this.owner.getMountedParent();
   if (!isObject(%parent))
      return;
   
   if (t2dVectorLength(%parent.linearVelocity) >= 0.1)
   {
      %this.owner.rotation = -%parent.rotation;
      if (!%this.playing)
      {
         %this.owner.playEffect(false);
         %this.playing = true;
      }
   }
   else
   {
      %this.owner.stopEffect(true, false);
      %this.playing = false;
   }
}
