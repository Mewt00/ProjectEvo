//-----------------------------------------------------------------------------
// Torque Game Builder
// Copyright (C) GarageGames.com, Inc.
//-----------------------------------------------------------------------------

// Audio description
new AudioDescription(AudioNonLooping)
{
   volume = 1.0;
   isLooping = false;
   is3D = false;
   type = $GuiAudioType;
};

// Whoosh sound effect
new AudioProfile(whoosh)
{
   filename = "~/data/audio/whoosh_mono.wav";
   description = "AudioNonLooping";
   preload = true;
};

