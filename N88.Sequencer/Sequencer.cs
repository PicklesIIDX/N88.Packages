namespace N88.Sequencer
{
	using System;
	using System.Collections.Generic;

	public class Sequencer<TContext, TState, TEndpoint> where TState : IEquatable<TState>
	{
		private readonly Dictionary<TEndpoint, Func<TContext, TContext>> actions = new();
		private readonly Dictionary<TState, List<TEndpoint>> keyToEndpointMap = new();
		private readonly Dictionary<TState, List<TransitionMapping>> stateTransitionMap = new();

		public Sequencer(TState initialState)
		{
			CurrentState = initialState;
		}

		public TState CurrentState { get; private set; }

		public TState Update(TContext context)
		{
			if (keyToEndpointMap.TryGetValue(CurrentState, out var sequence))
			{
				context = PlaySequence(context, sequence, actions);
			}
			if (stateTransitionMap.TryGetValue(CurrentState, out var transitions))
			{
				CurrentState = TransitionState(transitions, context, CurrentState);
			}
			return CurrentState;
		}

		public void RegisterAction(TState state, TEndpoint endpoint, Func<TContext, TContext> action)
		{
			if (!keyToEndpointMap.ContainsKey(state))
			{
				keyToEndpointMap[state] = new List<TEndpoint>();
			}
			keyToEndpointMap[state].Add(endpoint);
			actions[endpoint] = action;
		}

		public void RegisterTransition(TState fromState, Func<TContext, bool> toStateFunc, TState toState)
		{
			if (!stateTransitionMap.ContainsKey(fromState))
			{
				stateTransitionMap[fromState] = new List<TransitionMapping>();
			}
			stateTransitionMap[fromState].Add(new TransitionMapping(toStateFunc, toState));
		}

		private static TContext PlaySequence(TContext state, List<TEndpoint> sequence,
			Dictionary<TEndpoint, Func<TContext, TContext>> actions)
		{
			foreach (var path in sequence)
			{
				state = actions[path].Invoke(state);
			}
			return state;
		}

		private static TState TransitionState(List<TransitionMapping> transitions, TContext context, TState currentKey)
		{
			foreach (var transition in transitions)
			{
				if (transition.Validator.Invoke(context))
				{
					return transition.DestinationState;
				}
			}
			return currentKey;
		}

		private readonly struct TransitionMapping
		{
			public readonly Func<TContext, bool> Validator;
			public readonly TState DestinationState;

			public TransitionMapping(Func<TContext, bool> validator, TState destinationState)
			{
				Validator = validator;
				DestinationState = destinationState;
			}
		}
	}
}