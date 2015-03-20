Youtube Video Player

How it works:
You need to pass the youtube video id and the system returns the video .mp4 file. 
You can call the unity3D Handheld.PlayFullScreenMovie to play your video in fullscreen.

Class Usage:
You need to instantiate the YoutubeVideo class.
And call StartCoroutine(youtube.LoadVideo(videoId1)) to get the video .mp4.

Example Scene:
We have one gameobject called YoutubeManager in that gameobject we have 2 fields:
videoId1 and videoId2 you can pass the youtube video id here.
When you run your project on mobile plataforms, if you click in one of the two buttons one video will be loaded in fullscreen.


Update: If you need to use your server to host the php files i added the php files to project, just upload to your server and change the variable
serverGetVideoFile to your getvideo.php path, attention: all files need to be in same directory in server.
-Working on pro version of this plugin, in the new version we will have control to search for videos inside unity, see thumbnails, and much more.
-In pro version we help you to make this work for desktop, inside a texture(in pro this will work on android too).

Support:

If you need help you can send one email to

Email: kelvinparkour@gmail.com