if (!isObject(DriftBehavior))
{
  %template = new BehaviorTemplate(DriftBehavior);

  %template.friendlyName = "Drift Down";
  %template.behaviorType = "Movement";
  %template.description  = "Drift Down.  Recycle Object At Bottom";

  %template.addBehaviorField(speed, "Minimum speed to fall", float, 10.0);
}

function DriftBehavior::onBehaviorAdd(%this)
{
	%this.recycle();
}

function DriftBehavior::onCollision(%this, %object, %collisionDetails)
{
	if(%object.getSceneGroup() == Utility.getCollisionGroup("Player") )	//Player sceneGroup 
	{
		//%this.recycle(%object.side);
		//%object.takeDamage(15);
	}
	else if(%object.getSceneGroup() == Utility.getCollisionGroup("Wall"))
	{
		if(%object.side $= "top")
			%this.recycle(%object.side);
	}
}

function DriftBehavior::recycle(%this)
{
  %this.owner.setPosition(getRandom(-$roomWidth/4, $roomWidth/4), $roomHeight/2);
  %this.owner.setLinearVelocityX( getRandom(-%this.speed/4, %this.speed/4) );
  %this.owner.setLinearVelocityY( -%this.speed );
}