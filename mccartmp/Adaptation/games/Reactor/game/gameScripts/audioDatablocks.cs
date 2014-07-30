//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

//the current unused pop sound effect
$popNumber = 0;

/// ()
/// prepares the 16 audio descriptions and profiles (one for each channel)
/// All the channels will be used to create the overlapping sound effects
// instead of simply running one on top of another
function audioSetup()
{
   for(%a = 0; %a < 16; %a++){
      new AudioDescription("AudioNonLooping" @ %a)
      {
         volume = 0.5;
         isLooping= false;
         is3D = false;
         type = $GuiAudioType;
         channel = %a;
      };

      new AudioProfile("popAudio" @ %a)
      {
         filename = "~/data/audio/BLIP.wav";
         description = "AudioNonLooping" @ %a;
         preload = true;
      };      
      
      
   }
}

/// ()
/// schedules a sound to be played within the next 200 milliseconds
/// Done this way so that not all the sounds are played at the same time
function playNextPop()
{

   schedule(getRandom(0, 200), 0, "playPop");
}

/// ()
/// Plays the sound in the next available channel
/// 
function playPop()
{
   alxPlay("popAudio" @ $popNumber);
   
   $popNumber++;
   if($popNumber >= 16)
   {
      $popNumber = 0;
   }
}