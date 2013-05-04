using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using SfxrSynth;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

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

		public enum Effect
		{
			SFX_BEGIN = 0,
			SFX_GET,
			SFX_ABSORB,
			SFX_REFLECT,
			SFX_JUMP,
			SFX_LAND,
			SFX_DIE,
			SFX_RELEASE,
			SFX_MAX
		}

		private int tracks = 0;
		private Synthesizer synth;
		private SfxrParams[] sParams;
		
		public AudioSys()
		{
		}

		~AudioSys()
		{
			gmeStop();
			gmeClose();
		}

		public void init()
		{
			synth								= new Synthesizer();
			sParams								= new SfxrParams[(int)Effect.SFX_MAX];
			sParams[(int)Effect.SFX_BEGIN]		= new SfxrParams("0,0.3384,0.5445,0.5561,0.4956,0.3772,,0.0062,0.2456,0.0003,0.7595,0.0796,-0.1607,0.6218,-0.006,-0.6183,-0.6678,-0.024,0.5126,0.0081,0.1318,0.0469,-0.0077,0.5");
			sParams[(int)Effect.SFX_GET]		= new SfxrParams("0,0.3384,0.5445,0.5561,0.4956,0.3772,,0.0062,0.2456,0.0003,0.7595,0.0796,-0.1607,0.6218,-0.006,-0.6183,-0.6678,-0.024,0.5126,0.0081,0.1318,0.0469,-0.0077,0.5");
			sParams[(int)Effect.SFX_ABSORB]		= new SfxrParams("3,0.3354,0.0955,0.0334,0.3164,0.5026,,,-0.2044,0.4221,0.6595,-0.95,0.8183,0.5397,0.5124,0.1875,0.7482,0.5152,0.9954,0.0183,,0.0002,0.0006,0.65");
			sParams[(int)Effect.SFX_REFLECT]	= new SfxrParams("2,,0.01,0.1423,0.1738,0.4757,0.0333,0.0092,-1,0.0436,0.0897,0.5108,0.8621,0.3061,0.1588,,0.7395,0.4665,0.6025,-0.5165,0.8161,,0.0676,0.65");
			sParams[(int)Effect.SFX_JUMP]		= new SfxrParams("0,,0.01,0.4467,0.948,0.0377,,0.7849,0.3974,,0.5108,0.897,0.3748,,-0.4317,,-0.1089,,0.4358,0.1187,0.6332,0.7691,-0.7573,0.5");
			sParams[(int)Effect.SFX_LAND]		= new SfxrParams("0,,0.0124,0.007,0.1675,0.2577,0.0554,-0.4136,0.0306,0.0098,0.0408,-0.0413,0.0391,0.4048,,,,-0.0201,0.9586,-0.0417,,,0.0241,0.5");
			sParams[(int)Effect.SFX_DIE]		= new SfxrParams("3,0.0168,0.2308,0.6785,0.4391,0.1938,,-0.3691,0.0193,0.0296,0.0336,0.7328,0.6829,,0.0321,0.7725,-0.0098,-0.0353,1,0.0324,0.0131,,,0.5");
			sParams[(int)Effect.SFX_RELEASE]	= new SfxrParams("2,,0.248,0.1032,0.2221,0.6241,0.0519,-0.5553,,,,,,0.0633,0.0022,,,,1,,,0.1231,,0.5");
			// maybe make permutations here?
		}

		public int loadNSF(string file)
		{
			gmeLoad(file);
			tracks = (int)gmeGetTracks();
			return tracks;
		}

		public int numTracks()
		{
			return tracks;
		}

		public void playSFX(AudioSys.Effect e)
		{
			synth.CreateMutation(sParams[(int)e], 0.1).Play();
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
