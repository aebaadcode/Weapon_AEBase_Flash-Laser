//////////
// item //
//////////
datablock ItemData(LaserPointerItem)
{
	category = "Weapon";  // Mission editor category
	className = "Weapon"; // For inventory system

	 // Basic Item Properties
	shapeFile = "./LaserPointer/LaserPointerItem.dts";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	//gui stuff
	uiName = "Laser Pointer";
	iconName = "./Icons/2";
	doColorShift = false;
	colorShiftColor = "0.5 0.5 0.5 1.000";

	 // Dynamic properties defined by the scripts
	image = LaserPointerImage;
	canDrop = true;

	AEBase = 1;

	uiColor = "1 1 1";
	description = "Industrial grade laser designator (pointer).";
	
	useImpactSounds = true;
	softImpactThreshold = 2;
	softImpactSound = "AEWepImpactSoft1Sound AEWepImpactSoft2Sound AEWepImpactSoft3Sound";
	hardImpactThreshold = 8;
	hardImpactSound = "AEWepImpactHard1Sound AEWepImpactHard2Sound AEWepImpactHard3Sound";
};

////////////////
//weapon image//
////////////////
datablock ShapeBaseImageData(LaserPointerImage)
{
   // Basic Item properties
   shapeFile = "./LaserPointer/LaserPointer.dts";
   emap = true;

   // Specify mount point & offset for 3rd person, and eye offset
   // for first person rendering.
   mountPoint = 0;
   offset = "0 0 0";
   eyeOffset = "0 0 0";
   rotation = eulerToMatrix( "0 0 0" );

   // When firing from a point offset from the eye, muzzle correction
   // will adjust the muzzle vector to point to the eye LOS point.
   // Since this weapon doesn't actually fire from the muzzle point,
   // we need to turn this off.  
   correctMuzzleVector = true;

   // Add the WeaponImage namespace as a parent, WeaponImage namespace
   // provides some hooks into the inventory system.
   className = "WeaponImage";

   // Projectile && Ammo.
   item = LaserPointerItem;
	
   //melee particles shoot from eye node for consistancy
   melee = false;
   //raise your arm up or not
   armReady = true;
   hideHands = true;
   doColorShift = false;
   colorShiftColor = LaserPointerItem.colorShiftColor;//"0.400 0.196 0 1.000";
 
	laserSize = "1.25 1.25 1.25";
	laserColor = "1.0 0.0 0.0 1";
	laserDistance = 500;
	laserOffStates = "Activate FDraw Holster Wait";
    laserFade = 64;
    ignoreAToggle = true;

   //casing = " ";

   // Images have a state system which controls how the animations
   // are run, which sounds are played, script callbacks, etc. This
   // state system is downloaded to the client so that clients can
   // predict state changes and animate accordingly.  The following
   // system supports basic ready->fire->reload transitions as
   // well as a no-ammo->dryfire idle state.
   // Initial start up state
	stateName[0]                     	= "Activate";
	stateTimeoutValue[0]             	= 0.1;
	stateTransitionOnTimeout[0]        = "Wait";
//	stateSequence[0]			= "FStart";

	stateName[1]                       = "FDraw";
	stateTransitionOnTimeout[1]        = "Ready";
	stateScript[1]                     = "onDraw";
	stateTimeoutValue[1]             	= 0.4;
	stateSequence[1]                = "FDraw";

	stateName[2]                     	= "Ready";
	stateScript[2]				= "onReady";
	stateTimeoutValue[2]               = 10;
	stateSequence[2]                = "Idle";
	stateTransitionOnTriggerUp[2]  	= "Holster";
	stateTransitionOnTimeout[2]        = "ReadyLoop";
	stateWaitForTimeout[2]			= false;
	stateAllowImageChange[2]         	= true;
	
	stateName[3]				= "ReadyLoop";
	stateTransitionOnTimeout[3]		= "Ready";	
	
	stateName[4]                       = "Holster";
	stateTransitionOnTimeout[4]        = "Wait";
	stateScript[4]                     = "onHolster";
	stateTimeoutValue[4]             	= 0.4;
	stateSequence[4]                = "Holster";

	stateName[5]				= "Wait";
	stateTransitionOnTriggerDown[5]		= "FDraw";	
	stateTimeoutValue[5]             	= 0.1;
//	stateSequence[5]			= "FStart";
};

// THERE ARE THREE STAGES OF VISUAL RECOIL, NONE, PLANT, JUMP

function LaserPointerImage::onDraw(%this,%obj,%slot)
{	
	%obj.DrawSoundSchedule = schedule(350, 0, serverPlay3D, FlashlightClickSound, %obj.getPosition());
}

function LaserPointerImage::onHolster(%this, %obj, %slot)
{
	%obj.HolsterSoundSchedule = serverPlay3D(FlashlightClickSound,%obj.getPosition());	
}

// HIDES ALL HAND NODES

function LaserPointerImage::onMount(%this,%obj,%slot)
{
	%this.AEMountSetup(%obj, %slot);

	parent::onMount(%this,%obj,%slot);
}

// APLLY BODY PARTS IS LIKE PRESSING CTRL O AND ESC, IT APPLIES THE AVATAR COLORS FOR YOU

function LaserPointerImage::onUnMount(%this,%obj,%slot)
{
	%this.AEUnmountCleanup(%obj, %slot);

	cancel(%obj.DrawSoundSchedule);
	cancel(%obj.HolsterSoundSchedule);

	parent::onUnMount(%this,%obj,%slot);	
}