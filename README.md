# uBuild

Unity3D Multi-Build Manager.

Sick of changing Builds all the time, choosing a folder and all that stuff?
Sick of zipping / archiving your Builds manually?
Great so was I.

But wait, uBuild to the rescue!

Simple and slick project-based Build Management.
One-Button Builds (as far as supported - means iPhone Builds on Window is a no-go)

## Current Features

* Manage multiple builds in one interface.
* Automatically place them in a certain root directory (Default: "./Builds/")


## Upcoming Features

* Per Build Post-System-Operation (useful for using your preferable zipping/packaging tool)
	* Basically a commandline execution.
	* Variables: 
		* $BUILD_DIR$ - The dir of the actual Build
* Per Build Resource Management
	* Useful if you use Editor Extensions (common way is to store Extension Assets in Resource/ due to availability, which then get packaged into the Build)






