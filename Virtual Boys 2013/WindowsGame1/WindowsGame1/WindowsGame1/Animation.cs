using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace WindowsGame1
{
	public class Animation
	{
		int loopCount;

		AnimationData aniData;

		int curFrameIndex;

		/**
		 * The already used time of the current frame
		 */
		long curFrameTime;

		public Animation(AnimationData aniData)
		{
			this.aniData = aniData;

			loopCount = this.aniData.LoopCount;
			curFrameIndex = -1;
			curFrameTime = 0;
		}

		public bool IsLooping
		{
			get { return loopCount != 0; }
		}

		public int LoopCount
		{
			get { return this.loopCount; }
			set { this.loopCount = (value >= -1)? value : -1; }
		}

		public Frame getFrame(int index)
		{
			return aniData.getFrame(index);
		}

		public int NumFrames
		{
			get { return aniData.NumFrames; }
		}

		public int CurFrameIndex
		{
			get { return curFrameIndex; }
		}

		public Frame CurFrame
		{
			get { return (curFrameIndex >= 0 && curFrameIndex < aniData.NumFrames)? aniData.getFrame(curFrameIndex) : (curFrameIndex == aniData.NumFrames)? aniData.getFrame(curFrameIndex - 1) : null; }
		}

		public bool IsFinished
		{
			get { return curFrameIndex >= aniData.NumFrames; }
		}

		public bool IsStarted
		{
			get { return curFrameIndex >= 0; }
		}

		public void reset()
		{
			curFrameIndex = -1;
			curFrameTime = 0;
		}

		public void start()
		{
			curFrameIndex = 0;
		}

		public void update(GameTime updateTime)
		{
			if (IsFinished)
				return;
			if (!IsStarted)
				start();

			curFrameTime += updateTime.ElapsedGameTime.Milliseconds;
			while (!IsFinished && curFrameTime >= CurFrame.FrameTime)
			{
				nextFrame();
			}
		}

		public void nextFrame()
		{
			curFrameTime -= CurFrame.FrameTime;
			curFrameIndex += 1;

			if (curFrameIndex >= aniData.NumFrames)
			{
				if (IsLooping)
				{
					curFrameIndex = 0;
					if (loopCount > 0)
						--loopCount;
				}
				else
				{
					//not looping and we've gone past the last frame, so set finished
					curFrameIndex = aniData.NumFrames;
					curFrameTime = 0;
				}
			}
		}

		public void setFrame(int index)
		{
			if (index < 0)
				index = 0;
			if (index >= aniData.NumFrames)
				index = aniData.NumFrames;

			curFrameTime = 0;
			curFrameIndex = index;
		}
	}
}
