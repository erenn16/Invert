PARTICLE EFFECTS - ASSIGNMENT 4

Game controls are A/Left and D/Right for movement.  Use Space to jump and Shift to invert to the other color layers.  To activate the particle effect, move the character to collide with the spikes.

Character design by Courtney Toder.

I am using the particle effect system provided by Unity and played with the settings to test the different uses.  In the game here, I added a gravity scale and upped the initial velocity so I would get the parabolic effect and upped the number of particles to 200 or so.  There doesn't seem to be any lag or crashing when I up the particles to an enormous number like 300,000,000.  I tried out some different materials to find the best effect and decided on the default Diffuse setting.  The particle system seems to be really good for Unity and doesn't lag or crash the system as far as I can tell by trying to max out the different settings.

Though there is a weird bit where when I simulate the effect while not running the game I can see more particles, but if I have the effect during the game running, there seems to be less.

All code that activates/deactivates the particle effect is in CharacterMovement.cs