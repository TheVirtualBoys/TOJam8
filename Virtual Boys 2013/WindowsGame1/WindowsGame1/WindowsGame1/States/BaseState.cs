using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
	public abstract class BaseState
	{
		public abstract void start();
		public abstract void pause();
		public abstract void resume();
		public abstract void stop();
	}
}
