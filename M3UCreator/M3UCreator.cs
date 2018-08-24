using Id3;
using System;
using System.IO;
using System.Linq;

namespace M3UCreator
{
    public class M3UCreator
    {
        /// <summary>
        /// Generates a m3u file with all mp3 files in the specified directory
        /// </summary>
        /// <param name="mp3Directory"></param>
        /// <param name="m3uName">The optional name of the m3u file. If not specified, the name of the directory containing the mp3 files is used.</param>
        /// <param name="m3uDirectory">The optional directory where the m3u file is created. If not specified, the m3u file is created in the directory containing the mp3 files.</param>
        public void CreateM3u(string mp3Directory, string m3uName = null, string m3uDirectory = null)
        {
            var directoryInfo = new DirectoryInfo(mp3Directory);

            string m3uFilename = String.IsNullOrEmpty(m3uName) ? directoryInfo.Name : m3uName;
            m3uFilename = Path.ChangeExtension(m3uFilename, ".m3u");

            string m3uDirectoryName = String.IsNullOrEmpty(m3uDirectory) ? mp3Directory : m3uDirectory;
            Directory.CreateDirectory(m3uDirectoryName);

            string m3uFile = Path.Combine(m3uDirectoryName, m3uFilename);
            

            using (var m3u = new StreamWriter(m3uFile, false))
            {
                m3u.WriteLine("#EXTM3U");

                foreach (string mp3File in Directory.EnumerateFiles(mp3Directory, "*.mp3", SearchOption.AllDirectories))
                {
                    String mp3FileRelative = Path.GetRelativePath(m3uDirectoryName, mp3File);

                    using (var mp3 = new Mp3(mp3File))
                    {
                        Id3Tag tag = mp3.GetTag(Id3TagFamily.Version2X);

                        m3u.WriteLine($"#EXTINF:{tag.Length.Value.TotalSeconds:F0},{tag.Artists.Value.FirstOrDefault() ?? ""} - {tag.Title}");
                        m3u.WriteLine(mp3FileRelative);
                    }
                }
            }
        }
    }
}
