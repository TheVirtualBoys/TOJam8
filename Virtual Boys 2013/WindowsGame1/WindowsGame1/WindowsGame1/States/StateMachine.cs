using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindowsGame1
{
	class StateMachine
	{
		private Stack<BaseState> states = new Stack<BaseState>();

		public void setState(BaseState state)
		{
			while (states.Count > 0) {
				states.Pop().stop();
			}
			states.Push(state);
			state.start();
		}

		public void pushState(BaseState state)
		{
			states.Peek().pause();
			states.Push(state);
			state.start();
		}

		public BaseState popState()
		{
			BaseState state = states.Peek();
			state.stop();
			states.Peek().resume();
			return state;
		}
	}
}
