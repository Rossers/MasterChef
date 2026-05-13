Name: Ross A. Metcalf
Email: pg09ross@vfs.com
Date: 20170219
Class: T2 Unity Development 1 (w/ Ivo van der Marel)
Assignment: A2 - Create a Game: "Master Chef: Cooking Evolved"
Summary:

2D side-scrolling platformer.

Controls: 
	A - Move left
	D - Move right
	S - Crouch / Reload
	Space - Jump
	LMB - Fire
	ESC - Pause menu

Features: 

	Moving platforms - mMve back and forth between two points.
	Weak platforms - When stepped on, shake and fall.
	Health Pickup - Recovers all lost health.
	Ovenant (enemies)
		- Dutch: Large health and slow. Can crouch under their detection & line of fire.
		- Mini: Tiny and fast, but low health. Must crouch to shoot the Minis.
	Pause Menu - Freezes gameplay, displays scores, allows resume or go to main menu.
	Death Menu - Lets player restart or go to main menu.
	Music - Menu and gameplay background music.
	SFX - Player and enemies have different shooting, hit, and death SFX.
	Animations - Player, enemies & projectile are animated sprites.

Description:

I combined everything I learned from the 3D, 2D & 2Player games we made in class into this project. 

The enemy movement is similar to Bario movement, but I added horizontal raycasters to detect the player. If they detect a player they stop moving and shoot. Enemies have a second, smaller collider that kills the player if they touch it. 

Moving platforms are like those from Bario, but I added weak platforms that detect if the player steps on it which triggers it to shake and fall. 

I took advantage of the owner mechanic for the projectiles because I was having problems with projectiles and colliders before we learned that in class. These projectiles add a bit more complexity because they don't just kill the target, they reduce its health and activate the target to play a special recieved-damage sound effect.

I used what I learned from the Bario animations to make animations for the player model and the projectile. I had really cool particle effects for the moving/weak platforms, but they had a strange effect on the pause menu which made the pause menu mostly unreadable so I got rid of them.

The pause/death menus use layered canvases so I could display different text depending on if the player was paused or dead, but I didn't have to make duplicate UI text fields with the score. I used Time.deltaTime to pause the projectiles so the game fully pauses in the background. Counters are kept of enemies killed, shots fired, and health packs used. 

The health packs are similar to the collectables from Bario. I couldn't really think of what to add to those. I didn't want to make ammo pickups because shooting is fun so the player should always be able to shoot. 

I purposefully made the Crouch button the same as the reload. This adds an interesting mechanic because you can't move or jump while crouched. You can crouch mid-air to reload. Crouching also reduces the size of your collider, which lets you duck under the DutchOvenant's projectiles and detection raycaster. The player must crouch to shoot Mini enemies or they will shoot over top of them.

