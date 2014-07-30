//-----------------------------------------------------------------------------
// EffectsModule: Effects class and functions
//-----------------------------------------------------------------------------

function EffectsModule::create( %this )
{
    exec("./scripts/Effects.cs");
    exec("./scripts/damageSplashScreen.cs");
    exec("./scripts/shadowDust.cs");
}

//-----------------------------------------------------------------------------

function EffectsModule::destroy( %this )
{
}
