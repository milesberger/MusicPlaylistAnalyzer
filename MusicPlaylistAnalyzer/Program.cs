using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace MusicPlaylistAnalyzer
{
	// This class encapsulates the data pertaining to a song.
	class Song
	{
		public string Name { get; set; }
		public string Artist { get; set; }
		public string Album { get; set; }
		public string Genre { get; set; }
		public string Size { get; set; }
		public int Time { get; set; }
		public int Year { get; set; }
		public int Plays { get; set; }

		override public string ToString()
		{
			return String.Format("Name: {0}, Artist: {1}, Album: {2}, Genre: {3}, Size: {4}, Time: {5}, Year: {6}, Plays: {7}", Name, Artist, Album, Genre, Size, Time, Year, Plays);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			// Check to see if there are the proper number of command line arguments
			if (args.Length != 2)
			{
				Console.WriteLine("Improper usage. Please run the program like so:");
				Console.WriteLine("MusicPlaylistAnalyzer.exe <music_playlist_file_path> <report_file_path>");
				return;
			}

			// Read the files line by line
			var inputFile = args[0];
            var outputFile = args[1];
			List<Song> songs = new List<Song>();
			try
			{
				var lines = File.ReadLines(inputFile);
				var lineNumber = 0;

				foreach (var line in lines)
				{
					// Make sure to skip the first line (headers)
					if (lineNumber++ == 0)
					{
						continue;
					}

					// Split the line by the tab delimiter (\t)
					var split = line.Split('\t');
					try
					{
						// Initialize a new song with the given info
						Song song;
						try
						{
							song = new Song
							{
								Name = split[0],
								Artist = split[1],
								Album = split[2],
								Genre = split[3],
								Size = split[4],
								Time = Int32.Parse(split[5]),
								Year = Int32.Parse(split[6]),
								Plays = Int32.Parse(split[7])
							};
						}
						catch (Exception)
						{
							Console.WriteLine("Record contains data of invalid type.");
							return;
						}
						songs.Add(song);
					}
					catch (IndexOutOfRangeException)
					{
						Console.WriteLine(String.Format("Row {} contains {} values. It should contain 8.", lineNumber, split.Length));
						return;
					}
				}
			}
			catch (IOException)
			{
				Console.WriteLine("Error opening file. Check that it exists and try again.");
				return;
			}

            try
            {
                using (StreamWriter writetext = new StreamWriter(outputFile))
                {

                    writetext.WriteLine("Music Playlist Report\n");

                    // Songs that received 200 or more plays
                    var songs200plus = from song in songs where song.Plays >= 200 orderby song.Year descending select song;
                    writetext.WriteLine("Songs that received 200 or more plays:");
                    foreach (var song in songs200plus)
                    {
                        writetext.WriteLine(song);
                    }

                    // Number of songs with the genre "Alternative"
                    var alternativeSongs = (from song in songs where song.Genre == "Alternative" select song).Count();
                    writetext.WriteLine("\nNumber of Alternative Songs: " + alternativeSongs);

                    // Number of songs with the genre "Hip-Hop/Rap"
                    var rapSongs = (from song in songs where song.Genre == "Hip-Hop/Rap" select song).Count();
                    writetext.WriteLine("\nNumber of Alternative Songs: " + rapSongs);

                    // Songs from the album "Welcome to the Fishbowl"
                    var fishbowlSongs = from song in songs where song.Album == "Welcome to the Fishbowl" orderby song.Year descending select song;
                    writetext.WriteLine("\nSongs that are from the album \"Welcome to the Fishbowl\":");
                    foreach (var song in fishbowlSongs)
                    {
                        writetext.WriteLine(song);
                    }

                    // Songs from before 1970
                    var oldSongs = from song in songs where song.Year <= 1970 orderby song.Year descending select song;
                    writetext.WriteLine("\nSongs that are from before 1970:");
                    foreach (var song in oldSongs)
                    {
                        writetext.WriteLine(song);
                    }

                    // Song names that are more than 85 characters long
                    var longNameSongs = from song in songs where song.Name.ToCharArray().Length > 85 orderby song.Year descending select song;
                    writetext.WriteLine("\nSongs whose names are more thna 85 characters long:");
                    foreach (var song in longNameSongs)
                    {
                        writetext.WriteLine(song);
                    }

                    // Longest song
                    var longestSong = songs.OrderByDescending(song => song.Time).First();
                    writetext.WriteLine("\nLongest song:");
                    writetext.WriteLine(longestSong);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Error writing to report file. Exiting...");
                return;
            }
		}
	}
}
