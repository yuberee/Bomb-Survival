﻿using Sandbox;
using Sandbox.Internal;
using Sandbox.Physics;

namespace BombSurvival;

public partial class Player
{
	private float cameraDistance = 250f;
	private TimeSince lastMoved;
	public bool IsZoomed = false;

	public void ComputeCamera()
	{
		var nearbyPlayers = Entity.FindInSphere( Position, 512 )
			.OfType<Player>()
			.Where( x => x != this ) // Exclude the current player
			.ToList();

		Vector3 centerPoint = this.Position;
		float maxDistance = 350f; // Default camera distance
		float minDistance = 200f; // Minimum camera distance

		if ( nearbyPlayers.Count > 0 )
		{
			Vector3 othersCenter = nearbyPlayers.Aggregate( Vector3.Zero, ( sum, player ) => sum + player.Position ) /
			                       nearbyPlayers.Count;
			maxDistance = nearbyPlayers.Max( player => Vector3.DistanceBetween( centerPoint, player.Position ) );

			// Adjust centerPoint to be a weighted average between player's position (70% weight) and othersCenter (30% weight)
			centerPoint = 0.7f * this.Position + 0.3f * othersCenter;
		}

		if ( Velocity.Length > 1f )
			lastMoved = 0f;

		if ( lastMoved > 2.5f )
		{
			centerPoint = this.Position; // focus on the current player
			cameraDistance = cameraDistance.LerpTo( 100f, Time.Delta * lastMoved );
			IsZoomed = true;
		}
		else
		{
			cameraDistance = Math.Clamp( maxDistance, minDistance, float.MaxValue );
			IsZoomed = false;
		}

		if ( Camera.Position == Vector3.Zero && BombSurvival.Instance.CurrentState is PodState )
			Camera.Position = centerPoint + Vector3.Right * cameraDistance + Vector3.Up * 64f;

		if ( Camera.Position == Vector3.Zero )
			Camera.Position = Checkpoint.First().RespawnPosition;
		else
		{
			var wishPosition = centerPoint;
			Camera.Position = Vector3.Lerp( Camera.Position,
				wishPosition + Vector3.Right * cameraDistance + Vector3.Up * 64f, Time.Delta * 5f );
		}

		Camera.Rotation = Rotation.FromYaw( 90f );
		Camera.FieldOfView = Screen.CreateVerticalFieldOfView( Game.Preferences.FieldOfView );

		/*var postProcess = Camera.Main.FindOrCreateHook<Sandbox.Effects.DepthOfField>();
		postProcess.Enabled = true;

		postProcess.BlurSize = 500;
		postProcess.FocalDistance = cameraDistance * 1.5f;
		postProcess.FrontBlur = false;
		postProcess.BackBlur = true;*/
	}
}
