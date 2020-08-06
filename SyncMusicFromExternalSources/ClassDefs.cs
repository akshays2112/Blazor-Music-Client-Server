using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SyncMusicFromExternalSources
{
    public class MyPlaylist
    {
        public class MyPlayListItem
        {
            public string Title { get; set; }
            public string Id { get; set; }
            public string CleanedUpTitle { get; set; }

            public MyPlayListItem(string title, string id)
            {
                Title = title;
                Id = id;
                if(!string.IsNullOrWhiteSpace(title))
                {
                    string tmpTitle = title;
                    Regex regex = new Regex("(?<Paren>\\(.*\\))");
                    GroupCollection groups = regex.Match(title).Groups;
                    foreach(Capture capture in groups["Paren"].Captures)
                    {
                        tmpTitle = tmpTitle.Replace(capture.Value, null);
                    }
                    regex = new Regex("(?<Brackets>\\[.*\\])");
                    groups = regex.Match(tmpTitle).Groups;
                    foreach (Capture capture in groups["Brackets"].Captures)
                    {
                        tmpTitle = tmpTitle.Replace(capture.Value, null);
                    }
                    regex = new Regex("[ ]{2,}");
                    CleanedUpTitle = regex.Replace(tmpTitle.Replace("-", " ").Replace("Lyrics", null).Replace("lyrics", null).Replace("Original", null).Replace("original", null), " ");
                }
            }
        }

        public string Title { get; set; }
        public string ID { get; set; }
        public long Count { get; set; }
        public List<MyPlayListItem> MyPlayListItems { get; set; }

        public MyPlaylist(string title, string id, long? count)
        {
            Title = title;
            ID = id;
            Count = count ?? 0;
            MyPlayListItems = new List<MyPlayListItem>();
        }
    }

    public class UserPlaylist
    {
        public class UserPlaylistTrack
        {
            public string Name { get; set; }
            public int Index { get; set; }

            public UserPlaylistTrack(string name, int index)
            {
                Name = name;
                Index = index;
            }
        }

        public string Name { get; set; }
        public int Total { get; set; }
        public string Id { get; set; }
        public List<UserPlaylistTrack> UserPlaylistTracks { get; set; }

        public UserPlaylist(string name, int total, string id)
        {
            Name = name;
            Total = total;
            Id = id;
            UserPlaylistTracks = new List<UserPlaylistTrack>();
        }
    }

    public class CustomTrack
    {
        public class CustomArtist
        {
            public string Name { get; set; }
            public string ID { get; set; }

            public CustomArtist(string name, string id)
            {
                Name = name;
                ID = id;
            }
        }

        public string Name { get; set; }
        public string ID { get; set; }
        public List<CustomArtist> Artists { get; set; }

        public CustomTrack(string name, string id)
        {
            Name = name;
            ID = id;
            Artists = new List<CustomArtist>();
        }
    }
}
