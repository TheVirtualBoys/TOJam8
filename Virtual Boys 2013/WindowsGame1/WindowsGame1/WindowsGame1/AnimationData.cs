using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
	public class AnimationData
	{
		int loopCount;
		List<Frame> frames;
		string name;

		public AnimationData(int loopCount, List<Frame> frames, string name)
		{
			this.loopCount = loopCount;
			this.frames = frames;
			this.name = name;
		}

		public int LoopCount
		{
			get { return this.loopCount; }
		}

		public Frame getFrame(int index)
		{
			return frames[index];
		}

		public int NumFrames
		{
			get { return frames.Count; }
		}

		public string Name
		{
			get { return name; }
		}

	}
}
