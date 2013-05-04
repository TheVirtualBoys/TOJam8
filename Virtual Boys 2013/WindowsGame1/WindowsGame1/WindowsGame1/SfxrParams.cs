using System;
namespace SfxrSynth
{
	public enum WaveType { Square, Saw, Sin, Noise };

	public class SfxrParams
	{
		public WaveType WaveType { get; set; }

		public double MasterVolume { get; set; }

		public double AttackTime { get; set; }
		public double SustainTime { get; set; }
		public double SustainPunch { get; set; }
		public double DecayTime { get; set; }

		public double StartFrequency { get; set; }
		public double MinFrequency { get; set; }

		public double Slide { get; set; }
		public double DeltaSlide { get; set; }

		public double VibratoDepth { get; set; }
		public double VibratoSpeed { get; set; }

		public double ChangeAmount { get; set; }
		public double ChangeSpeed { get; set; }

		public double SquareDuty { get; set; }
		public double DutySweep { get; set; }

		public double RepeatSpeed { get; set; }

		public double PhaserOffset { get; set; }
		public double PhaserSweep { get; set; }

		public double lpFilterCutoff { get; set; }
		public double lpFilterCutoffSweep { get; set; }
		public double lpFilterResonance { get; set; }

		public double hpFilterCutoff { get; set; }
		public double hpFilterCutoffSweep { get; set; }

		public SfxrParams()
		{
		}

		public SfxrParams(string s)
			: this()
		{
			FromString(s);
		}

		public SfxrParams(SfxrParams p)
		{
			WaveType = p.WaveType;

			MasterVolume = p.MasterVolume;

			AttackTime = p.AttackTime;
			SustainTime = p.SustainTime;
			SustainPunch = p.SustainPunch;
			DecayTime = p.DecayTime;

			StartFrequency = p.StartFrequency;
			MinFrequency = p.MinFrequency;

			Slide = p.Slide;
			DeltaSlide = p.DeltaSlide;

			VibratoDepth = p.VibratoDepth;
			VibratoSpeed = p.VibratoSpeed;

			ChangeAmount = p.ChangeAmount;
			ChangeSpeed = p.ChangeSpeed;

			SquareDuty = p.SquareDuty;
			DutySweep = p.DutySweep;

			RepeatSpeed = p.RepeatSpeed;

			PhaserOffset = p.PhaserOffset;
			PhaserSweep = p.PhaserSweep;

			lpFilterCutoff = p.lpFilterCutoff;
			lpFilterCutoffSweep = p.lpFilterCutoffSweep;
			lpFilterResonance = p.lpFilterResonance;

			hpFilterCutoff = p.hpFilterCutoff;
			hpFilterCutoffSweep = p.hpFilterCutoffSweep;
		}

		public void FromString(string s)
		{
			string[] splitStrings = s.Split(new char[] { ',' });

			for(int i = 0; i < splitStrings.Length; i++)
				if(splitStrings[i].Length == 0)
					splitStrings[i] = "0";

			WaveType = splitStrings[0] == "0" ? WaveType.Square : splitStrings[0] == "1" ? WaveType.Saw : splitStrings[0] == "2" ? WaveType.Sin : WaveType.Noise;
			AttackTime = float.Parse(splitStrings[1]);
			SustainTime = float.Parse(splitStrings[2]);
			SustainPunch = float.Parse(splitStrings[3]);
			DecayTime = float.Parse(splitStrings[4]);
			StartFrequency = float.Parse(splitStrings[5]);
			MinFrequency = float.Parse(splitStrings[6]);
			Slide = float.Parse(splitStrings[7]);
			DeltaSlide = float.Parse(splitStrings[8]);
			VibratoDepth = float.Parse(splitStrings[9]);
			VibratoSpeed = float.Parse(splitStrings[10]);
			ChangeAmount = float.Parse(splitStrings[11]);
			ChangeSpeed = float.Parse(splitStrings[12]);
			SquareDuty = float.Parse(splitStrings[13]);
			DutySweep = float.Parse(splitStrings[14]);
			RepeatSpeed = float.Parse(splitStrings[15]);
			PhaserOffset = float.Parse(splitStrings[16]);
			PhaserSweep = float.Parse(splitStrings[17]);
			lpFilterCutoff = float.Parse(splitStrings[18]);
			lpFilterCutoffSweep = float.Parse(splitStrings[19]);
			lpFilterResonance = float.Parse(splitStrings[20]);
			hpFilterCutoff = float.Parse(splitStrings[21]);
			hpFilterCutoffSweep = float.Parse(splitStrings[22]);
			MasterVolume = float.Parse(splitStrings[23]);
		}

		public void Mutate(double mutation)
		{
			Random r = new Random();

			if (r.NextDouble() < 0.5) StartFrequency += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) MinFrequency += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) Slide += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) DeltaSlide += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) SquareDuty += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) DutySweep += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) VibratoDepth += r.NextDouble() * mutation*2 - mutation;
			if (r.NextDouble() < 0.5) VibratoSpeed += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) AttackTime += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) SustainTime += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) DecayTime += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) SustainPunch += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) lpFilterCutoff += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) lpFilterCutoffSweep += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) lpFilterResonance += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) hpFilterCutoff += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) hpFilterCutoffSweep += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) PhaserOffset += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) PhaserSweep += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) RepeatSpeed += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) ChangeSpeed += r.NextDouble() * mutation * 2 - mutation;
			if (r.NextDouble() < 0.5) ChangeAmount += r.NextDouble() * mutation * 2 - mutation;
		}

		public SfxrParams GetMutation(double mutation)
		{
			SfxrParams p = new SfxrParams(this);
			p.Mutate(mutation);
			return p;
		}
	}
}
