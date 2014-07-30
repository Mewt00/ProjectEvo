if (!isObject(DriftBehavior))
{
  %template = new BehaviorTemplate(DriftBehavior);

  %template.friendlyName = "Drift Down";
  %template.behaviorType = "Movement";
  %template.description  = "Drift Down.  Recycle Object At Bottom";

  %template.addBehaviorField(minSpeed, "Minimum speed to fall", float, 350.0);
  %template.addBehaviorField(maxSpeed, "Maximum speed to fall", float, 375.0);
}

function DriftBehavior::onBehaviorAdd(%this)
{
	%this.recycle();
}

function DriftBehavior::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == 5)	//Player sceneGroup 
	{
		%this.recycle(%object.side);
	}
	else if(%object.getSceneGroup() == 15)
	{
		if(%object.side $= "top")
			%this.recycle(%object.side);
	}
}

function DriftBehavior::recycle(%this)
{
  %this.owner.setPosition(getRandom(-$roomWidth/2, $roomWidth/2), $roomHeight/2);
  %this.owner.setLinearVelocityX( 0 );
  %this.owner.setLinearVelocityY( -1000 );//-getRandom(%this.minSpeed, %this.maxSpeed) 
}