using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace SfxrSynth
{
	public class Synthesizer
	{
		public SoundEffect CreateSound(SfxrParams sfxrParams)
		{
			localParams = new SfxrParams(sfxrParams);

			Reset(true);

			int[] samples = new int[(int)envelopeFullLength];
			int samplesInBuffer = 0;

			bool finished = false;

			for (int i = 0; i < samples.Length; i++)
			{
				if (finished)
					break;

				// Repeats every _repeatLimit times, partially resetting the sound parameters
				if(repeatLimit != 0)
				{
					if(++repeatTime >= repeatLimit)
					{
						repeatTime = 0;
						Reset(false);
					}
				}
				
				// If _changeLimit is reached, shifts the pitch
				if(changeLimit != 0)
				{
					if(++changeTime >= changeLimit)
					{
						changeLimit = 0;
						period *= changeAmount;
					}
				}
				
				// Acccelerate and apply slide
				slide += deltaSlide;
				period *= slide;
				
				// Checks for frequency getting too low, and stops the sound if a minFrequency was set
				if(period > maxPeriod)
				{
					period = maxPeriod;
					if(minFrequency > 0.0)
						finished = true;
				}
				
				periodTemp = period;
				
				// Applies the vibrato effect
				if(vibratoAmplitude > 0.0)
				{
					vibratoPhase += vibratoSpeed;
					periodTemp = period * (float)(1.0 + Math.Sin(vibratoPhase) * vibratoAmplitude);
				}
				
				periodTemp = (int)periodTemp;
				if(periodTemp < 8)
					periodTemp = 8;
				
				// Sweeps the square duty
				if (waveType == 0)
				{
					squareDuty += dutySweep;
					if(squareDuty < 0.0f)
						squareDuty = 0.0f;
					else if (squareDuty > 0.5f)
						squareDuty = 0.5f;
				}
				
				// Moves through the different stages of the volume envelope
				if(++envelopeTime > envelopeLength)
				{
					envelopeTime = 0;
					
					switch(++envelopeStage)
					{
						case 1:
							envelopeLength = envelopeLength1;
							break;
						case 2:
							envelopeLength = envelopeLength2;
							break;
					}
				}
				
				// Sets the volume based on the position in the envelope
				switch(envelopeStage)
				{
					case 0:
						envelopeVolume = envelopeTime * envelopeOverLength0;
						break;
					case 1:
						envelopeVolume = 1.0f + (1.0f - envelopeTime * envelopeOverLength1) * 2.0f * sustainPunch;
						break;
					case 2:
						envelopeVolume = 1.0f - envelopeTime * envelopeOverLength2;
						break;
					case 3:
						envelopeVolume = 0.0f;
						finished = true;
						break;
				}
				
				// Moves the phaser offset
				if (phaser)
				{
					phaserOffset += phaserDeltaOffset;
					phaserInt = (int)phaserOffset;
					if(phaserInt < 0)
						phaserInt = -phaserInt;
					else if (phaserInt > 1023)
						phaserInt = 1023;
				}
				
				// Moves the high-pass filter cutoff
				if(filters && hpFilterDeltaCutoff != 0.0)
				{
					hpFilterCutoff *= hpFilterDeltaCutoff;
					if(hpFilterCutoff < 0.00001f)
						hpFilterCutoff = 0.00001f;
					else if(hpFilterCutoff > 0.1f)
						hpFilterCutoff = 0.1f;
				}
				
				superSample = 0.0f;
				for(int j = 0; j < 8; j++)
				{
					// Cycles through the period
					phase++;
					if(phase >= periodTemp)
					{
						phase = phase - (int)periodTemp;
						
						// Generates new random noise for this period
						if(waveType == WaveType.Noise) 
						{ 
							for(int n=0; n<32; n++)
								noiseBuffer[n] = (float)random.NextDouble() * 2.0f - 1.0f;
						}
					}
					
					// Gets the sample from the oscillator
					switch(waveType)
					{
						case WaveType.Square: // Square wave
						{
							sample = ((phase / periodTemp) < squareDuty) ? 0.5f : -0.5f;
							break;
						}
						case WaveType.Saw: // Saw wave
						{
							sample = 1.0f - (phase / periodTemp) * 2.0f;
							break;
						}
						case WaveType.Sin: // Sine wave (fast and accurate approx)
						{
							pos = phase / periodTemp;
							pos = pos > 0.5f ? (pos - 1.0f) * 6.28318531f : pos * 6.28318531f;
							sample = pos < 0 ? 1.27323954f * pos + .405284735f * pos * pos : 1.27323954f * pos - 0.405284735f * pos * pos;
							sample = sample < 0 ? .225f * (sample *-sample - sample) + sample : .225f * (sample * sample - sample) + sample;
							
							break;
						}
						case WaveType.Noise: // Noise
						{
							sample = noiseBuffer[(phase * 32 / (int)periodTemp) % 32];
							break;
						}
					}
					
					// Applies the low and high pass filters
					if (filters)
					{
						lpFilterOldPos = lpFilterPos;
						lpFilterCutoff *= lpFilterDeltaCutoff;
						if(lpFilterCutoff < 0.0f)
							lpFilterCutoff = 0.0f;
						else if(lpFilterCutoff > 0.1f)
							lpFilterCutoff = 0.1f;
						
						if(lpFilterOn)
						{
							lpFilterDeltaPos += (sample - lpFilterPos) * lpFilterCutoff;
							lpFilterDeltaPos *= lpFilterDamping;
						}
						else
						{
							lpFilterPos = sample;
							lpFilterDeltaPos = 0.0f;
						}
						
						lpFilterPos += lpFilterDeltaPos;
						
						hpFilterPos += lpFilterPos - lpFilterOldPos;
						hpFilterPos *= 1.0f - hpFilterCutoff;
						sample = hpFilterPos;
					}
					
					// Applies the phaser effect
					if (phaser)
					{
						phaserBuffer[phaserPos & 1023] = sample;
						sample += phaserBuffer[(phaserPos - phaserInt + 1024) & 1023];
						phaserPos = (phaserPos + 1) & 1023;
					}
					
					superSample += sample;
				}
				
				// Averages out the super samples and applies volumes
				superSample = masterVolume * envelopeVolume * superSample * 0.125f;
				
				// Clipping if too loud
				if(superSample > 1.0f)
					superSample = 1.0f;
				else if(superSample < -1.0f)
					superSample = -1.0f;

				samples[i] = (int)(32000.0 * superSample);

				samplesInBuffer++;
			}

			byte[] byteSamples = new byte[samplesInBuffer * 2];
			for (int i = 0; i < samplesInBuffer; i++)
			{
				if(BitConverter.IsLittleEndian)
				{
					byteSamples[i * 2] = (byte)(samples[i] & 0xff);
					byteSamples[i * 2 + 1] = (byte)(samples[i] >> 8);
				}
				else
				{
					byteSamples[i * 2] = (byte)(samples[i] >> 8);
					byteSamples[i * 2 + 1] = (byte)(samples[i] & 0xff);
				}
			}

			return new SoundEffect(byteSamples, 44100, AudioChannels.Mono);
		}

		public SoundEffect CreateSound(string paramsString)
		{
			return CreateSound(new SfxrParams(paramsString));
		}

		public SoundEffect CreateMutation(SfxrParams sfxrParams, double mutation)
		{
			return CreateSound(sfxrParams.GetMutation(mutation));
		}

		public SoundEffect CreateMutation(string paramsString, double mutation)
		{
			return CreateMutation(new SfxrParams(paramsString), mutation);
		}

		private void Reset(bool totalReset)
		{
			SfxrParams p = localParams;

			period = 100.0f / (p.StartFrequency * p.StartFrequency + 0.001f);
			maxPeriod = 100.0f / (p.MinFrequency * p.MinFrequency + 0.001f);
			
			slide = 1.0f - p.Slide * p.Slide * p.Slide * 0.01f;
			deltaSlide = -p.DeltaSlide * p.DeltaSlide * p.DeltaSlide * 0.000001f;
			
			if (p.WaveType == WaveType.Square)
			{
				squareDuty = 0.5f - p.SquareDuty * 0.5f;
				dutySweep = -p.DutySweep * 0.00005f;
			}
			
			if (p.ChangeAmount > 0.0f)
				changeAmount = 1.0f - p.ChangeAmount * p.ChangeAmount * 0.9f;
			else
				changeAmount = 1.0f + p.ChangeAmount * p.ChangeAmount * 10.0f;
			
			changeTime = 0;
			
			if(p.ChangeSpeed == 1.0)
				changeLimit = 0;
			else
				changeLimit = (int)((1.0f - p.ChangeSpeed) * (1.0f - p.ChangeSpeed) * 20000 + 32);
			
			if(totalReset)
			{
				masterVolume = p.MasterVolume * p.MasterVolume;
				
				waveType = p.WaveType;
				
				if (p.SustainTime < 0.01f)
					p.SustainTime = 0.01f;
				
				double totalTime = p.AttackTime + p.SustainTime + p.DecayTime;
				if (totalTime < 0.18) 
				{
					double multiplier = 0.18 / totalTime;
					p.AttackTime *= multiplier;
					p.SustainTime *= multiplier;
					p.DecayTime *= multiplier;
				}
				
				sustainPunch = p.SustainPunch;
				
				phase = 0;
				
				minFrequency = p.MinFrequency;
				
				filters = p.lpFilterCutoff != 1.0 || p.hpFilterCutoff != 0.0;
				
				lpFilterPos = 0.0f;
				lpFilterDeltaPos = 0.0f;
				lpFilterCutoff = p.lpFilterCutoff * p.lpFilterCutoff * p.lpFilterCutoff * 0.1f;
				lpFilterDeltaCutoff = 1.0f + p.lpFilterCutoffSweep * 0.0001f;
				lpFilterDamping = 5.0f / (1.0f + p.lpFilterResonance * p.lpFilterResonance * 20.0f) * (0.01f + lpFilterCutoff);
				if (lpFilterDamping > 0.8) lpFilterDamping = 0.8f;
				lpFilterDamping = 1.0f - lpFilterDamping;
				lpFilterOn = p.lpFilterCutoff != 1.0f;
				
				hpFilterPos = 0.0f;
				hpFilterCutoff = p.hpFilterCutoff * p.hpFilterCutoff * 0.1f;
				hpFilterDeltaCutoff = 1.0f + p.hpFilterCutoffSweep * 0.0003f;
				
				vibratoPhase = 0.0f;
				vibratoSpeed = p.VibratoSpeed * p.VibratoSpeed * 0.01f;
				vibratoAmplitude = p.VibratoDepth * 0.5f;
				
				envelopeVolume = 0.0f;
				envelopeStage = 0;
				envelopeTime = 0;
				envelopeLength0 = p.AttackTime * p.AttackTime * 100000.0f;
				envelopeLength1 = p.SustainTime * p.SustainTime * 100000.0f;
				envelopeLength2 = p.DecayTime * p.DecayTime * 100000.0f + 10;
				envelopeLength = envelopeLength0;
				envelopeFullLength = envelopeLength0 + envelopeLength1 + envelopeLength2;
				
				envelopeOverLength0 = 1.0f / envelopeLength0;
				envelopeOverLength1 = 1.0f / envelopeLength1;
				envelopeOverLength2 = 1.0f / envelopeLength2;
				
				phaser = p.PhaserOffset != 0.0f || p.PhaserSweep != 0.0f;
				
				phaserOffset = p.PhaserOffset * p.PhaserOffset * 1020.0f;
				if(p.PhaserOffset < 0.0)
					phaserOffset = -phaserOffset;
				phaserDeltaOffset = p.PhaserSweep * p.PhaserSweep * p.PhaserSweep * 0.2f;
				phaserPos = 0;

				for(int i=0; i<1024; i++)
					phaserBuffer[i] = 0;

				for(int i=0; i<32; i++)
					noiseBuffer[i] = (float)random.NextDouble() * 2.0f - 1.0f;
				
				repeatTime = 0;

				if (p.RepeatSpeed == 0.0)
					repeatLimit = 0;
				else
					repeatLimit = (int)((1.0f - p.RepeatSpeed) * (1.0f - p.RepeatSpeed) * 20000) + 32;
			}
		}

		Random random = new Random();

		SfxrParams localParams;

		double masterVolume;

		WaveType waveType;

		double envelopeVolume;
		int envelopeStage;
		double envelopeTime;
		double envelopeLength;
		double envelopeLength0;
		double envelopeLength1;
		double envelopeLength2;
		double envelopeOverLength0;
		double envelopeOverLength1;
		double envelopeOverLength2;
		double envelopeFullLength;

		double sustainPunch;

		int phase;
		double pos;
		double period;
		double periodTemp;
		double maxPeriod;

		double slide;
		double deltaSlide;
		double minFrequency;

		double vibratoPhase;
		double vibratoSpeed;
		double vibratoAmplitude;

		double changeAmount;
		int changeTime;
		int changeLimit;

		double squareDuty;
		double dutySweep;

		int repeatTime;
		int repeatLimit;

		bool phaser;
		double phaserOffset;
		double phaserDeltaOffset;
		int phaserInt;
		int phaserPos;
		double[] phaserBuffer = new double[1024];

		bool filters;
		double lpFilterPos;
		double lpFilterOldPos;
		double lpFilterDeltaPos;
		double lpFilterCutoff;
		double lpFilterDeltaCutoff;
		double lpFilterDamping;
		bool lpFilterOn;

		double hpFilterPos;
		double hpFilterCutoff;
		double hpFilterDeltaCutoff;

		double[] noiseBuffer = new double[32];

		double superSample;
		double sample;
	}
}
