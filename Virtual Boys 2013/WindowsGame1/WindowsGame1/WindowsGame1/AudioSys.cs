using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace WindowsGame1
{
	public class AudioSys
	{
		//[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		//public static extern double gmeInit([MarshalAs(UnmanagedType.LPStr)]string fname);
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmeLoad([MarshalAs(UnmanagedType.LPStr)]string fname);
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern int gmeGetTracks();
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmeSetTrack(int track);
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmePlay();
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmePause();
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmeStop();
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmeSetVolume(float volume);
		[DllImport("GMGME.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		public static extern void gmeClose();

		private int tracks = 0;

		public AudioSys()
		{
			//gmeLoad("cv3.nsf");
			//gmePlay();
		}

		~AudioSys()
		{
			gmeStop();
			gmeClose();
		}

		public void init()
		{
			
		}

		public int loadNSF(string file)
		{
			gmeLoad(file);
			tracks = (int)gmeGetTracks();
			return tracks;
		}

		public void loadSFX(string data)
		{

		}

		public int numTracks()
		{
			return tracks;
		}

		public void play()
		{
			gmePlay();
		}

		public void pause()
		{
			gmePause();
		}

		public void stop()
		{
			gmeStop();
		}

		public void setTrack(int track)
		{
			if (track > tracks || 0 > track)
				gmeSetTrack(track);
		}

		public void setVolume(float volume)
		{
			gmeSetVolume(volume);
		}
	};
}
