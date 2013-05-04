using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
	class Frame
	{
		int frameTime;

		List<FrameComponent> components;

		public Frame(int frameTime)
		{
			this.frameTime = frameTime;
			components = new List<FrameComponent>();
		}

		public Frame(int frameTime, List<FrameComponent> components)
		{
			this.frameTime = frameTime;
			this.components = components;
		}

		public int FrameTime
		{
			get { return frameTime; }
		}

		public int NumComponents
		{
			get { return components.Count; }
		}

		public void addComponent(FrameComponent component)
		{
			components.Add(component);
		}

		public void insertComponent(FrameComponent component, int index)
		{
			if (index < 0)
				index = 0;
			if (index > components.Count)
				index = components.Count;

			components.Insert(index, component);
		}

		public FrameComponent getComponent(int index)
		{
			return components[index];
		}

		public int getComponentIndex(FrameComponent component)
		{
			return components.IndexOf(component);
		}

		public FrameComponent removeComponentByIndex(int index)
		{
			FrameComponent component = components[index];
			components.RemoveAt(index);
			return component;
		}

	}
}
