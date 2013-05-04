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
		List<Frame> frames;

		int curFrameIndex;

		int posLeft;
		int posTop;

		/**
		 * The already used time of the current frame
		 */
		long curFrameTime;


		public Animation()
		{
			loopCount = 0;
			frames = new List<Frame>();

			curFrameIndex = -1;
			posLeft = 0;
			posTop = 0;

			curFrameTime = 0;
		}

		public Animation(int loopCount)
		{
			this.loopCount = loopCount;
			frames = new List<Frame>();

			curFrameIndex = -1;
			posLeft = 0;
			posTop = 0;

			curFrameTime = 0;
		}

		public Animation(int loopCount, List<Frame> frames)
		{
			this.loopCount = loopCount;
			this.frames = frames;

			curFrameIndex = -1;
			posLeft = 0;
			posTop = 0;

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
			return frames[index];
		}

		public int NumFrames
		{
			get { return frames.Count; }
		}

		public void addFrame(Frame frame)
		{
			frames.Add(frame);
		}

		public void insertFrame(Frame frame, int index)
		{
			if (index < 0)
				index = 0;
			if (index > frames.Count)
				index = frames.Count;
			frames.Insert(index, frame);
		}

		public int CurFrameIndex
		{
			get { return curFrameIndex; }
		}

		public Frame CurFrame
		{
			get { return (curFrameIndex >= 0 && curFrameIndex < frames.Count)? frames[curFrameIndex] : null; }
		}

		public int Left
		{
			get { return posLeft; }
			set { posLeft = value; }
		}

		public int Top
		{
			get { return posTop; }
			set { posTop = value; }
		}

		public bool IsFinished
		{
			get { return curFrameIndex > frames.Count; }
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

			if (curFrameIndex >= frames.Count)
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
					curFrameIndex = frames.Count;
					curFrameTime = 0;
				}
			}
		}

		public void setFrame(int index)
		{
			if (index < 0)
				index = 0;
			if (index >= frames.Count)
				index = frames.Count;

			curFrameTime = 0;
			curFrameIndex = index;
		}
	}
}
