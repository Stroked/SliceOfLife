# SliceOfLife: A Musical Spice
>>>>>>>>>>*A-Work-In-Progress by Aj & Sam*

***

#### **Resources:**

  1. **[SpotifyDownloader](https://github.com/NickLawsonDev/SpotifySongDownloader) by [Nick Lawson](https://github.com/NickLawsonDev)**
		
		    This open source project requires a Spotify 'ClientId' & a 'Secret Key' in addition to a Youtube API Key. 

  2. **[Spotify-Downloader](https://github.com/Ritiek/Spotify-Downloader) by [Ritiek](https://github.com/Ritiek)**
		
			This project is in python but has a lot more features relevant to SoL.
****

#### Goals:

   + *Simply extend from  [Nick's C# code](https://github.com/NickLawsonDev/SpotifySongDownloader) and avoid writing a lot of logic and functionality by ourselves*  
				
			  

   + *Integrate a local file management system to maintain a full back-up of all songs and playlists.*



****
## Featured Components

### **RemoteManager**


> The goal is to have a consistent way to manage playlists across itunes, spotify and youtube. 

	Requirements: 
	1. Auto-Updated playlists across remote clients and services.

### **LocalManager**
Since having a local copy is essential for the purpose of full back-up, a `FileManager` is required for mobile (android & iOS) and PC (windows & iOS)

_Requirements:_

+ Eventually-Consistent record of files across devices. 
+ Handle the following file-formats: `.mp3`
### Specification

1. Sources
			
	A. LocalFileSystem 
			
        a. WINDOWS
						
        
        b. Mac


   
  B. Spotify (nuget: )
				
			Q. How to get `Spotify Developer API` Key?
				


  C. Youtube (nuget: YoutubeExplode)

			Q. How to get `Youtube Developer API` Key?



  



