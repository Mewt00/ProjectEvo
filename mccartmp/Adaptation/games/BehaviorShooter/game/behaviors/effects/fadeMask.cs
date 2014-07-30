//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(fadeMaskBehavior))
{
   %template = new BehaviorTemplate(fadeMaskBehavior);
   
   %template.friendlyName = "Fade Mask";
   %template.behaviorType = "Effects";
   %template.description  = "Makes an object fade in or out when the level is loaded";

   %template.addBehaviorField(duration, "fade length (seconds)", float, "2.0");
   %template.addBehaviorField(stepCount, "fade quality (higher is better)", int, 20);
   %template.addBehaviorField(startDelay, "seconds", float, "0.5");
   %template.addBehaviorField(startColor4, "color before fade", color, "1 1 1 1");
   %template.addBehaviorField(endColor4, "color at end of fade", color, "1 1 1 0");
}

function fadeMaskBehavior::onLevelLoaded(%this)
{
   %this.schedule( %this.startDelay * 1000, "fade" );
 
   %this.step = 0;
   %this.timeStep = %this.duration*1000 / %this.stepCount;
   
   %this.curR = getWord( %this.startColor4, 0 );
   %this.curG = getWord( %this.startColor4, 1 );
   %this.curB = getWord( %this.startColor4, 2 );
   %this.curA = getWord( %this.startColor4, 3 );
   
   %this.rStep = (getWord(%this.endColor4, 0)-%this.curR) / %this.stepCount;
   %this.gStep = (getWord(%this.endColor4, 1)-%this.curG) / %this.stepCount;
   %this.bStep = (getWord(%this.endColor4, 2)-%this.curB) / %this.stepCount;
   %this.aStep = (getWord(%this.endColor4, 3)-%this.curA) / %this.stepCount;
   
   %this.owner.setBlendColor(%this.curR, %this.curG, %this.curB);
   %this.owner.setBlendAlpha( %this.curA );
   
}

function fadeMaskBehavior::fade(%this)
{
   %this.curR += %this.rStep;
   %this.curG += %this.gStep;
   %this.curB += %this.bStep;
   %this.curA += %this.aStep;
   
   %this.owner.setBlendColor(%this.curR, %this.curG, %this.curB);
   %this.owner.setBlendAlpha( %this.curA );
   
   %this.step++;
   if( %this.step < %this.stepCount )
      %this.schedule( %this.timeStep, "fade" );
      
}
