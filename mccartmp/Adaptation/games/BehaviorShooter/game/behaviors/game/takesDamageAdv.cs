//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

if (!isObject(TakesDamageAdvBehavior))
{
   %template = new BehaviorTemplate(TakesDamageAdvBehavior);
   
   %template.friendlyName = "Takes Damage Advanced";
   %template.behaviorType = "Game";
   %template.description  = "Set the object to take damage from DealsDamage objects that collide with it";

   %template.addBehaviorField(health, "The amount of health the object has", int, 100);
   %template.addBehaviorField(lives, "The number of times the object can lose all its health", int, 3);
   %template.addBehaviorField(tintRedForDamage, "Tint the object red as it takes damage", bool, 0);
   %template.addBehaviorField(respawnTime, "The time between death and respawn (seconds)", float, 2.0);
   %template.addBehaviorField(invincibleTime, "The time after spawning before damage is applied (seconds)", float, 1.0);
   %template.addBehaviorField(respawnEffect, "The particle effect to play on spawn", object, "", t2dParticleEffect);
   %template.addBehaviorField(explodeEffect, "The particle effect to play on death", object, "", t2dParticleEffect);
   
   %template.addBehaviorField(scoreboard, "Optional scoreboard text object", object, "", t2dSceneObject);
   %template.addBehaviorField(value, "Point value for being destroyed", int, 10);
   %template.addBehaviorField(resetScore, "Reset scoreboard when killed", bool, "0");
   
   %template.addBehaviorField(nextLevel, "Level to load upon total death (\blank\ does nothing)", String, "");
}

function TakesDamageAdvBehavior::onAddToScene(%this)
{
   %this.startHealth = %this.health;
   %this.startFrame = %this.owner.getFrame();
   %this.spawn();
}

function TakesDamageAdvBehavior::takeDamage(%this, %amount, %assailant)
{
   if (%this.invincible)
      return;
   
   %this.health -= %amount;
   if (%this.health <= 0)
   {
      //award points for the kill
      if (isObject(%this.scoreboard))
      {
         %scoreboardBehavior = %this.scoreboard.getBehavior("ScoreBoardBehavior");  
         if (isObject(%scoreboardBehavior))
         {
            if( %this.resetScore )
               %scoreboardBehavior.setScore( 0 );
            else
               %scoreboardBehavior.incrementScore( %this.value );
         }
            
      }
      
      //explode and kill the object
      %this.explode();
      %this.kill();
      return;
   }
   
   if(%this.tintRedForDamage)
   {
      %tint = %this.health / %this.startHealth;
      %this.owner.setBlendColor(1, %tint, %tint, 1);
   }
}

function TakesDamageAdvBehavior::kill(%this)
{
   %this.lives--;
   if (%this.lives <= 0)
   {
      %this.owner.safeDelete();
      
      
      //error("Killing: "@%this@". Its nextLevel is <"@%this.nextLevel@">");
      if( %this.nextLevel !$= "" &&  %this.nextLevel !$= "0")
      {
         schedule( 2000, 0, "TakesDamageAdvBehavior_loadLevel", %this.nextLevel );
      }
         
      return;
   }
   
   %this.invincible = true;
   %this.owner.visible = false;
   %this.owner.collisionActiveReceive = false;
   %this.schedule(%this.respawnTime * 1000, "spawn");
}

function TakesDamageAdvBehavior::setVincible(%this)
{
   %this.invincible = false;
}

function TakesDamageAdvBehavior::spawn(%this)
{
   %this.owner.collisionActiveReceive = true;
   %this.schedule(%this.invincibleTime * 1000, "setVincible");
   %this.health = %this.startHealth;
   %this.owner.setBlendColor(1, 1, 1, 1);
   %this.owner.visible = true;
   %this.owner.setFrame(%this.startFrame);
   
   if (isObject(%this.respawnEffect))
   {
      %explosion = %this.respawnEffect.cloneWithBehaviors();
      %explosion.position = %this.owner.position;
      %explosion.setEffectLifeMode("Kill", 1.0);
      %explosion.playEffect();
   }
}

function TakesDamageAdvBehavior::explode(%this)
{
   if (isObject(%this.explodeEffect))
   {
      %explosion = %this.explodeEffect.cloneWithBehaviors();
      %explosion.position = %this.owner.position;
      %explosion.setEffectLifeMode("Kill", 1.0);
      %explosion.playEffect();
   }
}

function TakesDamageAdvBehavior_loadLevel(%level)
{
   sceneWindow2D.endLevel();
   sceneWindow2D.loadLevel( expandFilename( "game/data/levels/" @ %level @ ".t2d"));
}
   
