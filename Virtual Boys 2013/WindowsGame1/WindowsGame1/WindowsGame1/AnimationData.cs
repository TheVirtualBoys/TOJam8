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

		public AnimationData(int loopCount, List<Frame> frames)
		{
			this.loopCount = loopCount;
			this.frames = frames;
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

	}
}
