$card::suits = "h d c s";
$card::values = "1 2 3 4 5 6 7 8 9 10 11 12 13";

function card::onAdd(%this)
{
   //echo("I am a" SPC %this.value SPC "of" SPC %this.suit);
   if (%this.callbackActive)
   {
      %this.onStatusUpdate();
   }
}

function card::onRemove(%this)
{
   // hmmm... may need to figure out how to propagate delete...
}

function card::getCard(%this)
{
   return %this.value @ %this.suit;
}

function card::getValue(%this)
{
   return %this.value;
}

function card::getSuit(%this)
{
   return %this.suit;
}

function card::getFacing(%this)
{
   return %this.facing;
}

function card::setFacing(%this, %facing)
{
   %this.facing = %facing;
   if (%this.callbackActive)
   {
      %this.onStatusUpdate();
   }
}

function card::toggleFacing(%this)
{
   if (%this.facing $= "up")
   {
      %this.facing = "down";
   }
   else if (%this.facing $= "down")
   {
      %this.facing = "up";
   }
   else
   {
      error("Bad facing!");
   }
   if (%this.callbackActive)
   {
      %this.onStatusUpdate();
   }
}