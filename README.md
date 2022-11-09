# Playlist-Manager
This utility is a basic Windows desktop application that enables export of iTunes playlists to text. All playlists are exported in a single batch, with a couple of clicks. The result is a list of text files, one for each playlist. Each text file contains the track titles of the given playlist, in the format "Artist name - Track Title".

## Prerequisites

1. On your Windows computer, open iTunes and go to **Edit > Preferences**.
2. Click **Advanced** and select the **Share iTunes Library XML with other applications** check box.

As a result, a new XML file should be created in **C:\Users\<username>\Music\iTunes** folder. This is your iTunes XML Library file and you will need it in subsequent steps.

## How to export playlists

1. Run the Playlist Manager executable.
2. Click **Browse** and select your iTunes XML Library file obtained previously. Be patient while the information is extracted; this can take a couple of minutes, depending on the size of your library. Once the operation is complete, the number of playlists found is displayed in the application's toolbar.
3. Click **Export** and select a destination folder where all text files should be saved.
